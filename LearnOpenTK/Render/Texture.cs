using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LearnOpenTK.Render
{
    class Texture
    {
        public enum TexWrapMode
        {
            Repeat,
            Clamp,
            ClampToBorderNv,
            ClampToBorder,
            ClampToBorderSgis,
            ClampToBorderArb,
            ClampToEdgeSgis,
            ClampToEdge,
            MirroredRepeat
        }

        private string mFileName;
        private int mTextureHandle;
        private int mHeight;
        private int mWidth;

        public string FileName { get { return mFileName; } }

        public string Name { get { return System.IO.Path.GetFileName(mFileName); } }

        public int TextureHandle { get { return mTextureHandle; } }

        public int Height { get { return mHeight; } }

        public int Width { get { return mWidth; } }

        public Texture()
        {
            mFileName = "";
        }

        public override string ToString()
        {
            return Name;
        }

        public static bool IsTextureExtension(string ext)
        {
            var lext = ext.ToLower();

            if ( lext == ".jpg" || lext == ".tga" || lext == ".png")
            {
                return true;
            }

            return false;
        }

        public void Set(string filename, Bitmap bitmap, int handle)
        {
            mFileName = filename;
            mTextureHandle = handle;
            mHeight = bitmap.Height;
            mWidth = bitmap.Width;
        }
    }
}