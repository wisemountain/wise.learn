using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnOpenTK.Render
{
    public class ShaderManager
    {
        public class ShaderConf
        {
            public string Name { get; set; }

            public string VsPath { get; set; }

            public string FsPath { get; set; }
        }

        private static ShaderManager instance = null; 

        public static ShaderManager Instance
        {
            get
            {
                if ( instance == null )
                {
                    instance = new ShaderManager();
                }
                return instance;
            }
        }

        private Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();

        private string current = "";

        public bool Load(ShaderConf sc)
        {
            var vs = LoadFile(sc.VsPath);
            var fs = LoadFile(sc.FsPath);

            var shader = new Shader(sc.Name, vs, fs);
            shaders[sc.Name] = shader;

            return true;
        }

        public bool Begin(string name)
        {
            if (shaders.ContainsKey(name))
            {
                current = name;
                return shaders[current].Begin();
            }

            return false;
        }

        public void End()
        {
            if ( shaders.ContainsKey(current))
            {
                shaders[current].End();
                current = "";
            }
            else
            {
                throw new InvalidOperationException($"Shader {current} is not available");
            }
        }

        private string LoadFile(string filename)
        {
            StreamReader reader = new StreamReader(filename);

            var r = reader.ReadToEnd();

            reader.Close();

            return r;
        }
    }
}
