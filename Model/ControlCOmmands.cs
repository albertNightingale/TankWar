using ClassLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ControlCommands
    {
        [JsonProperty]
        private string moving;
        /// <summary>
        /// string representing whether the player wants to move or not, and the desired direction none, up, left, down, right,
        /// </summary>
        public string Moving { get { return moving; } set { moving = value; } }

        [JsonProperty]
        private string fire;
        /// <summary>
        /// a string representing whether the player wants to fire or not, and the desired type none, main(projectile), alt(beam)
        /// </summary>
        public string Fire { get { return fire; } set { fire = value; } }

        [JsonProperty]
        private Vector2D tdir;
        /// <summary>
        /// a Vector2D representing where the player wants to aim their turret"x":1,"y":0
        /// </summary>
        public Vector2D Tdir { get { return tdir; } set { tdir = value; } }


        [JsonConstructor]
        public ControlCommands(string moving, string fire, Vector2D tdir)
        {
            this.moving = moving;
            this.fire = fire;
            this.tdir = tdir;
        }

        ControlCommands()
        { 
        }
    }
}
