using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace CubeF3D
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public class Angle
    {
        private float _x;
        private float _y;
        private float _z;

        public float X
        {
            get => _x;
            set => _x = Normalize(value);
        }
        public float Y
        {
            get => _y;
            set => _y = Normalize(value);
        }

        public float Z
        {
            get => _z;
            set => _z = Normalize(value);
        }

        public Angle(float x = 0, float y = 0, float z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        private float Normalize(float angle)
        {
            angle = angle % 360; 
            if (angle < 0)
            {
                angle += 360;
            }
            return angle;
        }
    }

    public class Point
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Point(float x, float y, float z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public static Point TransformTo2D(Point point, Size size)
        {
            // Perspective Projection
            float d = 5.0f; // distance from the camera to the scene (viewer's distance)

            // Perspective transformation (with a simple scaling based on z)
            float x2D = point.X / (1 + point.Z / d);
            float y2D = point.Y / (1 + point.Z / d);

            // Scale and translate to fit in the screen
            x2D = x2D * size.Width / 2 + size.Width / 2;
            y2D = -y2D * size.Height / 2 + size.Height / 2;

            return new Point(x2D, y2D);
        }

    }

    public class Cube
    {
        private Form form;
        
        public Angle rotationAngle;

        public float RotationSpeed;

        public float ScaleFactor;
        private const float MaxScaleFactor = 1.5f;  
        private const float MinScaleFactor = 0.2f;  

        private Point[] vertices3D;
        private Point[] vertices2D;

        public Boolean highlightedVertices = false;
        public float vEllipseSize = 20;

        public Color edgesColor = Color.ForestGreen;

        public Axis currentAxis = Axis.Y;


        public Cube(Form form, float scale = 1f, float rotationSpeed = 0.1f)
        {
            this.form = form;

            vertices3D = new Point[]
            {
                new Point(-1, -1, -1), new Point( 1, -1, -1), new Point( 1,  1, -1), new Point(-1,  1, -1),
                new Point(-1, -1,  1), new Point( 1, -1,  1), new Point( 1,  1,  1), new Point(-1,  1,  1)
            };
            rotationAngle = new Angle();
            ScaleFactor = scale;
            RotationSpeed = rotationSpeed;
        }

        private Point[] ApplyTransformations(Point[] vertices)
        {
            Matrix4 scalingMatrix = Matrix4.ScalingMatrix(ScaleFactor, ScaleFactor, ScaleFactor);
            Matrix4 rotationMatrix;
            switch (currentAxis)
            {
                case Axis.X:
                    rotationMatrix = Matrix4.RotationMatrix_X(rotationAngle.X);
                    break;
                case Axis.Y:
                    rotationMatrix = Matrix4.RotationMatrix_Y(rotationAngle.Y);
                    break;
                case Axis.Z:
                    rotationMatrix = Matrix4.RotationMatrix_Z(rotationAngle.Z);
                    break;
                default:
                    rotationMatrix = Matrix4.IdentityMatrix;
                    break;
            }

            Point[] transformedVertices = new Point[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                transformedVertices[i] = Matrix4.ApplyToPoint(rotationMatrix, vertices[i]);
                transformedVertices[i] = Matrix4.ApplyToPoint(scalingMatrix, transformedVertices[i]);
            }

            return transformedVertices;
        }

        public void Render(PaintEventArgs e)
        {
            Point[] transformedVertices = ApplyTransformations(vertices3D);

            vertices2D = new Point[transformedVertices.Length];
            for (int i = 0; i < transformedVertices.Length; i++)
            {
                vertices2D[i] = Point.TransformTo2D(transformedVertices[i], form.ClientSize);
            }

            DrawEdges(e.Graphics);
            DrawHighlightedVertices(e.Graphics);
        }

        private void DrawHighlightedVertices(Graphics g)
        {
            if (highlightedVertices)
            {
                Font font = new Font("Arial", 12, FontStyle.Bold); 
                Brush textBrush = Brushes.White;

                Color startColor = Color.Blue;
                Color endColor = Color.Red;


                for (int i = 0; i < vertices2D.Length; i++)
                {
                    var vertex = vertices2D[i];

                    float colorRatio = (float)i / (vertices2D.Length - 1); 
                    Color vertexColor = InterpolateColor(startColor, endColor, colorRatio); 

                    Brush brush = new SolidBrush(vertexColor);

                    g.FillEllipse(brush, vertex.X - vEllipseSize / 2, vertex.Y - vEllipseSize / 2, vEllipseSize, vEllipseSize);

                    string indexText = i.ToString(); 
                    SizeF textSize = g.MeasureString(indexText, font); 
                    float textX = vertex.X - textSize.Width / 2; 
                    float textY = vertex.Y - textSize.Height / 2; 

                    g.DrawString(indexText, font, textBrush, textX, textY);
                }
            }
        }

        private void DrawEdges(Graphics g)
        {
            Pen pen = new Pen(edgesColor);
            DrawEdge(g, pen, 0, 1); DrawEdge(g, pen, 1, 2); DrawEdge(g, pen, 2, 3); DrawEdge(g, pen, 3, 0); // Back 
            DrawEdge(g, pen, 4, 5); DrawEdge(g, pen, 5, 6); DrawEdge(g, pen, 6, 7); DrawEdge(g, pen, 7, 4); // Front
            DrawEdge(g, pen, 0, 4); DrawEdge(g, pen, 1, 5); DrawEdge(g, pen, 2, 6); DrawEdge(g, pen, 3, 7); // Front & Back
        }

        private void DrawEdge(Graphics g, Pen pen, int startIdx, int endIdx)
        {
            g.DrawLine(
                pen, 
                vertices2D[startIdx].X, vertices2D[startIdx].Y,
                vertices2D[endIdx].X, vertices2D[endIdx].Y
            );
        }

        private Color InterpolateColor(Color startColor, Color endColor, float colorRatio)
        {
            int r = (int)(startColor.R + colorRatio * (endColor.R - startColor.R));
            int g = (int)(startColor.G + colorRatio * (endColor.G - startColor.G));
            int b = (int)(startColor.B + colorRatio * (endColor.B - startColor.B));

            return Color.FromArgb(r, g, b); 
        }

    }

    public partial class CubeWin : Form
    {
        Cube Cube { get; set; }

        private Timer timer;
        private int FPS = 60;
        private bool paused = false;

        public CubeWin()
        {
            InitializeComponent();

            Width = 600;
            Height = 600;
            BackColor = Color.Black;
            
            this.DoubleBuffered = true; 
            this.MouseWheel += CubeWin_MouseWheel;

            Cube = new Cube(this, scale: 1f);
            Cube.highlightedVertices = true;
            Cube.vEllipseSize = 40;

            InitializeTimer();

        }
        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 1000 / FPS;
            timer.Tick += (sender, e) =>
            {
                if (!paused) 
                {
                    Cube.rotationAngle.X += Cube.RotationSpeed;
                    Cube.rotationAngle.Y += Cube.RotationSpeed;
                    Cube.rotationAngle.Z += Cube.RotationSpeed;
                    Invalidate(); 
                }
            };
            timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Cube.Render(e);
        }

        private void CubeWin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.X)
            {
                Cube.currentAxis = Axis.X; 
            }
            else if (e.KeyCode == Keys.Y)
            {
                Cube.currentAxis = Axis.Y; 
            }
            else if (e.KeyCode == Keys.Z)
            {
                Cube.currentAxis = Axis.Z; 
            }
            if (e.KeyCode == Keys.Space)
            {
                paused = !paused; 
            }
        }

        private void CubeWin_MouseWheel(object sender, MouseEventArgs e)
        {
            Cube.ScaleFactor += e.Delta > 0 ? 0.1f : -0.1f;
        }
    }
}
