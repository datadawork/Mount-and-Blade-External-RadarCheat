using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Drawing;
using D3D = Microsoft.DirectX.Direct3D;
namespace Warband
{
    abstract public class Figures
    {
        protected Device device;
        protected static D3D.Font font;
        protected static D3D.Line line;

        public Figures(Device device) 
        {
            this.device = device;
            font = new D3D.Font(device, new System.Drawing.Font("Arial", 10, FontStyle.Regular));
            line = new D3D.Line(device);
        }

        protected void DrawRect(Rectangle<float> rect, Color color)
        {
            CustomVertex.TransformedColored[] vertecies = new CustomVertex.TransformedColored[8];

            vertecies[0].Position = new Vector4(rect.X, rect.Y, 0.0f, 1.0f);
            vertecies[1].Position = new Vector4(rect.X + rect.W, rect.Y, 0.0f, 1.0f);
            vertecies[2].Position = new Vector4(rect.X, rect.Y, 0.0f, 1.0f);
            vertecies[3].Position = new Vector4(rect.X, rect.Y + rect.H, 0.0f, 1.0f);
            vertecies[4].Position = new Vector4(rect.X + rect.W, rect.Y, 0.0f, 1.0f);
            vertecies[5].Position = new Vector4(rect.X + rect.W, rect.Y + rect.H, 0.0f, 1.0f);
            vertecies[6].Position = new Vector4(rect.X, rect.Y + rect.H, 0.0f, 1.0f);
            vertecies[7].Position = new Vector4(rect.X + rect.W, rect.Y + rect.H, 0.0f, 1.0f);

            for (int i = 0; i < 8; i++)
                vertecies[i].Color = color.ToArgb();

            device.VertexFormat = CustomVertex.TransformedColored.Format;
            device.DrawUserPrimitives(PrimitiveType.LineList, 4, vertecies);
        }

        protected void DrawLine(Vector4 start,Vector4 end, Color color)
        {
            CustomVertex.TransformedColored[] vertecies = new CustomVertex.TransformedColored[2];
         
            int col1 = color.ToArgb();
            vertecies[0] = new CustomVertex.TransformedColored(start.X, start.Y, 0, 1, col1);
            vertecies[1] = new CustomVertex.TransformedColored(end.X, end.Y, 0, 1, col1);

            for (int i = 0; i < vertecies.Length; i++)
            {
                vertecies[i].Color = color.ToArgb();
            }

            device.VertexFormat = CustomVertex.TransformedColored.Format;
            device.DrawUserPrimitives(PrimitiveType.LineList, 1, vertecies);
        }

        protected void DrawTriangle(Rectangle<float> rect, Color color)
        {
            CustomVertex.TransformedColored[] vertecies = new CustomVertex.TransformedColored[3];
            Vector4 vector = new Vector4(85, 110, 0, 1f);
            int col1 = color.ToArgb();
            vertecies[0] = new CustomVertex.TransformedColored(rect.X,rect.Y-(rect.H/2), 0, 1,col1);
            vertecies[1] = new CustomVertex.TransformedColored((rect.W/2)+rect.X, rect.Y + (rect.H / 2), 0, 1,col1);
            vertecies[2] = new CustomVertex.TransformedColored(rect.X-(rect.W /2) , rect.Y + (rect.H / 2), 0, 1,col1);
        
            for (int i = 0; i < vertecies.Length; i++)
            {
                vertecies[i].Color = color.ToArgb();

            }

            device.VertexFormat = CustomVertex.TransformedColored.Format;
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 1, vertecies);
        }

        protected Vector4 RotatePoint(Vector4 pointToRotate,Vector4 pointToRotating, double angle, bool angleInRadians = false)
        {
            Vector4 vector = pointToRotate - pointToRotating;
            var gedraaid = RotatePoint(vector, angle);
            return gedraaid + pointToRotating; 
        }

        protected Vector4 RotatePoint(Vector4 pointToRotate, double angle, bool angleInRadians = false)
        {
            angle = RadianToDegree(angle);
            float[,] lol = { { (float)Math.Cos(DegreeToRadian(angle)), (float)Math.Sin(DegreeToRadian(angle)) }, { (float)Math.Sin(DegreeToRadian(angle)) * -1, (float)Math.Cos(DegreeToRadian(angle)) } };

            return Mult(lol, pointToRotate);
        }

        private Vector4 Mult(float[,] m, Vector4 v)
        {
            Vector4 result = new Vector4();
            result.X = m[0,0] * v.X + m[0,1] * v.Y;
            result.Y = m[1,0] * v.X + m[1,1] * v.Y;
            result.Z = 0;
            result.W = 1.0f;
            return result;
        }

        protected static void DrawShadowText(string text, Point Position, Color color)
        {
            font.DrawText(null, text, new Point(Position.X + 1, Position.Y + 1), Color.Red);
            font.DrawText(null, text, Position, color);
        }

        protected double RadianToDegree(double angle)
        {

            double number = angle * (180.0 / Math.PI);
           
            return number;
        }

        protected float DegreeToRadian(double angle)
        {
            return (float)(Math.PI * angle / 180.0);
        }

        protected void DrawCircle(int x, int y, float radius, int slices, Color color)
        {
            CustomVertex.TransformedColored[] vertices = new CustomVertex.TransformedColored[slices + 2];
            int[] indices = new int[slices * 3];
            int col1;
            float deltaRad = DegreeToRadian(360) / slices;
            float delta = 0;

            col1 = color.ToArgb();

            vertices[0] = new CustomVertex.TransformedColored(x, y, 0, 1, col1);

            for (int i = 1; i < slices + 2; i++)
            {
                vertices[i] = new CustomVertex.TransformedColored(
                    (float)Math.Cos(delta) * radius + x,
                    (float)Math.Sin(delta) * radius + y,
                    0, 1, col1);
                delta += deltaRad;
            }

            indices[0] = 0;
            indices[1] = 1;

            for (int i = 0; i < slices; i++)
            {
                indices[3 * i] = 0;
                indices[(3 * i) + 1] = i + 1;
                indices[(3 * i) + 2] = i + 2;
            }

            device.VertexFormat = CustomVertex.TransformedColored.Format;
            device.DrawUserPrimitives(PrimitiveType.TriangleFan, slices, vertices);
        }

        protected struct Rectangle<T>
        {
            public T X { get; set; }
            public T Y { get; set; }
            public T W { get; set; }
            public T H { get; set; }

            public Rectangle(T x, T y, T w, T h)
                : this()
            {
                this.X = x;
                this.Y = y;
                this.W = w;
                this.H = h;
            }
        }
    }
}
