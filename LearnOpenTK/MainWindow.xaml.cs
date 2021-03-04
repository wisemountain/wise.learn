using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using LearnOpenTK.Render;

using Size = System.Drawing.Size;

namespace LearnOpenTK
{
    public partial class MainWindow : Window
    {
        public static string HOME = @"E:\wisemountain\wise.scratchpad\LearnOpenTK";

        private WriteableBitmap backbuffer;

        private FrameBufferHandler framebufferHandler;

        private int frames;

        private DateTime lastMeasureTime;

        private Renderer renderer;

        private Scene scene = new Scene();

        private MeshRenderer meshRenderer = new MeshRenderer();

        private Point mousePoint;

        private Bullet.RayTestSpace space = new Bullet.RayTestSpace();

        public Render.Scene Scene {  get { return scene; } }

        public MainWindow()
        {
            this.InitializeComponent();

            this.renderer = new Renderer(new Size(400, 400));
            this.framebufferHandler = new FrameBufferHandler(scene);

            CameraInfo info = new CameraInfo();
            info.Position = new Vector3(500.0f, -500.0f, 500.0f);
            info.LookAt = new Vector3(0, 0, 0);
            info.Fov = MathHelper.PiOver4;
            info.Far = 10000.0f;
            info.Near = 2.0f;
            info.Width = 800;
            info.Height = 600;

            scene.SetupCamera(info);
            space.CreateWorld();

            CreateShapeNodes();
            CreateCollisions(); 

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += this.TimerOnTick;
            timer.Start();
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                var diff = e.GetPosition(this) - mousePoint;
                scene.Camera.Rotate(new Vector2((float)diff.X / 100, (float)diff.Y / 100));
                mousePoint = e.GetPosition(this);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mousePoint = e.GetPosition(this);

            // 
            // CTRL 버튼이 눌려져 있으면 피킹을 진행 
            // 
            var mx = (float)e.GetPosition(image).X;
            var my =(float)e.GetPosition(image).Y;

            if ( System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) )
            {
                var start = UnProjectPos(new Vector3(mx, my, 0.0f), scene.Camera.ProjMatrix, scene.Camera.ViewMatrix, framebufferHandler.Size);
                var end = UnProjectPos(new Vector3(mx, my, 1.0f), scene.Camera.ProjMatrix, scene.Camera.ViewMatrix, framebufferHandler.Size);

                Vector3 pos;

                if ( space.RayPick( start, end, out pos) )
                {
                    TextRange txt = new TextRange(log.Document.ContentStart, log.Document.ContentEnd);
                    txt.Text = "";

                    log.AppendText($"Pick:{pos}");
                }
            }
        }

        private Vector3 UnProjectPos(Vector3 mouse, Matrix4 projection, Matrix4 view, Size viewport)
        {
            Vector4 vec;

            vec.X = 2.0f * mouse.X / (float)viewport.Width - 1;
            vec.Y = -(2.0f * mouse.Y / (float)viewport.Height - 1);
            vec.Z = mouse.Z;
            vec.W = 1.0f;

            Matrix4 viewInv = Matrix4.Invert(view);
            Matrix4 projInv = Matrix4.Invert(projection);

            Vector4.Transform(ref vec, ref projInv, out vec);
            Vector4.Transform(ref vec, ref viewInv, out vec);

            if (vec.W > 0.000001f || vec.W < -0.000001f)
            {
                vec.X /= vec.W;
                vec.Y /= vec.W;
                vec.Z /= vec.W;
            }

            return vec.Xyz;
        } 

        private void Window_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            scene.Camera.Zoom(e.Delta / 10);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.W:
                    MoveForward();
                    break;
                case System.Windows.Input.Key.S:
                    MoveBackward();
                    break;
                case System.Windows.Input.Key.A:
                    MoveLeft();
                    break;
                case System.Windows.Input.Key.D:
                    MoveRight();
                    break;
            }
        }

        private void Render()
        {
            if (this.image.ActualWidth <= 0 || this.image.ActualHeight <= 0)
            {
                return;
            }

            // Framebuffer 생성. Viewport 설정
			this.framebufferHandler.Prepare( new Size( (int)this.imageContainer.ActualWidth , (int)this.imageContainer.ActualHeight ) );

            scene.Draw(meshRenderer);

            GL.Finish();

            this.framebufferHandler.Cleanup(ref this.backbuffer);

            if (this.backbuffer != null)
            {
                this.image.Source = this.backbuffer;
            }

            this.frames++;
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if (DateTime.Now.Subtract(this.lastMeasureTime) > TimeSpan.FromSeconds(1))
            {
                this.Title = this.frames + "fps";
                this.frames = 0;
                this.lastMeasureTime = DateTime.Now;
            }

            this.Render();
        }

        private void CreateShapeNodes()
        {
            ShaderManager.Instance.Load(
                new ShaderManager.ShaderConf() { 
                    Name = "diffuse", 
                    VsPath = $"{HOME}/Assets/Shader/diffuse.vert", 
                    FsPath = $"{HOME}/Assets/Shader/diffuse.frag" 
                });

            var mat = new MaterialDiffuse() { 
                ShaderProgram = "diffuse", 
                Tex = $"{HOME}/Assets/Tex/penguine.jpg" 
            };

            var meshCube = Shape.CreateCube(new Vector4(1, 0, 0, 1));
            var meshPlane = Shape.CreatePlane();
            var meshSphere = Shape.CreateSphere(new Vector4(0, 1, 1, 1), 25, 25);

            // cube
            {
                var sn = new Scene.Node() { Name = "sampleCube", Material = mat, Mesh = meshCube };
                scene.Add(sn);

                sn.Transform.Position = new Vector3(0, 0, 2.8f);
                sn.Transform.Scale = new Vector3(100, 100, 100);
                sn.Transform.Update();
            }

            // plane 
            {
                var sn = new Scene.Node() { Name = "samplePlane", Material = mat, Mesh = meshPlane};
                scene.Add(sn);

                sn.Transform.Position = new Vector3(0, 0, 32.1f);
                sn.Transform.Scale = new Vector3(300, 300, 300);
                sn.Transform.Update();
            }

            // sphere 
            {
                var sn = new Scene.Node() { Name = "sampleShpere", Material = mat, Mesh = meshSphere };
                scene.Add(sn);

                sn.Transform.Position = new Vector3(500, 0, 0);
                sn.Transform.Scale = new Vector3(300, 300, 300);
                sn.Transform.Update();
            }
        }

        private void CreateCollisions()
        {
            var keys = scene.Nodes;

            foreach ( var key in keys )
            {
                var n = scene.Get(key);

                if ( n != null )
                {
                    space.AddShape(n.Mesh, n.Transform);
                }
            }
        }


        private void MoveForward()
        {
            scene.Camera.Zoom(5);
        }

        private void MoveBackward()
        {
            scene.Camera.Zoom(-5);
        }

        private void MoveLeft()
        {
            scene.Camera.Move(new Vector3(-100, 0, 0));
        }

        private void MoveRight()
        {
            scene.Camera.Move(new Vector3(100, 0, 0));
        }

    }
}