using System;
using System.Collections.Generic;
using System.Media;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Windows.Input;
using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform;

namespace SPACE
{
    public partial class Form1 : Form
    {
        const int RANGE = 150;
        float speed = 0.5f;
        SByte dash = 0, viewMode = 0;
        SByte[] KeyPressed = new SByte[2] { 0, 0 };
        Timer updateTimer = new Timer();
        Random rand = new Random(DateTime.Now.Millisecond);
        SoundPlayer SP = new SoundPlayer(Properties.Resources.theme);
        Boolean SPB = false, Paused;
        Boolean[] ButtonPressed = new Boolean[3];
        float mouseMoveDistance = 0f;
        Point mouseMoveStart, viewStart;
        float[] viewDistance = new float[3] { 1f, 1f, 1f },
            offsetDistance = new float[3] { 0f, 0f, 0f },
            viewAngle = new float[2] { 0f, 0f };

        class Star
        {
            public float x;
            public float y;
            public float z;
            public Star(float _x, float _y, float _z)
            {
                x = _x;
                y = _y;
                z = _z;
            }
        }
        List<Star> starList = new List<Star>();

        public Form1()
        {
            if (SPB)
                SP.Play();
            InitializeComponent();
            AnT.InitializeContexts();
            AnT.Paint += new PaintEventHandler(pDraw);
            AnT.KeyDown += new KeyEventHandler(pKeyDown);
            AnT.KeyUp += new KeyEventHandler(pKeyUp);
            AnT.MouseClick += new MouseEventHandler(pMouseClick);
            AnT.MouseDown += new MouseEventHandler(pMouseDown);
            AnT.MouseUp += new MouseEventHandler(pMouseUp);
            AnT.MouseMove += new MouseEventHandler(pMouseMotion);
            //AnT.MouseWheel += new MouseEventHandler(pMouseWheel);
            //this.Paint += new PaintEventHandler(pDraw);
            //this.KeyDown += new KeyEventHandler(pKeyDown);
            updateTimer.Interval = 1;
            updateTimer.Tick += new EventHandler(pUpdate);
            updateTimer.Start();
        }
        
        //void pMouseWheel(object sender, MouseEventArgs e)
        //{
        //    viewOffset[2] += (float)(e.Delta / 120);
        //    Gl.glTranslatef(0, 0, viewOffset[2]);
        //    viewOffset[2] = (float)Math.Round(viewOffset[2], 1);
        //}
        
