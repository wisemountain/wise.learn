using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace LearnOpenTK.Render
{
    public class MaterialDiffuse : Material
    {
        public string Tex { get; set; }

        protected override void OnBeginDraw()
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.DepthTest);

            var tex = TextureManager.Instance.GetTexture(Tex, TextureWrapMode.Clamp);
            GL.BindTexture(TextureTarget.Texture2D, tex.TextureHandle); 
        }

        protected override void OnEndDraw()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.DepthTest);
        }
    }
}
