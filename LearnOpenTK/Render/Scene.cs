using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LearnOpenTK.Render
{
    public class Scene
    {
        public class Node
        {
            public string Name { get; set; }

            public Transform Transform{ get; private set; }

            public Mesh Mesh { get; set; }

            public Material Material { get; set; }

            public Node()
            {
                Transform = new Transform();
            }
        }

        Dictionary<string, Node> nodes = new Dictionary<string, Node>();

        public Camera Camera { get; private set; }

        public int Tick { get { return Environment.TickCount; } }

        public Dictionary<string, Node>.KeyCollection Nodes { get { return nodes.Keys;  } }

        public void SetupCamera(CameraInfo info)
        {
            Camera = new Camera(info);
        }

        public void Add(Node node)
        {
            nodes[node.Name] = node;
        }

        public void Remove(string name)
        {
            nodes.Remove(name);
        }

        public Node Get(string name)
        {
            if ( nodes.ContainsKey(name))
            {
                return nodes[name];
            }

            return null;
        }

        public void Draw(MeshRenderer renderer)
        {
            GL.ClearColor(Color.FromArgb(200, Color.Black));
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            foreach ( var kv in nodes )
            {
                renderer.Push(kv.Value);
            }

            renderer.Render(Camera);
            renderer.Clear();
        }

        public void RenderArc(float radius, float angle)
        {
            // RenderArc는 제대로 나온다
            float lw = GL.GetFloat(GetPName.LineWidth);
            GL.LineWidth(2);
            GL.Begin(PrimitiveType.LineLoop);

            GL.Color3(Color.DarkOrchid);

            // y 축 방향을 향하는 호를 그린다. 
            // 먼저 원점에서 시작 
            GL.Vertex3(0, 0, 0);

            float dec = 1;

            float sa = angle / 2;
            float ea = -angle / 2;

            for (float a = sa; a >= ea; a -= dec)
            {
                float ra = MathUtil.DegreeToRadian(a);
                float x = (float)Math.Sin(ra) * radius;
                float y = (float)Math.Cos(ra) * radius;

                GL.Vertex3(x, y, 0);
            }

            GL.End();

            GL.LineWidth(lw);
        }

        private void DrawSampleArcs()
        {
            GL.MatrixMode(MatrixMode.Modelview);

            Camera.LoadViewMatrix();
            GL.Translate(0, 0, -10);
            GL.Rotate(Tick, Vector3.UnitZ);
            RenderArc(100, 120);

            Camera.LoadViewMatrix();
            GL.Translate(10, 10, -10);
            GL.Rotate(-Tick, Vector3.UnitZ);
            RenderArc(200, 120);
        }
    } 
}
