using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LearnOpenTK.Render
{
    public class MeshRenderer
    {
        public enum Location
        {
            MvpMatrix = 12,  // layout (location = 12) uniform mat4 mvp_matrix;
        }

        private List<Scene.Node> mRenderObjects = new List<Scene.Node>();

        public void Push(Scene.Node node)
        {
            mRenderObjects.Add(node);
        }

        public void Clear()
        {
            mRenderObjects.Clear();
        }

        public void Render(Camera camera)
        {
            foreach (var node in mRenderObjects)
            {
                if (node == null)
                    continue;

                if (node.Mesh == null)
                    continue;

                // Model-View Matrix
                Matrix4 mv = node.Transform.Matrix * camera.ViewMatrix;
                // Projection Matrix
                Matrix4 pj = camera.ProjMatrix;
                // Model-View-Projection Matrix Uniform
                Matrix4 mvp = mv * pj;

                node.Material.BeginDraw();

                // Model-View-Projection Matrix Uniform
                GL.UniformMatrix4((int)Location.MvpMatrix, false, ref mvp);

                node.Mesh.BeginDraw();
                node.Mesh.Draw();
                node.Mesh.EndDraw();

                node.Material.EndDraw();
            } 
        }
    }
}