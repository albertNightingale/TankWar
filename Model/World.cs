//Code written by Albert liu and Alex Hudson
//Last updated 12/5/2019
//Skeleton Code provided by professor Kopta

using System;
using System.Collections.Generic;

namespace Model
{
    public class World
    {
        public Dictionary<int, Tanks> TankList { get; set; }
        public Dictionary<int, Powerups> PowerList { get; set; }
        public Dictionary<int, Walls> WallList { get; set; }
        public Dictionary<int, Projectiles> ProjectileList { get; set; }
        public Dictionary<int, Beams> BeamList { get; set; }
        private int GridSize;
        private int framerate;
        private int framepershot;
        private int respawnrate;
        private Tanks myTank;

        /// <summary>
        /// constructor with world size
        /// </summary>
        /// <param name="grid"></param>
        public World(int grid)
        {
            GridSize = grid;
            TankList = new Dictionary<int, Tanks>();
            PowerList = new Dictionary<int, Powerups>();
            WallList = new Dictionary<int, Walls>();
            ProjectileList = new Dictionary<int, Projectiles>();
            BeamList = new Dictionary<int, Beams>();
        }

        /// <summary>
        /// default constructor
        /// </summary>
        public World()
        {
            GridSize = 0;
            TankList = new Dictionary<int, Tanks>();
            PowerList = new Dictionary<int, Powerups>();
            WallList = new Dictionary<int, Walls>();
            ProjectileList = new Dictionary<int, Projectiles>();
            BeamList = new Dictionary<int, Beams>();
        }

        /// <summary>
        /// returns world size
        /// </summary>
        /// <returns></returns>
        public int GetGrid()
        {
            return GridSize;
        }
        /// <summary>
        /// set world size
        /// </summary>
        /// <param name="new_grid"></param>
        public void SetGrid(int new_grid)
        {
            GridSize = new_grid;
        }

        /// <summary>
        /// get frame rate
        /// </summary>
        /// <returns></returns>
        public int GetFrame()
        {
            return framerate;
        }

        /// <summary>
        /// set frame rate
        /// </summary>
        /// <param name="f"></param>
        public void SetFrame(int f)
        {
            framerate = f;
        }

        /// <summary>
        /// get frames per shot (cooldown)
        /// </summary>
        /// <returns></returns>
        public int GetFramePerShot()
        {
            return framepershot;
        }

        /// <summary>
        /// set frames per shot (cool down)
        /// </summary>
        /// <param name="sps"></param>
        public void SetFramePerShot(int sps)
        {
            framepershot = sps;
        }

        /// <summary>
        /// get respawn rate
        /// </summary>
        /// <returns></returns>
        public int GetRespawnRate()
        {
            return respawnrate;
        }

        /// <summary>
        /// set respawn rate
        /// </summary>
        /// <param name="rr"></param>
        public void SetRespawnRate(int rr)
        {
            respawnrate = rr;
        }

        /// <summary>
        /// get tank
        /// </summary>
        /// <returns></returns>
        public Tanks GetMyTank()
        {
            return myTank;
        }
        /// <summary>
        /// set tank
        /// </summary>
        /// <param name="t"></param>
        public void SetMyTank(Tanks t)
        {
            myTank = t;
        }
        /// <summary>
        /// checks if a projectile collides tih a wall
        /// </summary>
        /// <param name="p">the projectile</param>
        /// <param name="w">the wall</param>
        /// <returns></returns>
        public bool IsProjectileCollide(Projectiles p, Walls w)
        {
            int wallSize = 50;
            int projSize = 0;
 

            double wxmin = Math.Min(w.P1.GetX(), w.P2.GetX()) - wallSize / 2 - projSize / 2;
            double wxmax = Math.Max(w.P1.GetX(), w.P2.GetX()) + wallSize / 2 + projSize / 2;
            double wymin = Math.Min(w.P1.GetY(), w.P2.GetY()) - wallSize / 2 - projSize / 2;
            double wymax = Math.Max(w.P1.GetY(), w.P2.GetY()) + wallSize / 2 + projSize / 2;

            if (wxmin <= p.Loc.GetX() && p.Loc.GetX() <= wxmax)
                if (wymin <= p.Loc.GetY() && p.Loc.GetY() <= wymax)
                    return true;
            return false;
        }

