//Code written by Albert liu and Alex Hudson
//Last updated 12/5/2019
//Skeleton Code provided by professor Kopta

using ClassLibrary;
using Model;
using NetworkUtil;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace Control
{
    public class ServerController
    {
        public static World TheWorld { get; private set; }
        private Dictionary<long, SocketState> clients;
        private TcpListener listener;
        private TcpListener httplistener;
        private static int powId = 0;
        private static int projectileId = 0;
        private ArrayList data_awaiting_updates;
        private Dictionary<int, GameModel> games;
        public static long TimeSpan { get; set; } // length of time in game

        /// <summary>
        /// Constructor for the server controller
        /// </summary>
        /// <param name="filename">File for the server settings</param>
        public ServerController(string filename)
        {
            clients = new Dictionary<long, SocketState>();
            data_awaiting_updates = new ArrayList();
            TimeSpan = 0; 
            BuildObject(filename);
        }

        /// <summary>
        /// Set the games data
        /// </summary>
        /// <param name="games"></param>
        public void SetGames(Dictionary<int, GameModel> games)
        {
            this.games = games;
        }

        /// <summary>
        /// read xml file, create a world object 
        /// </summary>
        /// <param name="filename"></param>
        private static void BuildObject(string filename)
        {
            TheWorld = new World();
            using (XmlReader reader = XmlReader.Create(filename))
            {
                Vector2D GenerateVector2D() // building the vector2d based on the information parsed
                {
                    double x = 0;
                    double y = 0;

                    reader.Read();
                    if (reader.IsStartElement())
                    {
                        if (reader.Name.Equals("x"))
                        {
                            reader.Read(); // step into reading x's value
                            double.TryParse(reader.Value, out x);
                            reader.Read();  // end x
                        }
                    }

                    reader.Read();  // start reading y 
                    if (reader.IsStartElement())
                    {
                        if (reader.Name.Equals("y"))
                        {
                            reader.Read(); // step into reading y's value
                            double.TryParse(reader.Value, out y);
                            reader.Read(); // end y
                        }
                    }

                    return new Vector2D(x, y);
                }

                int id = 0;

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "GameSettings":
                                break;
                            case "UniverseSize":
                                reader.Read();
                                if (int.TryParse(reader.Value, out int grid))
                                    TheWorld.SetGrid(grid);
                                break;
                            case "MSPerFrame":
                                reader.Read();
                                if (int.TryParse(reader.Value, out int frate))
                                    TheWorld.SetFrame(frate);
                                break;
                            case "FramesPerShot":
                                reader.Read();
                                if (int.TryParse(reader.Value, out int fps))
                                    TheWorld.SetFramePerShot(fps);
                                break;
                            case "RespawnRate":
                                reader.Read();
                                if (int.TryParse(reader.Value, out int rr))
                                    TheWorld.SetRespawnRate(rr);
                                break;
                            case "Wall":
                                Walls w;
                                Vector2D p1 = new Vector2D();
                                Vector2D p2 = new Vector2D();
                                reader.Read();
                                if (reader.IsStartElement())
                                    if (reader.Name.Equals("p1"))
                                        p1 = GenerateVector2D();
                                reader.Read();

                                reader.Read();
                                if (reader.IsStartElement())
                                    if (reader.Name.Equals("p2"))
                                        p2 = GenerateVector2D();
                                w = new Walls(id++, p1, p2);
                                TheWorld.WallList.Add(w.Wall, w);
                                reader.Read();
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Start accepting Tcp sockets connections from clients
        /// </summary>
        public void StartServer()
        {
            // This begins an "event loop"
            listener = Networking.StartServer(NewClientConnected, 11000);
            Console.WriteLine("Server is running ...");
        }

        /// <summary>
        /// Start accepting http sockets connections from clients
        /// </summary>
        public void StartHttpServer()
        {
            // This begins an "event loop"
            httplistener = Networking.StartServer(NewHttpClientConnected, 80);
            Console.WriteLine("Http Server is running ...");
        }

        /// <summary>
        /// End the server 
        /// </summary>
        public void EndServer()
        {
            DBManager.SaveData(TheWorld);

            Networking.StopServer(listener);
            Networking.StopServer(httplistener);
            Console.WriteLine("Server is closing ...");
        }

        /// <summary>
        ///  Creates a New HTTP Client
        /// </summary>
        /// <param name="state">The socket state</param>
        private void NewHttpClientConnected(SocketState state)
        {
            if (state.ErrorOccured)  
            {
                int disconnected_id = (int)state.ID;
                HandleDisconnection(disconnected_id);
                return;
            }

            state.OnNetworkAction = ServeHttpRequest;

            Networking.GetData(state);
        }

        /// <summary>
        /// Generates the data to serve request to webserver
        /// </summary>
        /// <param name="state">The socket state</param>
        private void ServeHttpRequest(SocketState state)
        {
            if (state.ErrorOccured) 
            {
                int disconnected_id = (int)state.ID;
                HandleDisconnection(disconnected_id);
                return;
            }
            string request = state.GetData();

            if (request.Contains("GET"))
            {
                if (request.Contains("player="))
                {
                    int idx = request.IndexOf("player=") + 7;  // end index
                    int idx1 = request.IndexOf("HTTP", idx);  // begin index
                    string name = request.Substring(idx, idx1 - idx).Trim();

                    Networking.SendAndClose(state.TheSocket, WebViews.GetPlayerGames(name, DBManager.GetPlayerData(name)));
                }
                else if (request.Contains("games"))
                {
                    Networking.SendAndClose(state.TheSocket, WebViews.GetAllGames(games));
                }
                else
                {
                    Networking.SendAndClose(state.TheSocket, WebViews.Get404());
                }
            }
            else
            {
                Networking.SendAndClose(state.TheSocket, WebViews.Get404());
            }
        }

        /// <summary>
        /// Set a timer to update once every frame
        /// </summary>
        public void SetTimer()
        {
            Stopwatch watch = new Stopwatch();
            int fps = 1000 / TheWorld.GetFrame();

            while (true)
            {
                watch.Start();
                while (watch.ElapsedMilliseconds < TheWorld.GetFrame())
                { /* do nothing */ }

                watch.Reset();

                Update();
            }
        }

        /// <summary>
        /// update the world, Do collision detection and send data to all clients
        /// </summary>
        public void Update()
        {
            StringBuilder data_to_send = new StringBuilder("");
            ArrayList data_to_remove = new ArrayList();
            Random random = new Random();
            lock (TheWorld)
            {
                //randomly spawn powerups
                UpdateSpawnPowHelper(random);

                //update all of the items in the world
                UpdateAwaitingUpdate();

                //update tanks and check for deaths/DC's
                UpdateTankHelper(ref data_to_remove, ref data_to_send);

                //update projectiles and check for collisions with walls or tanks
                UpdateProjectileHelper(ref data_to_remove, ref data_to_send);

                //update powerups and check if tank has picked it up
                UpdatePowerUpHelper(ref data_to_remove, ref data_to_send);

                //update beam and check for colisions with tanks
                UpdateBeamHelper(ref data_to_remove, ref data_to_send);

                //Remove all of the data that needs to be romeved
                UpdateRemoveHelper(ref data_to_remove, ref data_to_send);
            }

            //send all of our data to send
            UpdateSendHelper(ref data_to_send);


        }
        /// <summary>
        /// helper method for update
        /// has a random chance of spawning a power up somewhere random in the world
        /// </summary>
        /// <param name="random"></param>
        private void UpdateSpawnPowHelper(Random random)
        {
            lock (data_awaiting_updates)
            {
                //spawn power ups randomly with a max number of 3 in the world at any time
                if (TheWorld.PowerList.Values.Count < 3)
                {
                    //random spawn check its 1/250 chance to spawn power up per frame
                    if (random.Next(250) == 1)
                    {
                        data_awaiting_updates.Add(SpawnPowerUps());
                    }
                }
            }
        }

        /// <summary>
        /// helper method for update
        ///  Itterates through and adds all the data to the world that needs to be updated
        /// </summary>
        private void UpdateAwaitingUpdate()
        {
            lock (data_awaiting_updates)
            {
                foreach (object o in data_awaiting_updates)
                {
                    if (o is Tanks)
                    {
                        TheWorld.TankList[(o as Tanks).Tank] = o as Tanks;
                    }
                    else if (o is Projectiles)
                    {
                        TheWorld.ProjectileList[(o as Projectiles).Proj] = o as Projectiles;
                    }
                    else if (o is Powerups)
                    {
                        TheWorld.PowerList[(o as Powerups).Power] = o as Powerups;
                    }
                    else if (o is Beams)
                    {
                        TheWorld.BeamList[(o as Beams).Beam] = o as Beams;
                    }
                }

                data_awaiting_updates.Clear();
            }
        }

        /// <summary>
        /// helper method for update
        /// itterates through all the tanks and checks for deaths or dc's
        /// </summary>
        /// <param name="data_to_remove">The data that will be deleted from the world</param>
        /// <param name="data_to_send">The data that will be sent to clients</param>
        private void UpdateTankHelper(ref ArrayList data_to_remove, ref StringBuilder data_to_send)
        {
            foreach (Tanks t in TheWorld.TankList.Values)
            {
                t.DecrementCoolDown();

                lock (clients)
                {
                    if (t.Dc || !clients.ContainsKey((long)t.Tank)) // if client has been removed from the socket list or disconnected
                    {
                        t.Died = true;
                        t.Hp = 0;
                        data_to_remove.Add(t);
                        continue; // do not add tank to the string yet
                    }
                    else if (t.Hp == 0) // tank is dead
                    {
                        data_to_remove.Add(t);
                        continue;
                    }
                }

                data_to_send.Append(JsonConvert.SerializeObject(t) + "\n");

                if (t.Join)
                    t.Join = false; // set join to be false 
            }
        }

        /// <summary>
        /// helper method for update
        /// itterates through all the projectiels to see if they collide with wall or tank and do damage
        /// </summary>
        /// <param name="data_to_remove">The data that will be deleted from the world</param>
        /// <param name="data_to_send">The data that will be sent to clients</param
        private void UpdateProjectileHelper(ref ArrayList data_to_remove, ref StringBuilder data_to_send)
        {
            foreach (Projectiles p in TheWorld.ProjectileList.Values)
            {
                //projectile speed
                double projSpeed = 25;

                //update movement
                //get the vector
                Vector2D angleV = p.Dir;

                //normalize and get angle in radians
                angleV.Normalize();
                double angle = (angleV.ToAngle() * Math.PI / 180.0) - Math.PI / 2;
                //get the x and y components of the speed
                double xSpeed = projSpeed * Math.Cos(angle);
                double ySpeed = projSpeed * Math.Sin(angle);

                //add speed to current vector
                p.Loc = new Vector2D(p.Loc.GetX() + xSpeed, p.Loc.GetY() + ySpeed);

                //check if its outside the world boundries
                if (Math.Abs(p.Loc.GetX()) > TheWorld.GetGrid() / 2 || Math.Abs(p.Loc.GetY()) > TheWorld.GetGrid() / 2)
                {
                    p.Died = true;
                }

                //Check if it collides with wall
                foreach (Walls w in TheWorld.WallList.Values)
                {
                    if (TheWorld.IsProjectileCollide(p, w))
                    {
                        p.Died = true;
                        break;
                    }
                }

                //check if it collides with tank
                foreach (Tanks t in TheWorld.TankList.Values)
                {
                    if (t.Tank == p.Owner || t.Hp == 0 || t.Dc)  // tank is died or owner of projectile or disconnected
                        continue;
                    else if (TheWorld.IsProjectileCollide(p, t))
                    {
                        //if the projectile is already dead dont do anything
                        //if (p.Died)
                        //    break;
                        //projectile dies
                        p.Died = true;
                        //tank subtraction
                        t.Hp--;
                        //add shot hit taken for acuracy
                        TheWorld.TankList[p.Owner].ShotHit();
                        //kill tank if died
                        if (t.Hp == 0)
                        {
                            t.Died = true;
                            data_to_remove.Add(t);
                            TheWorld.TankList[p.Owner].Score++;
                        }
                        break;
                    }
                }

                if (p.Died)
                    data_to_remove.Add(p);
                data_to_send.Append(JsonConvert.SerializeObject(p) + "\n");
            }
        }

        /// <summary>
        /// helper method for update
        /// itterates through all the powerups and see if a tank has collected them
        /// </summary>
        /// <param name="data_to_remove">The data that will be deleted from the world</param>
        /// <param name="data_to_send">The data that will be sent to clients</param
        private void UpdatePowerUpHelper(ref ArrayList data_to_remove, ref StringBuilder data_to_send)
        {
            foreach (Powerups p in TheWorld.PowerList.Values)
            {
                foreach (Tanks t in TheWorld.TankList.Values)
                {
                    if (TheWorld.IsPowerUpCollide(t, p))
                    {
                        p.Died = true;
                        t.Powerup = true;
                    }
                }


                if (p.Died)
                    data_to_remove.Add(p);
                data_to_send.Append(JsonConvert.SerializeObject(p) + "\n");
            }
        }

        /// <summary>
        /// helper method for update
        /// itterates through all the beams and see if a tank has been hit and set hp to 0
        /// </summary>
        /// <param name="data_to_remove">The data that will be deleted from the world</param>
        /// <param name="data_to_send">The data that will be sent to clients</param
        private void UpdateBeamHelper(ref ArrayList data_to_remove, ref StringBuilder data_to_send)
        {
            foreach (Beams b in TheWorld.BeamList.Values)
            {
                foreach (Tanks t in TheWorld.TankList.Values)
                {
                    //skip the tank that shot the beam
                    if (t.Tank == b.Owner || t.Dc || t.Hp == 0)
                        continue;

                    //30 is half the tank size
                    //check if beam intersects
                    if (Intersects(b.Org, b.Dir, t.Loc, 30))
                    {
                        //kill the tank and update sore of other tank
                        t.Died = true;
                        t.Hp = 0;
                        data_to_remove.Add(t);
                        TheWorld.TankList[b.Owner].Score++;
                    }
                }

                data_to_send.Append(JsonConvert.SerializeObject(b) + "\n");
                data_to_remove.Add(b);
            }
        }

        /// <summary>
        /// helper method for update
        /// itterates through all the dead items and adds the data to be sent as "died" =true
        /// </summary>
        /// <param name="data_to_remove">The data that will be deleted from the world</param>
        /// <param name="data_to_send">The data that will be sent to clients</param
        private void UpdateRemoveHelper(ref ArrayList data_to_remove, ref StringBuilder data_to_send)
        {
            foreach (object o in data_to_remove)  // remove data
            {
                if (o is Tanks)  // tank is dead here
                {
                    Tanks t = o as Tanks;
                    data_to_send.Append(JsonConvert.SerializeObject(t) + "\n"); // add the tank to the string to send 

                    if (t.Dc)    // do not remove tank if not disconnect, wait until it respawn
                    {
                        TheWorld.TankList.Remove(t.Tank);
                        continue; // skip the rest of the steps
                    }

                    t.DecrementRespawnTime(); // starting to count down on the time of death
                    t.Died = false; // set the Died to be false

                    if (t.GetRespawnTime() <= 0)  // tank can respawn now
                    {
                        t = new Tanks(FindSpawnPoint(t));
                        lock (data_awaiting_updates)
                            data_awaiting_updates.Add(t);  // add to the data awaiting updates
                    }
                }
                else if (o is Projectiles)
                {
                    TheWorld.ProjectileList.Remove((o as Projectiles).Proj);
                }
                else if (o is Powerups)
                {
                    TheWorld.PowerList.Remove((o as Powerups).Power);
                }
                else if (o is Beams)
                {
                    TheWorld.BeamList.Remove((o as Beams).Beam);
                }
            }

            data_to_remove.Clear();
        }

        /// <summary>
        /// helper method for update
        /// sends the data to all the clients in client list
        /// </summary>
        /// <param name="data_to_send">The data that will be sent to clients</param
        private void UpdateSendHelper(ref StringBuilder data_to_send)
        {
            lock (clients)
            {
                foreach (SocketState ss in clients.Values)
                {
                    Networking.Send(ss.TheSocket, data_to_send.ToString());
                    lock (ss)
                    {
                        Networking.GetData(ss);
                    }
                }
            }
        }

        /// <summary>
        /// when a new client is connected
        /// </summary>
        /// <param name="state">The Socket State</param>
        private void NewClientConnected(SocketState state)
        {
            lock (state)
            {
                if (state.ErrorOccured)
                {
                    int disconnected_id = (int)state.ID;
                    HandleDisconnection(disconnected_id);
                    return;
                }

                // Save the client state
                // Need to lock here because clients can disconnect at any time
                lock (clients)
                {
                    clients[state.ID] = state;
                }

                state.OnNetworkAction = Handshake;
                Console.WriteLine("a new client is connected, id: " + state.ID);

                Networking.GetData(state);

            }

        }

        /// <summary>
        /// Handling the first communication between the client and the server 
        /// after accepting the client
        /// </summary>
        /// <param name="state">The Socket State</param>
        private void Handshake(SocketState state)
        {
            lock (state)
            {
                if (state.ErrorOccured)
                {
                    int disconnected_id = (int)state.ID;
                    HandleDisconnection(disconnected_id);
                    return;
                }

                string player_name = state.GetData();


                state.ClearData();

                player_name = player_name.Remove(player_name.Length - 1);

                if (player_name.Length > 16)
                {
                    player_name = "long name";
                }

                Tanks t = new Tanks((int)state.ID, player_name, new Vector2D(0, 0), new Vector2D(0, 1),
                    new Vector2D(0, 1), 0, 3, false, false, false);
                t = FindSpawnPoint(t);

                lock (TheWorld)
                {
                    t.SetStandardCoolDown(TheWorld.GetFramePerShot()); // set the standard cool down/reload time for ammo
                    t.SetStandardRespawnTime(TheWorld.GetRespawnRate()); // set the standard respawning rate 
                }

                lock (data_awaiting_updates)
                {
                    data_awaiting_updates.Add(t);  // creates the tank and add tank to the list
                }


                string data_to_send = t.Tank + "\n" + TheWorld.GetGrid() + "\n";  // send tank id and world grid

                lock (TheWorld)
                {
                    foreach (Walls w in TheWorld.WallList.Values)
                        data_to_send += JsonConvert.SerializeObject(w) + "\n";  // send datas about the world
                }
                Networking.Send(state.TheSocket, data_to_send);

          

                //send to clients
                lock (clients)
                {
                    clients[state.ID] = state;  // add socketstate to the clients
                }
                state.OnNetworkAction = ReceiveMessage;
                Networking.GetData(state);
            }

        }

        /// <summary>
        /// Handles the disconnection
        /// </summary>
        /// <param name="disconnected_id"></param>
        private void HandleDisconnection(int disconnected_id)
        {
            lock (clients)
            {
                clients.Remove((long)disconnected_id); // remove socketstate
            }

            Tanks t;
            lock (TheWorld)
            {
                if (TheWorld.TankList.ContainsKey(disconnected_id))
                {
                    t = TheWorld.TankList[disconnected_id];
                    if (!t.Dc)
                    {
                        Console.WriteLine("Client " + disconnected_id + " has disconnected ");
                        t.Dc = true;
                        lock (data_awaiting_updates)
                        {
                            data_awaiting_updates.Add(t); // add to the update list
                        }
                    }

                }
            }


        }

        /// <summary>
        /// Call backs for receiving messages from the client
        /// </summary>
        /// <param name="state">The Socket State</param>
        private void ReceiveMessage(SocketState state)
        {
            if (state.ErrorOccured)
            {
                int disconnected_id = (int)state.ID;
                HandleDisconnection(disconnected_id);
                return;
            }

            ProcessMessage(state);
        }

        /// <summary>
        /// parse the data, create an updated object if needed, add to the data updating list
        /// </summary>
        /// <param name="state">The socket State</param>
        private void ProcessMessage(SocketState state)
        {
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");  // split by "\n", 
            int parts_length = parts.Length;
            state.ClearData();  // empty out the server

            // Loop until have processed all messages.
            foreach (string p in parts)  // p represents one json object
            {
                if (p.Length == 0) // ignore empty strings
                    continue;
                if (p[p.Length - 1] != '\n')  // last string to process
                    break;

                ControlCommands c = JsonConvert.DeserializeObject<ControlCommands>(p);
                Tanks tank_tobe_updated;
                lock (TheWorld)
                {
                    tank_tobe_updated = new Tanks(TheWorld.TankList[(int)state.ID]);
                }

                if (tank_tobe_updated.Hp != 0 && !tank_tobe_updated.Dc)  // if tank's disconnected or dead, then all tanks updates are false
                    UpdateTank(tank_tobe_updated, c);
            }
        }

        /// <summary>
        /// updates tank location and sees if tank has fired a shto
        /// </summary>
        /// <param name="tank_tobe_updated">tank to update</param>
        /// <param name="c">control command</param>
        private void UpdateTank(Tanks tank_tobe_updated, ControlCommands c)
        {
            object send = null;
            //check if c is null
            if (c == null)
                return;

            //always update turret position
            tank_tobe_updated.Tdir = c.Tdir;

            switch (c.Fire)
            {
                case "none":
                    break;
                case "alt":
                    if (tank_tobe_updated.Powerup)
                    {
                        send = new Beams(projectileId++, tank_tobe_updated.Loc, c.Tdir, tank_tobe_updated.Tank);
                        tank_tobe_updated.Powerup = false;
                    }

                    break;
                case "main":
                    //create new projectile and add it to be updated if the tank can shoot
                    if (tank_tobe_updated.GetCoolDown() <= 0)
                    {
                        tank_tobe_updated.ResetCoolDown();
                        send = new Projectiles(projectileId++, tank_tobe_updated.Loc, c.Tdir, false, tank_tobe_updated.Tank);
                        tank_tobe_updated.ShotTaken();
                    }
                    break;

            }

            float speed = 2.9f;
            Vector2D prev_tank_location = new Vector2D(tank_tobe_updated.Loc);
            switch (c.Moving)
            {
                case "none":
                    break;
                case "left":
                    tank_tobe_updated.Bdir = new Vector2D(-1, 0);
                    tank_tobe_updated.Loc = new Vector2D(tank_tobe_updated.Loc.GetX() - speed, tank_tobe_updated.Loc.GetY());
                    break;
                case "right":
                    tank_tobe_updated.Bdir = new Vector2D(1, 0);
                    tank_tobe_updated.Loc = new Vector2D(tank_tobe_updated.Loc.GetX() + speed, tank_tobe_updated.Loc.GetY());
                    break;
                case "up":
                    tank_tobe_updated.Bdir = new Vector2D(0, -1);
                    tank_tobe_updated.Loc = new Vector2D(tank_tobe_updated.Loc.GetX(), tank_tobe_updated.Loc.GetY() - speed);
                    break;
                case "down":
                    tank_tobe_updated.Bdir = new Vector2D(0, 1);
                    tank_tobe_updated.Loc = new Vector2D(tank_tobe_updated.Loc.GetX(), tank_tobe_updated.Loc.GetY() + speed);
                    break;
            }

            foreach (Walls w in TheWorld.WallList.Values)
            {
                if (TheWorld.IsWallCollide(w, tank_tobe_updated))
                {
                    tank_tobe_updated.Loc = prev_tank_location;  // if the update causes collision, then reverse the update
                    break;
                }
            }

            lock (data_awaiting_updates)
            {
                data_awaiting_updates.Add(tank_tobe_updated);
                if (send != null)
                    data_awaiting_updates.Add(send);
            }
        }

        /// <summary>
        /// Determines if a ray interescts a circle
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        public static bool Intersects(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substitute to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);
            return (root1 > 0.0 && root2 > 0.0);
        }

        /// <summary>
        /// Find the respawn point and revive the tank, 
        /// if the respawn point will cause collision right the way, 
        /// will respawn another point
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private Tanks FindSpawnPoint(Tanks t)
        {
            Random random = new Random();
            Tanks new_tank = new Tanks(t); // make a copy of the old tank

            new_tank.ResetRespawnTime();  // set the respawn time back to standard
            new_tank.Died = false;
            new_tank.Hp = 3;
            new_tank.Join = true;
            new_tank.Powerup = false;

            lock (TheWorld)
            {
                bool stay = true;
                while (stay)
                {
                    bool didcollide = false;
                    //create a random location
                    int x = random.Next(TheWorld.GetGrid()) - (TheWorld.GetGrid() / 2);
                    int y = random.Next(TheWorld.GetGrid()) - (TheWorld.GetGrid() / 2);
                    Vector2D loc = new Vector2D(x, y);
                    new_tank.Loc = loc;

                    //check for collision
                    foreach (Walls w in TheWorld.WallList.Values)
                    {
                        if (TheWorld.IsWallCollide(w, new_tank))
                            didcollide = true;
                    }
                    foreach (Projectiles p in TheWorld.ProjectileList.Values)
                    {
                        if (TheWorld.IsProjectileCollide(p, new_tank))
                            didcollide = true;
                    }
                    if (didcollide)
                        continue;
                    else
                        stay = false;
                }
            }

            return new_tank;
        }

        /// <summary>
        /// creates a new projectile spawned in a random spot in the world
        /// </summary>
        /// <returns>the power up to spawn</returns>
        private Powerups SpawnPowerUps()
        {
            Random random = new Random();
            Vector2D tempV = new Vector2D(0, 0);
            Powerups p = new Powerups(powId++, tempV, false);

            lock (TheWorld)
            {
                bool stay = true;
                while (stay)
                {
                    bool didcollide = false;
                    //create a random location
                    int x = random.Next(TheWorld.GetGrid()) - (TheWorld.GetGrid() / 2);
                    int y = random.Next(TheWorld.GetGrid()) - (TheWorld.GetGrid() / 2);
                    Vector2D loc = new Vector2D(x, y);
                    p.Loc = loc;

                    //check for collision
                    foreach (Walls w in TheWorld.WallList.Values)
                    {
                        if (TheWorld.IsWallCollide(w, p))
                            didcollide = true;
                    }

                    foreach (Tanks t in TheWorld.TankList.Values)
                    {
                        if (TheWorld.IsPowerUpCollide(t, p))
                            didcollide = true;
                    }
                    if (didcollide)
                        continue;
                    else
                        stay = false;
                }
            }
            return p;
        }
    }
}
