using System;
using NetworkUtil;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Model;
using ClassLibrary;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Control
{
    public class Controller
    {
        private SocketState theServer;
        private static World theWorld;

        private string player_name;
        private int player_id;

        public delegate void ServerUpdatesHandler();
        private ServerUpdatesHandler handler;
        private event ServerUpdatesHandler UpdatesArrived;

        // control commands variables
        private string moving = "none";
        private string fire = "none";
        private Vector2D tdir = null; 


        public static World GetWorld()
        {
            return theWorld;
        }

        public Controller(string player_name)
        {
            this.player_name = player_name;
            theWorld = new World();
        }

        /// <summary>
        /// A wrapper to connect to the server. 
        /// </summary>
        /// <param name="servername"></param>
        public void GetConnection(string servername)
        {
            Networking.ConnectToServer(OnConnect, servername, 11000);
        }

        /// <summary>
        /// Call back during the onConnect
        /// </summary>
        /// <param name="state"></param>
        private void OnConnect(SocketState state)
        {
            if (state.ErrorOccured)
            {
                MessageBox.Show("Unable to connect to the server! ");
            }
            
            // Save the SocketState so we can use it to send messages
            theServer = state;

            // send the player name to the server
            Networking.Send(theServer.TheSocket, player_name + "\n"); 

            // Start an event loop to receive messages from the server
            state.OnNetworkAction = GetSetUpDataFromServer;
            Networking.GetData(state);
        }

        /// <summary>
        /// Get the tank id and the size of the world from the server 
        /// as a confirmation that it can receive data from server
        /// </summary>
        /// <param name="state"></param>
        private void GetSetUpDataFromServer(SocketState state)
        {
            if (state.ErrorOccured)
                return;

            theServer = state;

            string data = theServer.GetData(); 
            string[] parts = Regex.Split(data, @"(?<=[\n])");  // may not able to split correctly
            int parts_length = parts.Length;

            // Console.WriteLine("Total amount of objects: " + parts_length);
            // get the tank id
            int.TryParse(parts[0], out player_id);

            // get the grid
            int.TryParse(parts[1], out int grid);
            // set the grid to the world
            theWorld.SetGrid(grid);

            
            for (int i = 2; i < parts_length; i++)
            {
                if (parts[i].Length == 0) // ignore empty strings
                    continue;

                if (parts[i][parts[i].Length - 1] != '\n')  // last string to process
                    break;

                FromJson(parts[i]); // converting the string to C# object
            }

            theServer.ClearData();  

            if (handler != null)
                UpdatesArrived += handler; // if handler is assigned, then add it to the event

            state.OnNetworkAction = ReceiveMessage;

            lock (theServer)
            {
                Networking.GetData(theServer); // Continue the event loop
            }
        }

        /// <summary>
        /// Delegate call back for receive message
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveMessage(SocketState state)
        {
            if (state.ErrorOccured)
            {
                return;
            }

            theServer = state; // Save the SocketState so access the current data for processing

            ProcessMessages(); // process message

            lock (theServer)
            {
                Networking.GetData(theServer); // Continue the event loop
            }
        }

        public void SetHandler(ServerUpdatesHandler h)
        {
            handler = h;
        }

        /// <summary>
        /// Process messages
        /// </summary>
        private void ProcessMessages()
        {
            string totalData = theServer.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");  // split by "\n", 
            int parts_length = parts.Length;
            // Console.WriteLine("Total amount of objects in this data transportation: " + parts_length);

            // Loop until have processed all messages.
            foreach (string p in parts)  // p represents one json object
            {
                if (p.Length == 0) // ignore empty strings
                    continue;
                if (p[p.Length - 1] != '\n')  // last string to process
                    break;

                FromJson(p);  // converting the string to C# object
            }

            theServer.ClearData();  // empty out the server

            // activate the event
            lock (this)  // lock to prevent controller updates when sending it
            {
                SendCtrls();   // send the control out 
                UpdatesArrived?.Invoke();
            }
        }

        /// <summary>
        /// Build ControlCommands Object based on the input information from the parameter
        /// </summary>
        /// <param name="mouse"> if 0, nothing; 1048576, leftclick; 2097152, rightclick</param>
        /// <param name="key">if 0, nothing; up(w) 87; down(s) 83; left(a) 65; right(d) 68</param>
        /// <param name="mouseloc">the location of the mouse</param>
        public void HandleControlCommands(int mouse, int key, Vector2D mouseloc)
        {
            if (mouseloc == null)
                return;
            
            Vector2D gamerlocation = new Vector2D(400, 400); 

            switch (key)
            {
                case 87:
                    moving = "up";
                    break;
                case 83:
                    moving = "down";
                    break;
                case 65:
                    moving = "left";
                    break;
                case 68:
                    moving = "right";
                    break;
                case 0: // set moving back to none
                    moving = "none";
                    break;
                default:
                    break;
            }

            switch (mouse)
            {
                case 1048576:
                    fire = "main";   
                    break;
                case 2097152:
                    fire = "alt";
                    break;
                case 0:
                    fire = "none";
                    break;
                default:  // ignore all other mouses
                    break;
            }

            // calculating the unit vector
            tdir = mouseloc - gamerlocation;
            tdir.Normalize();   // normalized the vector
        }

        /// <summary>
        /// send out the control by traversing through the control list
        /// </summary>
        private void SendCtrls() 
        {
            /*
            if (tdir == null)
                return;  // do not send if turret direction is null
            */ 

            Networking.Send(theServer.TheSocket, JsonConvert.SerializeObject(new ControlCommands(moving, fire, tdir)) + "\n");

            lock (theServer)
            {
                Networking.GetData(theServer);
            }
        }

        /// <summary>
        /// Determine the type the object to be converted to
        /// Adding the appropriate object to the World. 
        /// </summary>
        /// <param name="json_data"> The data </param>
        private void FromJson(string json_data)
        {
            lock (theWorld)
            {
                object model_object;
                if (json_data.Contains("wall") && json_data.Contains("p1") && json_data.Contains("p2"))  // determine if is wall 
                {
                    model_object = JsonConvert.DeserializeObject<Walls>(json_data);
                    Walls w = (Walls)model_object;
                    
                    if (theWorld.WallList.ContainsKey(w.Wall))  // old wall
                        theWorld.WallList[w.Wall] = w; 
                    else
                        theWorld.WallList.Add(w.Wall, w);
                }
                else if (json_data.Contains("tank") && json_data.Contains("loc") && json_data.Contains("name"))  // determine if is tank 
                {
                    model_object = JsonConvert.DeserializeObject<Tanks>(json_data);
                    Tanks t = (Tanks)model_object;
                    if (theWorld.TankList.ContainsKey(t.Tank))  // old tank
                        theWorld.TankList[t.Tank] = t;
                    else
                        theWorld.TankList.Add(t.Tank, t);

                    if (t.Tank == player_id)
                    {
                        theWorld.SetMyTank(t);
                    }
                }
                else if (json_data.Contains("proj") && json_data.Contains("dir") && json_data.Contains("owner"))  // determine if is projectile object
                {
                    model_object = JsonConvert.DeserializeObject<Projectiles>(json_data);
                    Projectiles p = (Projectiles)model_object;

                    if (theWorld.ProjectileList.ContainsKey(p.Proj))  // update the projectile
                        theWorld.ProjectileList[p.Proj] = p;
                    else
                        theWorld.ProjectileList.Add(p.Proj, p);
                }
                else if (json_data.Contains("beam") && json_data.Contains("org") && json_data.Contains("dir"))  // determine if is beam
                {
                    model_object = JsonConvert.DeserializeObject<Beams>(json_data);
                    Beams b = (Beams)model_object;
                    theWorld.BeamList.Add(b.Beam, b);  // add the beam  
                }
                else if (json_data.Contains("power") && json_data.Contains("loc") && json_data.Contains("died"))  // determine if is powerups
                {
                    model_object = JsonConvert.DeserializeObject<Powerups>(json_data);
                    Powerups p = (Powerups)model_object;
                    if (theWorld.PowerList.ContainsKey(p.Power)) // update the power
                        theWorld.PowerList[p.Power] = p;
                    else
                        theWorld.PowerList.Add(p.Power, p);
                }
            }
        }

    }
}

