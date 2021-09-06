using ClassLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Beams
    {
        [JsonProperty]
        private int beam; 
        /// <summary>
        /// an int representing the beam's unique ID
        /// </summary>
        public int Beam { get { return beam; } set { beam = value; } }

        [JsonProperty]
        private Vector2D org;
        /// <summary>
        /// a Vector2D representing the origin of the beam
        /// </summary>
        public Vector2D Org { get { return org; } set { org = value; } }

        [JsonProperty]
        private Vector2D dir;
        /// <summary>
        /// a Vector2D representing the direction of the beam
        /// </summary>
        public Vector2D Dir { get { return dir; } set { dir = value; } }

        [JsonProperty]
        private int owner; 
        /// <summary>
        /// an int representing the ID of the tank that fired the beam
        /// </summary>
        public int Owner { get { return owner; } set { owner = value; } }

        private bool isDrawn;
        /// <summary>
        /// a boolean variable that determines if the beam is drawn or not
        /// </summary>
        public bool IsDrawn { get { return isDrawn; } set { isDrawn = value; } }

        private int drawn_times;
        /// <summary>
        /// a int variable that will keep track of the number of times the object has been drawn. 
        /// </summary>
        public int Drawn_times { get { return drawn_times; } set { drawn_times = value; } }

        [JsonConstructor]
        public Beams(int beam, Vector2D org, Vector2D dir, int owner)
        {
            this.beam = beam;
            this.org = org;
            this.dir = dir;
            this.owner = owner;
            isDrawn = false; 
        }

        Beams()
        { 
            
        }


    }
}
