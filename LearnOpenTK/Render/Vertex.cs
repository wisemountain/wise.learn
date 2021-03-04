using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace LearnOpenTK.Render
{
    public struct Vertex
    {
        public static int OffsetPosition = 0;
        public static int OffsetNormal = 12;
        public static int OffsetColor = 24;
        public static int OffsetUv = 40;
        public static int SizeInBytes = 48;

        public Vector3 Position { get; set; }

        public Vector3 Normal { get; set; }

        public Vector4 Color { get; set; }

        public Vector2 Uv { get; set; }
    }
}
