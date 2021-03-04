using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace LearnOpenTK.Render
{
    public class Shape
    {
        public static Mesh CreateCube(Vector4 color)
        {
            List<int> indices = new List<int>() {
                0, 1, 2, 0, 2, 3,       //front
                4, 5, 6, 4, 6, 7,       //right 
                8, 9, 10, 8, 10, 11,    //back 
                12, 13, 14, 12, 14, 15, //left 
                16, 17, 18, 16, 18, 19, //upper 
                20, 21, 22, 20, 22, 23  //bottom
            }; 

            List<Vertex> verts = new List<Vertex>();

            // front
            verts.Add( new Vertex() { Position = new Vector3(-1, -1, 1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add( new Vertex() { Position = new Vector3(1, -1, 1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add( new Vertex() { Position = new Vector3(1, 1, 1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add( new Vertex() { Position = new Vector3(-1, 1, 1), Normal = new Vector3(), Color = color, Uv = new Vector2() });

            // right
            verts.Add(new Vertex() { Position = new Vector3(1, 1, 1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add(new Vertex() { Position = new Vector3(1, 1, -1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add(new Vertex() { Position = new Vector3(1, -1, -1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add(new Vertex() { Position = new Vector3(1, -1, 1), Normal = new Vector3(), Color = color, Uv = new Vector2() });

            // back 
            verts.Add(new Vertex() { Position = new Vector3(-1, -1, -1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add(new Vertex() { Position = new Vector3(1, -1, -1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add(new Vertex() { Position = new Vector3(1, 1, -1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add(new Vertex() { Position = new Vector3(-1, 1, -1), Normal = new Vector3(), Color = color, Uv = new Vector2() });

            // left
            verts.Add(new Vertex() { Position = new Vector3(-1, -1, -1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add(new Vertex() { Position = new Vector3(-1, -1, 1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add(new Vertex() { Position = new Vector3(-1, 1, 1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add(new Vertex() { Position = new Vector3(-1, 1, -1), Normal = new Vector3(), Color = color, Uv = new Vector2() });

            // upper
            verts.Add(new Vertex() { Position = new Vector3(1, 1, 1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add(new Vertex() { Position = new Vector3(-1, 1, 1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add(new Vertex() { Position = new Vector3(-1, 1, -1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add(new Vertex() { Position = new Vector3(1, 1, -1), Normal = new Vector3(), Color = color, Uv = new Vector2() });

            // bottom 
            verts.Add(new Vertex() { Position = new Vector3(-1, -1, -1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add(new Vertex() { Position = new Vector3(1, -1, -1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add(new Vertex() { Position = new Vector3(1, -1, 1), Normal = new Vector3(), Color = color, Uv = new Vector2() });
            verts.Add(new Vertex() { Position = new Vector3(-1, -1, 1), Normal = new Vector3(), Color = color, Uv = new Vector2() });

            var mesh = new Mesh();
            mesh.Load(verts, indices, 12);

            return mesh;
        }

        public static Mesh CreatePlane()
        {
            List<int> indices = new List<int>() {
                0, 1, 2,
                2, 3, 0
            };

            List<Vertex> verts = new List<Vertex>();

            verts.Add(new Vertex() { Position = new Vector3(1, 1, 0), Normal = new Vector3(), Color = new Vector4(0, 1, 0, 1), Uv = new Vector2(0, 0) });
            verts.Add(new Vertex() { Position = new Vector3(1, -1, 0), Normal = new Vector3(), Color = new Vector4(0, 1, 0, 1), Uv = new Vector2(0, 1) });
            verts.Add(new Vertex() { Position = new Vector3(-1, -1, 0), Normal = new Vector3(), Color = new Vector4(0, 1, 0, 1), Uv = new Vector2(1, 1) });
            verts.Add(new Vertex() { Position = new Vector3(-1, 1, 0), Normal = new Vector3(), Color = new Vector4(0, 1, 0, 1), Uv = new Vector2(1, 0) });

            var mesh = new Mesh();
            mesh.Load(verts, indices, 2);

            return mesh;
        }

        public static Mesh CreateSphere(Vector4 color, float ures, float vres)
        {
            List<int> indices = new List<int>();
            List<Vertex> verts = new List<Vertex>();

            // https://stackoverflow.com/questions/7687148/drawing-sphere-in-opengl-without-using-glusphere
            // - 2nd answer using spherical coordinate function
            float startU = 0;
            float startV = 0;
            float endU = OpenTK.MathHelper.Pi * 2;
            float endV = MathHelper.Pi;

            float stepU = (endU - startU) / ures; // step size between U-points on the grid
            float stepV = (endV - startV) / vres;   // step size between V-points on the grid

            for (int i = 0; i < ures; i++)
            { // U-points
                for (int j = 0; j < vres; j++)
                { // V-points
                    float u = i * stepU + startU;
                    float v = j * stepV + startV;
                    float un = (i + 1 == ures) ? endU : (i + 1) * stepU + startU;
                    float vn = (j + 1 == vres) ? endV : (j + 1) * stepV + startV;

                    // Find the four points of the grid
                    // square by evaluating the parametric
                    // surface function
                    Vector3 p0 = SphericalCoord(u, v);
                    Vector3 p1 = SphericalCoord(u, vn);
                    Vector3 p2 = SphericalCoord(un, v);
                    Vector3 p3 = SphericalCoord(un, vn);

                    // NOTE: For spheres, the normal is just the normalized
                    // version of each vertex point; this generally won't be the case for
                    // other parametric surfaces.
                    // Output the first triangle of this grid square
                    verts.Add(new Vertex() { Position = p0, Color = color, Normal = p0, Uv = new Vector2() });
                    verts.Add(new Vertex() { Position = p2, Color = color, Normal = p0, Uv = new Vector2() });
                    verts.Add(new Vertex() { Position = p1, Color = color, Normal = p0, Uv = new Vector2() });

                    indices.Add(verts.Count - 3);
                    indices.Add(verts.Count - 2);
                    indices.Add(verts.Count - 1);

                    // Output the other triangle of this grid square
                    verts.Add(new Vertex() { Position = p3, Color = color, Normal = p0, Uv = new Vector2() });
                    verts.Add(new Vertex() { Position = p1, Color = color, Normal = p0, Uv = new Vector2() });
                    verts.Add(new Vertex() { Position = p2, Color = color, Normal = p0, Uv = new Vector2() });

                    indices.Add(verts.Count - 3);
                    indices.Add(verts.Count - 2);
                    indices.Add(verts.Count - 1);
                }
            }

            var mesh = new Mesh();
            mesh.Load(verts, indices, indices.Count / 3);
            return mesh;
        }

        private static Vector3 SphericalCoord(float u, float v)
        {
            // [ cos(u)*sin(v)*r, cos(v)*r, sin(u)*sin(v)*r ]

            float sinv = (float)Math.Sin(v);
            float cosv = (float)Math.Cos(v);
            float sinu = (float)Math.Sin(u);
            float cosu = (float)Math.Cos(u);

            return new Vector3(cosu * sinv, cosv, sinu * sinv);
        }
    }
}
