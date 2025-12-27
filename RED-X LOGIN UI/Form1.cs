
using Guna.UI2.WinForms;
using LicenseAuth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheArtOfDevHtmlRenderer.Adapters;

namespace RED_X_LOGIN_UI
{
    public partial class LoginForm : Form
    {


        public static api LicenseAuthApp = new api(
            name: "RED-X LITE",
            ownerid: "kiFnrOz8gq",
            secret: "9e83813783c063a0a4a7420a1e0861f949d6be8c2c920f2d9f8b99381ff81d20",
            version: "1.0"
        //path: @"Your_Path_Here" // (OPTIONAL) if you use token validation, example: Token.txt or anything.txt and paste token in txt file
        );

        public LoginForm()
        {



            InitializeComponent();
            LicenseAuthApp.init();
            // Enable double buffering for smooth particles
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            // Particle timer
            particleTimer = new Timer();
            particleTimer.Interval = 30; // ~33 FPS
            particleTimer.Tick += (s, e) =>
            {
                UpdateParticles();
                this.Invalidate(); // redraw form
            };
            particleTimer.Start();
            this.DoubleBuffered = true;

            // Hide controls initially
            loginBtn.Visible = false;
            passwordBox.Visible = false;
            usernameBox.Visible = false;





            // Fade Timer (~60 FPS)
            fadeTimer = new Timer();
            fadeTimer.Interval = 16;
            fadeTimer.Tick += FadeTimer_Tick;
            fadeTimer.Start();

            // Move Timer (~120 FPS)
            moveTimer = new Timer();
            moveTimer.Interval = 8;
            moveTimer.Tick += MoveTimer_Tick;
        }



        // FOR CUSTOM FONT
        private EmbeddedFontLoader _fontLoader;
        // Animation positions
        private PointF startPosition;
        private PointF targetPosition;
        private PointF currentPosition;

        // Fade & move
        private float textOpacity = 0f;
        private bool fadeInComplete = false;
        private bool moveComplete = false;

        private Timer fadeTimer;
        private Timer moveTimer;

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (!fadeInComplete)
            {
                textOpacity += 0.02f; // smooth fade-in
                if (textOpacity >= 1f)
                {
                    textOpacity = 1f;
                    fadeInComplete = true;
                    fadeTimer.Stop();
                    moveTimer.Start(); // start moving after fade-in
                }
                this.Invalidate();
            }
        }

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            if (!moveComplete)
            {
                float speed = 0.08f;
                currentPosition.X += (targetPosition.X - currentPosition.X) * speed;
                currentPosition.Y += (targetPosition.Y - currentPosition.Y) * speed;

                if (Math.Abs(currentPosition.Y - targetPosition.Y) < 1f)
                {
                    moveComplete = true;

                    // Hide controls initially
                    usernameBox.Visible = true;
                    passwordBox.Visible = true;
                    loginBtn.Visible = true;
                }

                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Clear background first
            e.Graphics.Clear(Color.Black);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            // Draw particles
            foreach (var p in particles)
            {
                using (SolidBrush b = new SolidBrush(Color.FromArgb((int)p.Life, p.Color)))
                {
                    e.Graphics.FillEllipse(b, p.Position.X, p.Position.Y, p.Size, p.Size);
                }
            }

            // Draw RED-X LITE text
            using (Font font = new Font(_fontLoader.GetFontFamilyByName("Nevan RUS"), 28, FontStyle.Regular))
            using (Brush brush = new SolidBrush(Color.FromArgb((int)(textOpacity * 255), 255, 0, 0)))
            {
                e.Graphics.DrawString("RED-X LOGIN UI", font, brush, currentPosition, StringFormat.GenericTypographic);
            }
        }


        // === Particle System ===
        private class Particle
        {
            public PointF Position;
            public PointF Velocity;
            public float Size;
            public float Life;
            public Color Color;
        }

        private readonly List<Particle> particles = new List<Particle>();
        private readonly Random rand = new Random();
        private Timer particleTimer;
        private void LoginForm_Load(object sender, EventArgs e)
        {
            LoadInformation();

            _fontLoader = new EmbeddedFontLoader();

            _fontLoader.LoadFontsFromResources(
                "RED_X_LOGIN_UI.NevanRUS.ttf",
                "RED_X_LOGIN_UI.NeutraTextTF-Demi.ttf"

            );


            label1.Font = new Font(_fontLoader.GetFontFamilyByName("Neutra Text TF Light"), 11f, FontStyle.Regular);
            passwordBox.Font = new Font(_fontLoader.GetFontFamilyByName("Neutra Text TF Light"), 11f, FontStyle.Regular);
            usernameBox.Font = new Font(_fontLoader.GetFontFamilyByName("Neutra Text TF Light"), 11f, FontStyle.Regular);
            loginBtn.Font = new Font(_fontLoader.GetFontFamilyByName("Neutra Text TF Light"), 11f, FontStyle.Bold);





            // Safely get the FontFamily
            var fontFamily = _fontLoader.GetFontFamilyByName("Nevan RUS");

            using (Graphics g = this.CreateGraphics())
            {
                Font font = new Font(fontFamily, 28, FontStyle.Bold);
                SizeF textSize = g.MeasureString("RED-X LOGIN UI", font, Point.Empty, StringFormat.GenericTypographic);

                startPosition = new PointF((this.ClientSize.Width - textSize.Width) / 2,
                                           (this.ClientSize.Height - textSize.Height) / 2);
                targetPosition = new PointF((this.ClientSize.Width - textSize.Width) / 2, 60);
                currentPosition = startPosition;
            }
        }

        // === Particle Update ===
        private void UpdateParticles()
        {
            // Add new particles
            if (particles.Count < 50) // limit for performance
            {
                particles.Add(new Particle
                {
                    Position = new PointF(rand.Next(0, this.Width), rand.Next(0, this.Height)),
                    Velocity = new PointF((float)(rand.NextDouble() - 0.5f) * 2f, (float)(rand.NextDouble() - 0.5f) * 2f),
                    Size = rand.Next(2, 5),
                    Life = 255,
                    Color = Color.FromArgb(200, Color.Red)
                });
            }

            // Update existing
            foreach (var p in particles.ToList())
            {
                p.Position = new PointF(p.Position.X + p.Velocity.X, p.Position.Y + p.Velocity.Y);
                p.Life -= 2f;

                if (p.Life <= 0)
                    particles.Remove(p);
            }
        }


        //PAINT OVERRRIDE
        // === Draw Particles ===

        private void LoadInformation()
        {
            usernameBox.Text = Properties.Settings.Default.usernameBox;
            passwordBox.Text = Properties.Settings.Default.passwordBox;
        }

        private void SaveUserAndPass()
        {
            Properties.Settings.Default.usernameBox = usernameBox.Text;
            Properties.Settings.Default.passwordBox = passwordBox.Text;
            Properties.Settings.Default.Save();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            SaveUserAndPass();
            // Input validation
            if (string.IsNullOrWhiteSpace(usernameBox.Text))
            {
                label1.Text = "Please enter your username first";
                return;
            }



            // Perform login
            LicenseAuthApp.login(usernameBox.Text, passwordBox.Text);

            if (LicenseAuthApp.response.success)
            {
                // Login success - open Main form and hide login
                MainForm main = new MainForm();
                main.Show();
                this.Hide();
            }
            else
            {
                // Login failed - show error message
                label1.Text = "Status: " + LicenseAuthApp.response.message;
            }
        }

        private void usernameBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
