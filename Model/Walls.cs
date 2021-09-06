using ClassLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Walls
    {
        [JsonProperty]
        private int wall;
        /// <summary>
        /// an int representing the wall's unique ID
        /// </summary>
        public int Wall { get { return wall; } set { wall = value; } }

        [JsonProperty]
        private Vector2D p1;
        /// <summary>
        /// a Vector2D representing one endpoint of the wall
        /// </summary>
        public Vector2D P1 { get { return p1; } set { p1 = value; } }

        [JsonProperty]
        private Vector2D p2;
        /// <summary>
        /// a Vector2D representing the other endpoint of the wall
        /// </summary>
        public Vector2D P2 { get { return p2; } set { p2 = value; } }

        [JsonConstructor]
        public Walls(int wall, Vector2D p1, Vector2D p2)
        {
            this.wall = wall;
            this.p1 = p1;
            this.p2 = p2;
        }

        Walls()
        { 
        
        }
    }
}
