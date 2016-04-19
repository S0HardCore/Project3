using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        Timer updateTimer = new Timer();
        Random newRandom = new Random(DateTime.Now.Millisecond);
        int lastTick, frameRate, lastFrameRate;
        PointF lastClick = new PointF(0, 0);
        float scale = 1f;
        class Particle
        {
            public float X;
            public float Y;
            public double D;
            public float S;

            public Particle(float x, float y, double d, float s)
            {
                X = x;
                Y = y;
                D = d;
                S = s;
            }
        }

        List<Particle> objs = new List<Particle>();

        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(1920, 1080);
            this.Paint += new PaintEventHandler(pDraw);
            this.KeyDown += new KeyEventHandler(pKeyDown);
            this.MouseWheel += new MouseEventHandler(pMouseWheel);
            //this.KeyUp += new KeyEventHandler(pKeyDown);
            this.MouseClick += new MouseEventHandler(pMouseClick);
            updateTimer.Interval = 1;
            updateTimer.Tick += new EventHandler(pUpdate);
            updateTimer.Start(); InitializeComponent();
            for (int a = 0; a < 4096; ++a)
            {
                float X = newRandom.Next(1920) - 960;
                float Y = newRandom.Next(1080) - 540;
                float D = newRandom.Next(360);
                float S = newRandom.Next(10) + (float)newRandom.NextDouble();
                objs.Add(new Particle(X, Y, D, S));
            }
        }

        private int CalculateFrameRate()
        {
            if (System.Environment.TickCount - lastTick >= 1000)
            {
                lastFrameRate = frameRate;
                frameRate = 0;
                lastTick = System.Environment.TickCount;
            }
            frameRate++;
            return lastFrameRate;
        }

        void pMouseWheel(object sender, MouseEventArgs e)
        {
            scale += (float)(e.Delta / 120) / 10;
            if (scale > 1)
                scale = 1;
            else if (scale < 0.1)
                scale = 0.1f;
            Math.Round(scale, 1);
        }

        void pKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;
            }
        }

        void pMouseClick(object sender, MouseEventArgs e)
        {
            PointF newE = e.Location;
            newE.X = newE.X / scale - 1920 / scale / 2;
            newE.Y = newE.Y / scale - 1080 / scale / 2;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    foreach (var o in objs)
                    {
                        //if (o.X >= 1920 | o.X <= 0)
                        //    o.D = (360 - o.D);
                        //else
                            o.X = newE.X + newRandom.Next(-50, 50);
                        //if (o.Y >= 1080 | o.Y <= 0)
                        //    o.D = (360 - o.D);
                        //else
                            o.Y = newE.Y + newRandom.Next(-50, 50);
                    }
                    break;
                case MouseButtons.Right:
                    foreach (var o in objs)
                    {
                        float XO = lastClick.X - o.X;
                        float YO = lastClick.Y - o.Y;
                        o.X = newE.X + XO;
                        o.Y = newE.Y + YO;
                    }
                    //foreach (var o in objs)
                    //{
                    //    if (o.S <= 10)
                    //        o.S += 2;
                    //    else
                    //        o.S = 2;
                    //}
                    break;
            }
            lastClick = newE;
        }

        void pUpdate(object sender, EventArgs e)
        {
            foreach (var o in objs)
            {
                o.X += (float)Math.Cos(o.D) * o.S;
                o.Y += (float)Math.Sin(o.D) * o.S;
            }
            Invalidate();
        }

        void pDraw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.ScaleTransform(scale, scale);
            g.TranslateTransform(1920 / scale / 2, 1080 / scale / 2);
            foreach (var o in objs)
            {
                int R = newRandom.Next(255),
                    G = newRandom.Next(255),
                    B = newRandom.Next(255);
                SolidBrush myBrush = new SolidBrush(Color.FromArgb(R, G, B));
                g.FillRectangle(myBrush, o.X, o.Y, 30 * (1.1f - scale), 30 * (1.1f - scale));
            }
            g.ResetTransform();
            string ba = "";
            for (float a = 0.1f; a < 1.1f; a += 0.1f)
            {
                ba += Math.Round(a, 1) + " = ";
                ba += ((5f - a * 5f) * 1920).ToString();
                ba += "x";
                ba += ((5f - a * 5f) * 1080).ToString();
                ba += ";;";
                ba += (1920 / a / 2).ToString();
                ba += "x";
                ba += (1080 / a / 2).ToString();
                ba += "\n";
            }
             g.DrawString(ba + CalculateFrameRate().ToString() + "\n" + Math.Round(scale, 1).ToString(), new Font("Verdana", 13), Brushes.Black, new Point(0, 0));
        }
    }
}
