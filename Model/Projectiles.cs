using ClassLibrary;
using Newtonsoft.Json;

namespace Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Projectiles
    {
        [JsonProperty]
        private int proj;
        /// <summary>
        /// an int representing the projectile's unique ID.
        /// </summary>
        public int Proj { get { return proj; } set { proj = value; } }

        [JsonProperty]
        private Vector2D loc; 
        /// <summary>
        /// a Vector2D representing the projectile's location
        /// </summary>
        public Vector2D Loc { get { return loc; } set { loc = value; } }

        [JsonProperty]
        private Vector2D dir;
        /// <summary>
        /// a Vector2D representing the projectile's orientation
        /// </summary>
        public Vector2D Dir { get { return dir; } set { dir = value; } }

        [JsonProperty]
        private bool died;
        /// <summary>
        /// a bool representing if the projectile died on this frame
        /// </summary>
        public bool Died { get { return died; } set { died = value; } }

        [JsonProperty]
        private int owner;
        /// <summary>
        /// an int representing the ID of the tank that created the projectile
        /// </summary>
        public int Owner { get { return owner; } set { owner = value; } }

        [JsonConstructor]
        public Projectiles(int proj, Vector2D loc, Vector2D dir, bool died, int owner)
        {
            this.proj = proj;
            this.loc = loc;
            this.dir = dir;
            this.died = died;
            this.owner = owner;
        }

        Projectiles()
        { 
        
        }
    }
}
