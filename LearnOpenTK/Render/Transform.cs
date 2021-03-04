using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

using OpenTK;


namespace LearnOpenTK.Render
{
    public class Transform
    {
        private Vector3 mPosition;
        private Vector3 mRotation;
        private Vector3 mScale;
        private Matrix4 mMatrix;

        // 행렬은 변환된 최종 결과를 갖고 있다. 
        public Matrix4 Matrix { get { return mMatrix; } }

        public Vector3 Position
        {
            get
            {
                return mPosition;
            }
            set
            {
                mPosition = value;
            }
        }

        public Vector3 Rotation
        {
            get { return mRotation; }
            set
            {
                mRotation = value;
            }
        }

        public Vector3 Scale
        {
            get { return mScale; }
            set
            {
                mScale = value;
            }
        }

        public Transform()
            : this(Matrix4.Identity)
        {
        }

        public Transform(Matrix4 parentMatrix)
        {
            mScale = Vector3.One;
            Update(parentMatrix);
        }

        public void Update()
        {
            Update(Matrix4.Identity);
        }

        public void Update(Matrix4 parentMatrix)
        {
            Matrix4 matTrans = Matrix4.CreateTranslation(mPosition);

            Matrix4 matRotX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(mRotation.X));
            Matrix4 matRotY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(mRotation.Y));
            Matrix4 matRotZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(mRotation.Z));
            Matrix4 matRot = matRotZ * matRotY * matRotX;

            Matrix4 matScale = Matrix4.CreateScale(mScale);

            mMatrix = matScale * matRot * matTrans * parentMatrix;
        }
    }
}
