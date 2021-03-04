using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Platform;
using OpenTK.Graphics.OpenGL;

namespace LearnOpenTK.Render
{
    class Shader
    {
        /// <summary>
        /// OpenGL에서 할당 받은 Shader Program의 핸들입니다.
        /// </summary>
        private int mProgramHandle;

        /// <summary>
        /// OpenGL에서 할당 받은 Vertex Shader 핸들입니다.
        /// </summary>
        private int mVertexHandle;

        /// <summary>
        /// OpenGL에서 할당 받은 Fragment Shader 핸들입니다.
        /// </summary>
        private int mFragmentHandle;

        public string Name { get; private set; }

        public int ProgramHandle { get { return mProgramHandle; } }

        /// <summary>
        /// Shader의 생성자입니다.
        /// </summary>
        /// <param name="vs">Vertex Shader Source</param>
        /// <param name="fs">Fragment Shader Source</param>
        public Shader(string name, string vs, string fs)
        {
            Name = name;

            CreateShaders(vs, fs);
        }

        public bool Begin()
        {
            GL.UseProgram(ProgramHandle);

            return ProgramHandle != 0;
        }

        public void End()
        {
            GL.UseProgram(0);
        }

        /// <summary>
        /// Shader를 생성합니다.
        /// </summary>
        /// <param name="vs">Vertex Shader Source</param>
        /// <param name="fs">Fragment Shader Source</param>
        private void CreateShaders(string vs, string fs)
        {
            int status_code;
            string info;

            // Compile Vertex Shader
            mVertexHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(mVertexHandle, vs);
            GL.CompileShader(mVertexHandle);
            GL.GetShaderInfoLog(mVertexHandle, out info);
            GL.GetShader(mVertexHandle, ShaderParameter.CompileStatus, out status_code);

            if (status_code != 1)
            {
                throw new ApplicationException("Vertex Shader Compile Error : \r\n" + info);
            }

            // Compile Fragment Shader
            mFragmentHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(mFragmentHandle, fs);
            GL.CompileShader(mFragmentHandle);
            GL.GetShaderInfoLog(mFragmentHandle, out info);
            GL.GetShader(mFragmentHandle, ShaderParameter.CompileStatus, out status_code);

            if (status_code != 1)
            {
                throw new ApplicationException("Fragment Shader Compile Error : \r\n" + info);
            }

            // Create program
            mProgramHandle = GL.CreateProgram();

            GL.AttachShader(mProgramHandle, mVertexHandle);
            GL.AttachShader(mProgramHandle, mFragmentHandle);

            GL.LinkProgram(mProgramHandle);
        } 
    }
}

