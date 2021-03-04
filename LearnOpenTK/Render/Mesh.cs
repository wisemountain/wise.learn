using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LearnOpenTK.Render
{
    public class Mesh
    {
        public enum Location
        {
            Position = 0,   // layout (location = 0) in vec3 position;
            Normal = 1,     // layout (location = 1) in vec3 normal;
            Color = 2,      // layout (location = 2) in vec4 color;
            Uv = 3,        // layout (location = 3) in vec2 texcoord;
        }

        public List<Vertex>  Vertices { get; private set; }

        public List<int> Indices { get; private set; }

        public int TriangleCount { get; private set; }

        private int mVBOHandle = 0;
        private int mIndexBufferHandle = 0;

        public Mesh()
        {
            Vertices = new List<Vertex>();
            Indices = new List<int>();
        }

        public bool Load(List<Vertex> vertices, List<int> indices, int triangleCount)
        {
            Vertices = vertices;
            Indices = indices;
            TriangleCount = triangleCount;

            if ( triangleCount * 3 > Indices.Count)
            {
                throw new InvalidOperationException($"Insufficent indices. Tris:{triangleCount}, Indices:{Indices.Count}");
            }

            CreateVbo();

            return true;
        }


        public void BeginDraw()
        {
            BindVbo();
        }

        public void Draw()
        {
            int offset = 0;
            int elementCount = TriangleCount * 3;

            GL.DrawElements(PrimitiveType.Triangles, elementCount, DrawElementsType.UnsignedInt, offset);
        }

        public void EndDraw()
        {
            UnbindVbo();
        }

        private void CreateVbo()
        {
            // vbo
            GL.GenBuffers(1, out mVBOHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBOHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Vertices.Count * Vertex.SizeInBytes), Vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // indices
            GL.GenBuffers(1, out mIndexBufferHandle);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIndexBufferHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(Indices.Count * sizeof(uint)), Indices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        private bool BindVbo()
        {
            // vbo
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBOHandle);

            // position
            GL.VertexAttribPointer((int)Location.Position, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vertex.OffsetPosition);
            GL.EnableVertexAttribArray((int)Location.Position);

            // normal
            GL.VertexAttribPointer((int)Location.Normal, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vertex.OffsetNormal);
            GL.EnableVertexAttribArray((int)Location.Normal);

            // color 
            GL.VertexAttribPointer((int)Location.Color, 4, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vertex.OffsetColor);
            GL.EnableVertexAttribArray((int)Location.Color);

            // uv1
            GL.VertexAttribPointer((int)Location.Uv, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vertex.OffsetUv);
            GL.EnableVertexAttribArray((int)Location.Uv);

            // index
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIndexBufferHandle);

            return true;
        }

        private void UnbindVbo()
        {
            GL.DisableVertexAttribArray((int)Location.Position);
            GL.DisableVertexAttribArray((int)Location.Normal);
            GL.DisableVertexAttribArray((int)Location.Color);
            GL.DisableVertexAttribArray((int)Location.Uv);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
