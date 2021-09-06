
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Control;
using System.Drawing;
using ClassLibrary;

namespace View
{
    public class GamingPanel : Panel
    {
        private Controller controller;

        public GamingPanel(Controller c) : base()
        {
            DoubleBuffered = true;
            KeyDown += this.OnKeyDown;
            KeyUp += this.OnKeyUp;
            MouseClick += this.OnMouseClick;
            MouseMove += this.OnMouseMove;
            controller = c;
            
        }

        //////////////////// EVENT HANDLER /////////////////

        /// <summary>
        /// Handle the event of mouse click
        /// </summary>
        private void OnMouseClick(Object sender, MouseEventArgs e)
        {
            controller.HandleControlCommands((int)e.Button, 0, new Vector2D(MousePosition.X, MousePosition.Y));
        }

        /// <summary>
        /// Handle the event of mouse movement
        /// </summary>
        private void OnMouseMove(Object sender, MouseEventArgs e)
        {
            // no mouse movements nor keyboard movements
            controller.HandleControlCommands(0, 0, new Vector2D(MousePosition.X, MousePosition.Y));
        }

        /// <summary>
        /// Handle the event of key pressed down
        /// </summary>
        private void OnKeyDown(Object sender, KeyEventArgs e)
        {
            controller.HandleControlCommands(0, e.KeyValue, new Vector2D(MousePosition.X , MousePosition.Y));
        }

        /// <summary>
        /// Handle the event of key being released
        /// set the control command to none
        /// </summary>
        private void OnKeyUp(Object sender, KeyEventArgs e)
        {
            controller.HandleControlCommands(0, 0, new Vector2D(MousePosition.X , MousePosition.Y));
        }

        ///////////////////////// DRAWING ///////////////////////////    

        /// <summary>
        /// Helper method for DrawObjectWithTransform
        /// </summary>
        /// <param name="size">The world (and image) size</param>
        /// <param name="w">The worldspace coordinate</param>
        /// <returns></returns>
        private static int WorldSpaceToImageSpace(int size, double w)
        {
            return (int)w + size / 2;  // off set the size
        }

        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e  
        public delegate void ObjectDrawer(object o, PaintEventArgs e);


        /// <summary>
        /// This method performs a translation and rotation to drawn an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldSize">The size of one edge of the world (assuming the world is square)</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, int worldSize, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            int x = WorldSpaceToImageSpace(worldSize, worldX);
            int y = WorldSpaceToImageSpace(worldSize, worldY);
            e.Graphics.TranslateTransform(x, y);
            e.Graphics.RotateTransform((float)angle);
            drawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;
        }

        private Image wallimg = Image.FromFile("..\\..\\..\\Resources\\Images\\WallSprite.png");
        private Image worldimg = Image.FromFile("..\\..\\..\\Resources\\Images\\Background.png");
        private Image tankimg = Image.FromFile("..\\..\\..\\Resources\\Images\\DarkTank.png");
        private Image turrimg = Image.FromFile("..\\..\\..\\Resources\\Images\\DarkTurret.png");
        private Image projimg = Image.FromFile("..\\..\\..\\Resources\\Images\\shot_violet.png");

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">The PaintEventArgs to access the graphics</param>
        private void WorldDrawer(object o, PaintEventArgs e)
        {
            int size = Controller.GetWorld().GetGrid();
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle r = new Rectangle(-(size/2), -(size / 2), size, size);
            e.Graphics.DrawImage(worldimg, r);
        }

