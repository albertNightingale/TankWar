namespace View
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serverName = new System.Windows.Forms.TextBox();
            this.playerName = new System.Windows.Forms.TextBox();
            this.servLab = new System.Windows.Forms.Label();
            this.nameLab = new System.Windows.Forms.Label();
            this.connect = new System.Windows.Forms.Button();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // serverName
            // 
            this.serverName.Location = new System.Drawing.Point(111, 14);
            this.serverName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.serverName.Name = "serverName";
            this.serverName.Size = new System.Drawing.Size(148, 31);
            this.serverName.TabIndex = 0;
            this.serverName.Text = "localhost";
            // 
            // playerName
            // 
            this.playerName.Location = new System.Drawing.Point(378, 11);
            this.playerName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.playerName.Name = "playerName";
            this.playerName.ReadOnly = true;
            this.playerName.Size = new System.Drawing.Size(148, 31);
            this.playerName.TabIndex = 1;
            this.playerName.Text = "noob";
            // 
            // servLab
            // 
            this.servLab.AutoSize = true;
            this.servLab.Location = new System.Drawing.Point(18, 17);
            this.servLab.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.servLab.Name = "servLab";
            this.servLab.Size = new System.Drawing.Size(78, 25);
            this.servLab.TabIndex = 2;
            this.servLab.Text = "server:";
            // 
            // nameLab
            // 
            this.nameLab.AutoSize = true;
            this.nameLab.Location = new System.Drawing.Point(282, 14);
            this.nameLab.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.nameLab.Name = "nameLab";
            this.nameLab.Size = new System.Drawing.Size(71, 25);
            this.nameLab.TabIndex = 3;
            this.nameLab.Text = "name:";
            // 
            // connect
            // 
            this.connect.Location = new System.Drawing.Point(567, 11);
            this.connect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(112, 40);
            this.connect.TabIndex = 4;
            this.connect.TabStop = false;
            this.connect.Text = "connect";
            this.connect.UseVisualStyleBackColor = true;
            this.connect.Click += new System.EventHandler(this.connect_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.controlsToolStripMenuItem,
            this.aboutToolStripMenuItem1});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(74, 36);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // controlsToolStripMenuItem
            // 
            this.controlsToolStripMenuItem.Name = "controlsToolStripMenuItem";
            this.controlsToolStripMenuItem.Size = new System.Drawing.Size(238, 44);
            this.controlsToolStripMenuItem.Text = "Controls";
            this.controlsToolStripMenuItem.Click += new System.EventHandler(this.controlsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(238, 44);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(1647, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(85, 42);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "Help";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1860, 1277);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.connect);
            this.Controls.Add(this.nameLab);
            this.Controls.Add(this.servLab);
            this.Controls.Add(this.playerName);
            this.Controls.Add(this.serverName);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Battle of Tanks";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox serverName;
        private System.Windows.Forms.TextBox playerName;
        private System.Windows.Forms.Label servLab;
        private System.Windows.Forms.Label nameLab;
        private System.Windows.Forms.Button connect;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.MenuStrip menuStrip1;
    }
}

