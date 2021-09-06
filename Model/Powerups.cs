using ClassLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Powerups
    {
        [JsonProperty]
        private int power;
        /// <summary>
        /// an int representing the powerup's unique ID
        /// </summary>
        public int Power { get { return power; } set { power = value; } }


        [JsonProperty]
        private Vector2D loc;
        /// <summary>
        /// a Vector2D representing the location of the powerup
        /// </summary>
        public Vector2D Loc { get { return loc; } set { loc = value; } }


        [JsonProperty]
        private bool died;
        /// <summary>
        /// a bool indicating if the powerup "died" (was collected by a player) on this frame
        /// </summary>
        public bool Died { get { return died; } set { died = value; } }

        [JsonConstructor]
        public Powerups(int power, Vector2D loc, bool died)
        {
            this.power = power; 
            this.loc = loc;
            this.died = died;
        }

        Powerups()
        { 
        }


    }
}
