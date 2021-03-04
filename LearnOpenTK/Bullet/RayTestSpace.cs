using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearnOpenTK.Render;
using BulletSharp;

namespace LearnOpenTK.Bullet
{
    /// <summary>
    /// https://github.com/bulletphysics/bullet3/blob/master/examples/Raycast/RaytestDemo.cpp
    /// - 위 참고해서 만듦 
    /// </summary>
    class RayTestSpace
    {
        private DefaultCollisionConfiguration colConf;
        private CollisionDispatcher colDispatcher;
        private BroadphaseInterface broadPhase;
        private SequentialImpulseConstraintSolver solver;
        private DiscreteDynamicsWorld world;

        public void CreateWorld()
        {
            colConf = new DefaultCollisionConfiguration();
            colDispatcher = new CollisionDispatcher(colConf);
            broadPhase = new DbvtBroadphase();
            solver = new SequentialImpulseConstraintSolver();
            world = new DiscreteDynamicsWorld(colDispatcher, broadPhase, solver, colConf);
        }
        

        public void AddShape(Mesh mesh, Transform tran)
        {
            // Cube, Plane은 잘 된다. 구는 잘 안 되는데 
            // C++로 불릿 내부를 본 뒤에 다시 보도록 한다. 

            // TriangleMesh는 정적 노드를 기준으로 한다. 
            // 변환해서 넣어야 됨. WorldTransform 반영 안 함

            var triMesh = new TriangleMesh(true, true);
            
            for ( int i=0; i<mesh.Indices.Count; i+=3)
            {
                var v0 = ToBulletVector3(OpenTK.Vector3.TransformPosition(mesh.Vertices[mesh.Indices[i+0]].Position, tran.Matrix));
                var v1 = ToBulletVector3(OpenTK.Vector3.TransformPosition(mesh.Vertices[mesh.Indices[i+1]].Position, tran.Matrix));
                var v2 = ToBulletVector3(OpenTK.Vector3.TransformPosition(mesh.Vertices[mesh.Indices[i+2]].Position, tran.Matrix));

                triMesh.AddTriangle(v0, v1, v2, false);
            }

            var triShape = new BvhTriangleMeshShape(triMesh, true, true);
            // 아래는 생략해도 피킹 잘 됨
            // triShape.BuildOptimizedBvh();

            var colObj = new CollisionObject();

            colObj.CollisionShape = triShape;
            colObj.WorldTransform = ToBulletMatrix(OpenTK.Matrix4.Identity);

            world.AddCollisionObject(colObj);
            world.StepSimulation(0.1f);
        }

        public bool RayPick(OpenTK.Vector3 from, OpenTK.Vector3 to, out OpenTK.Vector3 col)
        {
            var bfrom = ToBulletVector3(from);
            var bto = ToBulletVector3(to);

            var cfrom = new BulletSharp.Math.Vector3();
            var cto = new BulletSharp.Math.Vector3();

            ClosestRayResultCallback rayCb = new ClosestRayResultCallback(ref cfrom, ref cto);

            world.RayTest(bfrom, bto, rayCb);

            // Lerp로 보간해야 결과가 나온다.  

            var p = BulletSharp.Math.Vector3.Lerp(bfrom, bto, rayCb.ClosestHitFraction);
            col = ToOpenVector3(p);

            return rayCb.HasHit;
        }  

        public static BulletSharp.Math.Vector3 ToBulletVector3(OpenTK.Vector3 v)
        {
            return new BulletSharp.Math.Vector3(v.X, v.Y, v.Z);
        }

        public static OpenTK.Vector3 ToOpenVector3(BulletSharp.Math.Vector3 v)
        {
            return new OpenTK.Vector3(v.X, v.Y, v.Z);
        }

        public static BulletSharp.Math.Matrix ToBulletMatrix(OpenTK.Matrix4 m)
        {
            return new BulletSharp.Math.Matrix(
                    m.M11, m.M12, m.M13, m.M14, 
                    m.M21, m.M22, m.M23, m.M24, 
                    m.M31, m.M32, m.M33, m.M34, 
                    m.M41, m.M42, m.M43, m.M44 
                );
        }
    }
}
