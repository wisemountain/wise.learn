using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearnOpenTK.Render;
using BulletSharp;

namespace LearnOpenTK
{
    class LearnTrianglePick
    {
        DynamicsWorld world;

        public LearnTrianglePick()
        {
            
        }

        public void Create()
        {
            var bp = new DbvtBroadphase();
            var conf = new DefaultCollisionConfiguration();
            var dispatcher = new CollisionDispatcher(conf);
            var solver = new SequentialImpulseConstraintSolver();

            world = new DiscreteDynamicsWorld(dispatcher, bp, solver, conf);
        }

    }
}
