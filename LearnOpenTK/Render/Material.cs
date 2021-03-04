using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace LearnOpenTK.Render
{
    public class Material
    {
        public string ShaderProgram { get; set; }

        public void BeginDraw()
        {
            ShaderManager.Instance.Begin(ShaderProgram);
            OnBeginDraw();
        }

        public void EndDraw()
        {
            OnEndDraw();
            ShaderManager.Instance.End();
        }

        virtual protected void OnBeginDraw()
        {
        }

        virtual protected void OnEndDraw()
        {
        }
    }
}
