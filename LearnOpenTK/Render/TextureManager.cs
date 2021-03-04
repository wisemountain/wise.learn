using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

using FreeImageAPI;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LearnOpenTK.Render
{
    class TextureManager
    {
        private static TextureManager inst = null;
        private static readonly object padlock = new object();
        private Dictionary<string, Texture> mTextures = new Dictionary<string, Texture>();

        public static TextureManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (inst == null)
                    {
                        inst = new TextureManager();
                    }
                    return inst;
                }
            }
        }

        public Texture GetTexture(string filename, TextureWrapMode mode)
        {
            if (filename.Length == 0)
            {
                return null; 
            }

            if (mTextures.ContainsKey(filename))
            {
                return mTextures[filename];
            }

            Texture tex = CreateTexture(filename, mode);
            return tex;
        }

        public int GetTextureHandle(string filename)
        {
            if (mTextures.ContainsKey(filename))
            {
                return mTextures[filename].TextureHandle;
            }

            return 0;
        }

        public Texture ReloadTexture(string filename, TextureWrapMode mode)
        {
            if (mTextures.ContainsKey(filename))
            {
                var tex = mTextures[filename];
                GL.DeleteTexture(tex.TextureHandle);
                mTextures.Remove(filename);
            }

            return GetTexture(filename, mode);
        }

        private Texture CreateTexture(string filename, TextureWrapMode mode)
        {
            Bitmap bitmap = null;
            int handle = 0;

            bitmap = LoadTexture(filename);

            if (bitmap == null)
            {
                return null;
            }

            handle = RegisterOpenGL(bitmap, mode);

            Texture tex = new Texture();
            tex.Set(filename, bitmap, handle);
            mTextures.Add(filename, tex);

            return tex;
        }

        private Bitmap LoadTexture(string filename)
        {
            FIBITMAP dib = new FIBITMAP();
            if (!dib.IsNull)
                FreeImage.Unload(dib);


            FREE_IMAGE_FORMAT fif = FreeImage.GetFIFFromFilename(filename);
            dib = FreeImage.Load(fif, filename, FREE_IMAGE_LOAD_FLAGS.DEFAULT);

            FreeImage.FlipVertical(dib);

            if (dib.IsNull)
            {
                //MessageBox.Show("로딩 실패" + file);
                return null;
            }

            Bitmap bitmap = FreeImage.GetBitmap(dib);

            FreeImage.Unload(dib);

            return bitmap;
        }

        private int RegisterOpenGL(Bitmap bitmap, TextureWrapMode wrapMode)
        {
            int tex = 0;

            GL.GenTextures(1, out tex);
            GL.BindTexture(TextureTarget.Texture2D, tex);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                ImageLockMode.ReadOnly, bitmap.PixelFormat);

            switch (bitmap.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb: // jpg
                    {
                        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, data.Width, data.Height, 0,
                                OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
                    }
                    break;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb: // TGA
                    {
                        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                                 OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                    }
                    break;
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);

            return tex;
        }
    }
}