        /// <summary>
        /// Drawing One wall
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void WallDrawer(object o, PaintEventArgs e)
        {
            Walls w = o as Walls;

            int width = 50; 
            int numwallx = (int)(w.P2.GetX() - w.P1.GetX()) / 50;  // the amount of the walls to draw in x direction
            if (numwallx > 0)
                numwallx++;
            else
                numwallx--;
            int numwally = (int)(w.P2.GetY() - w.P1.GetY()) / 50;  // the amount of the walls to draw in y direction
            if (numwally > 0)
                numwally++;
            else
                numwally--;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        
            int x, y;

            if (numwallx > 0) // posi x
            {
                for (int i = 0; i < numwallx; i++)   
                {
                    x = -width / 2 + i * 50;
                    if (numwally > 0)   // +x,+y   
                    {
                        for (int j = 0; j < numwally; j++)
                        {
                            y = -width / 2 + j * 50;
                            e.Graphics.DrawImage(wallimg, new Rectangle(x, y, width, width));
                        }
                    }
                    else  // +x,-y
                    {
                        for (int j = 0; j > numwally; j--)   
                        {
                            y = -width / 2 + j * 50;
                            e.Graphics.DrawImage(wallimg, new Rectangle(x, y, width, width));
                        }
                    }
                }
            }
            else // nega x
            {
                for (int i = 0; i > numwallx; i--)  
                {
                    x = -width / 2 + i * 50;
                    if (numwally > 0)  // -x,+y
                    {
                        for (int j = 0; j < numwally; j++)
                        {
                            y = -width / 2 + j * 50;
                            e.Graphics.DrawImage(wallimg, new Rectangle(x, y, width, width));
                        }
                    }
                    else // -x, -y
                    {
                        for (int j = 0; j > numwally; j--)
                        {
                            y = -width / 2 + j * 50;
                            e.Graphics.DrawImage(wallimg, new Rectangle(x, y, width, width));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Drawing Tank body and health bar
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void TankDrawer(object o, PaintEventArgs e)
        {
            Tanks t = o as Tanks;
            int tankwidth = 60;
            // int wsize = 600;

            // e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle entity = new Rectangle(-(tankwidth / 2), -(tankwidth / 2), tankwidth, tankwidth);
            e.Graphics.DrawImage(tankimg, entity);
        }

        /// <summary>
        /// Drawing the tank body
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void TurretDrawer(object o, PaintEventArgs e)
        {
            Tanks t = o as Tanks;
            int wsize = 60;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Rectangles are drawn starting from the top-left corner.
            // So if we want the rectangle centered on the player's location, we have to offset it
            // by half its size to the left (-width/2) and up (-height/2)
            Rectangle r = new Rectangle(-(wsize / 2), -(wsize / 2), wsize, wsize);
            e.Graphics.DrawImage(turrimg, -(wsize / 2), -(wsize / 2), wsize, wsize);

        }

        private void InfoDrawer(object o, PaintEventArgs e)
        {
            Tanks t = o as Tanks;
            int tankwidth = 60;
            // int wsize = 600;

            // e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle healthBar = new Rectangle(-(tankwidth / 2), -(tankwidth / 2) - 20, 50 * t.Hp / 3, 10);
            using (System.Drawing.SolidBrush GreenBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green))
            using (System.Drawing.SolidBrush YellowBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Yellow))
            using (System.Drawing.SolidBrush RedBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
            {
                if (t.Hp == 1)
                    e.Graphics.FillRectangle(RedBrush, healthBar);
                else if (t.Hp == 2)
                    e.Graphics.FillRectangle(YellowBrush, healthBar);
                else
                    e.Graphics.FillRectangle(GreenBrush, healthBar);

                e.Graphics.DrawString(t.Name + ": " + t.Score, new Font("Arial", 16), RedBrush, -(tankwidth / 2), tankwidth/2);
            }
        }


        /// <summary>
        /// Drawing the beam
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void BeamDrawer(object o, PaintEventArgs e)
        {
            Beams b = o as Beams;
            int bwid = 10;
            int blen = 500;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            using (System.Drawing.SolidBrush CyanBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Cyan))
            {                
                e.Graphics.FillRectangle(CyanBrush, -(bwid/2), -(bwid / 2) - blen, bwid, blen);
            }
        }

        /// <summary>
        /// Projectile Drawer
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void ProjectileDrawer(object o, PaintEventArgs e)
        {
            Projectiles p = o as Projectiles;
            
            int wsize = 30;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle r = new Rectangle(-(wsize / 2), -(wsize / 2), wsize, wsize);
            
            e.Graphics.DrawImage(projimg, r);
        }

        /// <summary>
        /// Draw powerup
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void PowerUpsDrawer(object o, PaintEventArgs e)
        {
            Powerups p = o as Powerups;
            int wsize = 20;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle r = new Rectangle(-(wsize / 2), -(wsize / 2), wsize, wsize);
            Rectangle r1 = new Rectangle(-(wsize / 2) + 2, -(wsize / 2) + 2, wsize-4, wsize-4);
            Rectangle r2 = new Rectangle(-(wsize / 2) + 6, -(wsize / 2) + 6, wsize-12, wsize-12);
            using (System.Drawing.SolidBrush RedBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
            using (System.Drawing.SolidBrush GreenBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green))
            using (System.Drawing.SolidBrush YellowBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Yellow))
            {
                e.Graphics.FillEllipse(RedBrush, r);
                e.Graphics.FillEllipse(GreenBrush, r1);
                e.Graphics.FillEllipse(YellowBrush, r2);
            }
        }

        // This method is invoked when the DrawingPanel needs to be re-drawn
        protected override void OnPaint(PaintEventArgs e)
        {
            lock (Controller.GetWorld())
            {
                if (Controller.GetWorld().GetMyTank() != null)
                {
                    int worldSize = Controller.GetWorld().GetGrid();
                    double playerX = Controller.GetWorld().GetMyTank().Loc.GetX();  // player's x coordinate
                    double playerY = Controller.GetWorld().GetMyTank().Loc.GetY();  // player's y coordinate

                    // calculate view/world size ratio
                    double ratio = (double)800 / (double)worldSize;
                    int halfSizeScaled = (int)(worldSize / 2.0 * ratio);

                    double inverseTranslateX = -WorldSpaceToImageSpace(worldSize, playerX) + halfSizeScaled;
                    double inverseTranslateY = -WorldSpaceToImageSpace(worldSize, playerY) + halfSizeScaled;

                    e.Graphics.TranslateTransform((float)inverseTranslateX, (float)inverseTranslateY);

                    DrawObjectWithTransform(e, null, worldSize, 0, 0, 0, WorldDrawer);

                    // Draw the wall
                    foreach (Walls wall in Controller.GetWorld().WallList.Values)
                    {
                        DrawObjectWithTransform(e, wall, worldSize, wall.P1.GetX(), wall.P1.GetY(), 0, WallDrawer);
                    }

                    // Draw the Tanks, body and turret
                    foreach (Tanks tank in Controller.GetWorld().TankList.Values)
                    {
                        if (tank.Hp > 0)
                        {
                            tank.Bdir.Normalize();
                            tank.Tdir.Normalize();
                            
                            DrawObjectWithTransform(e, tank, worldSize, tank.Loc.GetX(), tank.Loc.GetY(), 0, InfoDrawer);
                            DrawObjectWithTransform(e, tank, worldSize, tank.Loc.GetX(), tank.Loc.GetY(), tank.Bdir.ToAngle(), TankDrawer);
                            DrawObjectWithTransform(e, tank, worldSize, tank.Loc.GetX(), tank.Loc.GetY(), tank.Tdir.ToAngle(), TurretDrawer);
                        }
                    }

                    // Draw the powerups
                    foreach (Powerups pu in Controller.GetWorld().PowerList.Values)
                    { 
                        if (pu.Died == false)
                            DrawObjectWithTransform(e, pu, worldSize, pu.Loc.GetX(), pu.Loc.GetY(), 0, PowerUpsDrawer);
                    }

                    // Draw the Projectiles
                    foreach (Projectiles p in Controller.GetWorld().ProjectileList.Values)
                    {
                        if (p.Died == false)
                        {
                            p.Dir.Normalize();
                            DrawObjectWithTransform(e, p, worldSize, p.Loc.GetX(), p.Loc.GetY(), p.Dir.ToAngle(), ProjectileDrawer);
                        }
                    }

                    // Draw the beams
                    foreach (Beams b in Controller.GetWorld().BeamList.Values)
                    {
                        if (b.Drawn_times < 100)
                        {
                            b.Dir.Normalize();
                            DrawObjectWithTransform(e, b, worldSize, b.Org.GetX(), b.Org.GetY(), b.Dir.ToAngle(), BeamDrawer);
                            b.Drawn_times++; 
                        }
                    }
                }
            }

            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
        }

        public void OnFrame()
        {
            // Don't try to redraw if the window doesn't exist yet.
            // This might happen if the controller sends an update
            // before the Form has started.
            if (!IsHandleCreated)
                return;

            // Invalidate this form and all its children
            // This will cause the form to redraw as soon as it can
            MethodInvoker setLabel = new MethodInvoker(() => this.Invalidate(true));
            this.Invoke(setLabel);

        }
    }

}
