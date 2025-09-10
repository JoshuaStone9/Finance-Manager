using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace FM
{
    
    public partial class NotificationCenter : Form
    {
        private Timer closeTimer;
        private Label messageLabel;
        public NotificationCenter(string message, int durationMs = 3000, Point ? location = null)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.FromArgb(255, 120, 120);
            this.Size = new Size(300, 80);

            if (location.HasValue)
                this.Location = location.Value;
            else
            {
                var screen = Screen.PrimaryScreen.WorkingArea;
                this.Location = new Point(screen.Right - this.Width - 10, screen.Bottom - this.Height - 10);
            }

            messageLabel = new Label()
            {
                Text = message,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Montserrat", 10, FontStyle.Regular),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            this.Controls.Add(messageLabel);

            closeTimer = new System.Windows.Forms.Timer();
            closeTimer.Interval = durationMs;
            closeTimer.Tick += (s, e) => { this.Close(); };
            closeTimer.Start();
        }
    }
}
