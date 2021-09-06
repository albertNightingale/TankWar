using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Control;

namespace View
{
    public partial class Form1 : Form
    {
        private Controller controller;
        private GamingPanel drawingPanel;

        public Form1()
        {
            InitializeComponent();
        }

        private void connect_Click(object sender, EventArgs e)
        {
            connect.Enabled = false;
            serverName.Enabled = false;
            playerName.Enabled = false;

            controller = new Controller(playerName.Text);  
            controller.GetConnection(serverName.Text);  // establish the connection  

            ClientSize = new Size(ClientSize.Width, ClientSize.Height);
            drawingPanel = new GamingPanel(controller);
            drawingPanel.Location = new Point((ClientSize.Width - 800)/2, 50);
            drawingPanel.Size = new Size(800, 800);
            drawingPanel.BackColor = Color.Blue;
            Controls.Add(drawingPanel); // set up panel
            drawingPanel.Focus();
            controller.SetHandler(drawingPanel.OnFrame);
        }

        private void controlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("w:                 Move up\n" +
                            "s:                 Move down\n" +
                            "d:                 Move right\n" +
                            "a:                 Move left\n" +
                            "mouse:             Aim\n" +
                            "left-click:        Fire Projectile\n" +
                            "right-click:       Fire Beam\n");
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TankWars: \nGame Design and Implementation: Jeneve and Albert \n" +
                "Your goal is to kill all other tanks, \ndoes not matter whether\n" + 
                "they are AI or your classmates..." ); 
        }
    }
}
