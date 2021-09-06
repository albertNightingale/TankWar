//Code written by Albert liu and Alex Hudson
//Last updated 12/5/2019
//Skeleton Code provided by professor Kopta

using ClassLibrary;
using Newtonsoft.Json;
using System;

namespace Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Tanks
    {
        [JsonProperty]
        private int tank;
        /// <summary>
        /// an int representing the tank's unique ID
        /// </summary>
        public int Tank { get { return tank; } set { tank = value; } }


        [JsonProperty]
        private string name;
        /// <summary>
        /// a string representing the player's name.
        /// </summary>
        public string Name { get { return name; } set { name = value; } }


        [JsonProperty]
        private Vector2D loc;
        /// <summary>
        /// a string representing the player's name.
        /// </summary>
        public Vector2D Loc { get { return loc; } set { loc = value; } }


        [JsonProperty]
        private Vector2D bdir;
        /// <summary>
        /// a Vector2D representing the tank's orientation. This will always be an axis-aligned vector 
        /// </summary>
        public Vector2D Bdir { get { return bdir; } set { bdir = value; } }


        [JsonProperty]
        private Vector2D tdir;
        /// <summary>
        /// a Vector2D representing the direction of the tank's turret (where it's aiming)
        /// </summary>
        public Vector2D Tdir { get { return tdir; } set { tdir = value; } }


        [JsonProperty]
        private int score;
        /// <summary>
        /// an int representing the player's score
        /// </summary>
        public int Score { get { return score; } set { score = value; } }


        [JsonProperty]
        private int hp;
        /// <summary>
        /// int representing the hit points of the tank. This value ranges from 0 - 3
        /// </summary>
        public int Hp { get { return hp; } set { hp = value; } }


        [JsonProperty]
        private bool died;
        /// <summary>
        /// a bool indicating if the tank died on that frame.
        /// </summary>
        public bool Died { get { return died; } set { died = value; } }


        [JsonProperty]
        private bool dc;
        /// <summary>
        /// a bool indicating if the player controlling that tank disconnected on that frame. 
        /// </summary>
        public bool Dc { get{return dc;} set{dc = value;} }


        [JsonProperty]
        private bool join;
        /// <summary>
        /// a bool indicating if the player joined on this frame.
        /// </summary>
        public bool Join { get { return join; } set { join = value; } }

        /// <summary>
        /// About the shooting cool down time
        /// </summary>
        private int cooldown;
        private int readonlycooldown;

        /// <summary>
        /// About the player respawning time
        /// </summary>
        private int respawntime;
        private int readonlyrespawntime;
        private int shotTaken;
        private int shotHit;

        [JsonConstructor]
        public Tanks(int tank, string name, Vector2D loc, Vector2D bdir, Vector2D tdir, 
            int score, int hp, bool died, bool dc, bool join)
        {
            this.tank = tank;
            this.name = name;
            this.loc = loc;
            this.bdir = bdir;
            this.tdir = tdir;
            this.score = score;
            this.hp = hp;
            this.died = died;
            this.dc = dc;
            this.join=join;
            cooldown = 0;
            Powerup = false;
            respawntime = 0;
            shotTaken = 0;
            shotHit = 0;
        }

        public Tanks()
        {

        }

        public Tanks(Tanks t)
        {
            if (t == null)
                throw new Exception(" tank is null. " );
            else
            {
                this.tank = t.Tank;
                this.name = t.Name;
                this.loc = new Vector2D(t.Loc);
                this.bdir = new Vector2D(t.Bdir);
                this.tdir = new Vector2D(t.Tdir);
                this.score = t.score;
                this.hp = t.hp;
                this.died = t.died;
                this.dc = t.dc;
                this.join = t.join;
                cooldown = t.cooldown;
                this.Powerup = t.Powerup;
                this.shotHit = t.shotHit;
                readonlycooldown = t.readonlycooldown;
                respawntime = t.respawntime;
                readonlyrespawntime = t.readonlyrespawntime;
                this.shotTaken = t.shotTaken;
                this.shotHit = t.shotHit;
            }
        }
        public void ShotTaken()
        {
            shotTaken++;
        }
        public void ShotHit()
        {
            shotHit++;
        }
        public int getAccuracy()
        {
            return (int)(1.0*shotTaken/shotHit * 100);
        }

        public bool Powerup
        {
            get; set;
        }

        public int GetCoolDown()
        {
            return cooldown;
        }

        public void SetStandardCoolDown(int cdt)
        {
            readonlycooldown = cdt; 
        }
        
        public void DecrementCoolDown()
        {
            cooldown--;
        }

        public void ResetCoolDown()
        {
            cooldown = readonlycooldown;
        }

        /// <summary>
        /// Get the current respawn time
        /// </summary>
        /// <returns></returns>
        public int GetRespawnTime()
        {
            return respawntime;
        }

        /// <summary>
        /// set the standard respawn time
        /// </summary>
        /// <param name="rt"></param>
        public void SetStandardRespawnTime(int rt)
        {
            readonlyrespawntime = rt;
            respawntime = rt; 
        }

        /// <summary>
        /// decrease the respawn time by one
        /// </summary>
        public void DecrementRespawnTime()
        {
            respawntime--;
        }

        /// <summary>
        /// reset the respawn time
        /// </summary>
        public void ResetRespawnTime()
        {
            respawntime = readonlyrespawntime;
        }
    }
}