        public bool IsProjectileCollide(Projectiles p, Tanks t)
        {
            int tankSize = 60;
            int projSize = 0;


            double wxmin = t.Loc.GetX() - tankSize / 2 - projSize / 2;
            double wxmax = t.Loc.GetX() + tankSize / 2 + projSize / 2;
            double wymin = t.Loc.GetY() - tankSize / 2 - projSize / 2;
            double wymax = t.Loc.GetY() + tankSize / 2 + projSize / 2;

            if (wxmin <= p.Loc.GetX() && p.Loc.GetX() <= wxmax)
                if (wymin <= p.Loc.GetY() && p.Loc.GetY() <= wymax)
                    return true;
            return false;
        }
        /// <summary>
        /// Returns to if the tank collides with a wall;
        /// </summary>
        /// <param name="w">The wall</param>
        /// <param name="t">The Tank</param>
        /// <returns>True if collision is ditected</returns>
        public bool IsWallCollide(Walls w, Tanks t)
        {
            int awallSize = 50;  // the size of one single wall sprite
            int tankSize = 60;   // the size of one tank

            double wxmin = Math.Min(w.P1.GetX(), w.P2.GetX()) - awallSize / 2 - tankSize / 2;
            double wxmax = Math.Max(w.P1.GetX(), w.P2.GetX()) + awallSize / 2 + tankSize / 2;
            double wymin = Math.Min(w.P1.GetY(), w.P2.GetY()) - awallSize / 2 - tankSize / 2;
            double wymax = Math.Max(w.P1.GetY(), w.P2.GetY()) + awallSize / 2 + tankSize / 2;

            if (wxmin <= t.Loc.GetX() && t.Loc.GetX() <= wxmax)
                if (wymin <= t.Loc.GetY() && t.Loc.GetY() <= wymax)
                    return true;
            return false; 
        }

        /// <summary>
        /// Returns to if the powerup collides with a wall.
        /// </summary>
        /// <param name="w">The wall</param>
        /// <param name="p">The Powerup</param>
        /// <returns>True if collision is ditected</returns>
        public bool IsWallCollide(Walls w, Powerups p)
        {
            int awallSize = 50;  // the size of one single wall sprite
            int tankSize = 60;   // the size of one tank

            double wxmin = Math.Min(w.P1.GetX(), w.P2.GetX()) - awallSize / 2 - tankSize / 2;
            double wxmax = Math.Max(w.P1.GetX(), w.P2.GetX()) + awallSize / 2 + tankSize / 2;
            double wymin = Math.Min(w.P1.GetY(), w.P2.GetY()) - awallSize / 2 - tankSize / 2;
            double wymax = Math.Max(w.P1.GetY(), w.P2.GetY()) + awallSize / 2 + tankSize / 2;

            if (wxmin <= p.Loc.GetX() && p.Loc.GetX() <= wxmax)
                if (wymin <= p.Loc.GetY() && p.Loc.GetY() <= wymax)
                    return true;
            return false;
        }

        /// <summary>
        /// check if a power up collides with a tank
        /// </summary>
        /// <param name="t">the tank</param>
        /// <param name="p">the powerup</param>
        /// <returns></returns>
        public bool IsPowerUpCollide(Tanks t, Powerups p)
        {
            int tankSize = 60;
            int powSize = 0;


            double wxmin = t.Loc.GetX() - tankSize / 2 - powSize / 2;
            double wxmax = t.Loc.GetX() + tankSize / 2 + powSize / 2;
            double wymin = t.Loc.GetY() - tankSize / 2 - powSize / 2;
            double wymax = t.Loc.GetY() + tankSize / 2 + powSize / 2;

            if (wxmin <= p.Loc.GetX() && p.Loc.GetX() <= wxmax)
                if (wymin <= p.Loc.GetY() && p.Loc.GetY() <= wymax)
                    return true;
            return false;
        }

        /// <summary>
        /// Helper to find the top left of every wall
        /// </summary>
        /// <param name="w">The Wall</param>
        /// <param name="x">The X cord</param>
        /// <param name="y">The Y cord</param>
        private void GetTopLeftWall(Walls w, out double x, out double y)
        {
            y = 0;
            x = 0;
            if (w.P1.GetX() == w.P2.GetX() && w.P1.GetY() > w.P2.GetY())
            {
                //wall is vertical with p1 y on top
                x = w.P1.GetX();
                y = w.P1.GetY();

            }
            else if (w.P1.GetX() == w.P2.GetX() && w.P1.GetY() <= w.P2.GetY())
            {
                //wall is vertical with p2 y on top
                x = w.P1.GetX();
                y = w.P2.GetY();
            }
            else if (w.P1.GetX() > w.P2.GetX() && w.P1.GetY() == w.P2.GetY())
            {
                //wall is vertical with p1 x on top
                x = w.P1.GetX();
                y = w.P2.GetY();
            }
            else if (w.P1.GetX() <= w.P2.GetX() && w.P1.GetY() == w.P2.GetY())
            {
                //wall is vertical with p2 x on top
                x = w.P2.GetX();
                y = w.P2.GetY();
            }
        }
    }
}