        void pMouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    ButtonPressed[0] = false;
                    break;
                case MouseButtons.Right:
                    ButtonPressed[1] = false;
                    break;
                case MouseButtons.Middle:
                    ButtonPressed[2] = false;
                    break;
            }
        }
       
        void pMouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    ButtonPressed[0] = true;
                    mouseMoveStart = e.Location;
                    break;
                case MouseButtons.Right:
                    viewStart = e.Location;
                    ButtonPressed[1] = true;
                    break;
                case MouseButtons.Middle:
                    ButtonPressed[2] = true;
                    break;
            }
        }
        
        void pMouseMotion(object sender, MouseEventArgs e)
        {
            if (ButtonPressed[0])
            {
                double x = mouseMoveStart.X,
                        y = mouseMoveStart.Y;
                if (e.Y < y)
                    mouseMoveDistance += (float)Math.Sqrt(Math.Pow((e.X - x), 2) + Math.Pow((e.Y - y), 2));
                else
                    mouseMoveDistance -= (float)Math.Sqrt(Math.Pow((e.X - x), 2) + Math.Pow((e.Y - y), 2));
            }
            if (ButtonPressed[1])
            {
                double x = viewStart.X,
                    y = viewStart.Y;

                 viewAngle[1] += (float)(e.X - x);
                 viewAngle[0] += (float)(e.Y - y);
                 viewStart = e.Location;
            }
        }

        void pMouseClick(object sender, MouseEventArgs e)
        {

        }

        void pKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.Space:
                    dash = 1;
                    break;
                case Keys.A:
                    KeyPressed[0] = 1;
                    break;
                case Keys.D:
                    KeyPressed[0] = -1;
                    break;
                case Keys.W:
                    KeyPressed[1] = -1;
                    break;
                case Keys.S:
                    KeyPressed[1] = 1;
                    break;
                case Keys.P:
                    if (!Paused)
                        Paused = true;
                    else
                        Paused = false;
                    break;
                case Keys.NumPad0:
                    mouseMoveDistance = 0f;
                    break;
                case Keys.NumPad5:
                    viewAngle[0] = 0f;
                    viewAngle[1] = 0f;
                    viewMode = 0;
                    offsetDistance[0] = 0;
                    offsetDistance[1] = 0;
                    break;
                case Keys.NumPad9:
                    if (starList.Count <= 50000)
                        for (int q = 0; q < 10000; ++q)
                            starList.Add(new Star(1.0f * (rand.Next(2 * RANGE) - RANGE),
                                             1.0f * (rand.Next(2 * RANGE) - RANGE),
                                             1.0f * (rand.Next(1000) - 1000)));
                    break;
                case Keys.NumPad6:
                    if (starList.Count > 10000)
                        starList.RemoveRange(starList.Count - 10000, 10000);
                    break;
                case Keys.NumPad1:
                    viewAngle[0] = 0f;
                    viewAngle[1] = 0f;
                    viewMode = 1;
                    break;
                case Keys.B:
                    if (SPB)
                    {
                        SPB = false;
                        SP.Stop();
                    }
                    else
                    {
                        SPB = true;
                        SP.Play();
                    }
                    break;
            }
        }

        void pKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.A:
                case Keys.D:
                    KeyPressed[0] = 0;
                    break;
                case Keys.W:
                case Keys.S:
                    KeyPressed[1] = 0;
                    break;
            }
        }

        void pUpdate(object sender, EventArgs e)
        {
            if (!Paused)
            {
                offsetDistance[0] += KeyPressed[0] * 3;
                offsetDistance[1] += KeyPressed[1] * 3;
                foreach (var star in starList)
                {
                    star.z += speed;
                    if (star.z > 50)
                    {
                        star.x = 1.0f * (rand.Next(2 * RANGE) - RANGE);
                        star.y = 1.0f * (rand.Next(2 * RANGE) - RANGE);
                        star.z = -500;
                    }
                }
                switch (dash)
                {
                    case 1:
                        speed += 0.2f;
                        if (speed > 25f)
                            dash = 2;
                        break;
                    case 2:
                        speed -= 0.7f;
                        if (speed <= 2f)
                        {
                            speed = 1f;
                            dash = 0;
                        }
                        break;
                }
            }
            AnT.Invalidate();
        }

        void pDraw(object sender, PaintEventArgs e)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            string output;

            if (viewMode != 0)
            {
                Gl.glRotatef(viewAngle[0] / 50, 1, 0, 0);
                Gl.glRotatef(viewAngle[1] / 50, 0, 1, 0);
            }
            Gl.glTranslatef(offsetDistance[0], offsetDistance[1], mouseMoveDistance / 10);

            foreach (var star in starList)
            {
                float brightness = 70 / Math.Abs(star.z - 130);
                Gl.glPointSize(2.0f - (50 - star.z) / 500f);
                Gl.glColor3f(brightness, brightness, brightness);
                Gl.glBegin(Gl.GL_POINTS);
                Gl.glVertex3f(star.x, star.y, star.z);
                Gl.glEnd();
            }

            Gl.glTranslated(30, 35, 500);
            Gl.glColor3d(1, 1, 1);
            Glut.glutWireTorus(30, 65, 20, 20);
            Gl.glColor3d(0.01, 0.01, 0.2);
            Glut.glutSolidTorus(30, 65, 20, 20);
            Gl.glTranslated(-30, -35, -500);

            Gl.glTranslatef(-offsetDistance[0], -offsetDistance[1], -mouseMoveDistance / 10);
            if (viewMode != 0)
            {
                Gl.glRotatef(-viewAngle[0] / 50, 1, 0, 0);
                Gl.glRotatef(-viewAngle[1] / 50, 0, 1, 0);
            }
            Gl.glFlush();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int q = 0; q < 20000; ++q)
                starList.Add(new Star(1.0f * (rand.Next(3 * RANGE) - RANGE),
                                      1.0f * (rand.Next(3 * RANGE) - RANGE),
                                      1.0f * (rand.Next(RANGE * 3) - RANGE * 3)));
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);

            Gl.glClearColor(0, 0, 0, 1);
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(45, (float)AnT.Width / (float)AnT.Height, 0.1, 1000);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glEnable(Gl.GL_DEPTH_TEST);
        } 
    }
}
