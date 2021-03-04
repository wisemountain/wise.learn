using System;
using System.Collections.Generic;
using System.Windows.Forms;

using OpenTK;

namespace LearnOpenTK.Render
{
    public class Plane
    {
        public enum DiscriminantType
        {
            Invalid,
            OnPoint,     // Plane 위의 점, 판별식의 값이 0
            FrontPoint,  // Normal의 방향, 판별식의 값이 0보다 큼
            BackPoint,   // Normal의 반대 방향, 판별식의 값이 0보다 작음
        }

        private Vector3 mNormal;                // 평명의 법선 벡터

        private float mLengthToZeroPoint;      // 원점에서 평면까지의 최단 거리

        public Vector3 Normal { get { return mNormal; } set { mNormal = value; } }

        public Plane() 
        { 
        }

        /// <summary>
        /// 법선 벡터와 원점으로부터 평면까지의 최단거리로 평면을 구성합니다.
        /// </summary>
        /// <param name="a">법선벡터의 X값</param>
        /// <param name="b">법선벡터의 Y값</param>
        /// <param name="c">법선벡터의 Z값</param>
        /// <param name="d">최단 거리</param>
        public Plane(float a, float b, float c, float d)
        {
            Vector3 normal = new Vector3(a, b, c);

            if (normal == Vector3.Zero)
            {
                MessageBox.Show("Plane을 구성하기 위한 Normal 값이 잘못 되었습니다(" + normal.ToString() + ").");
                return;
            }

            normal.Normalize();
            mNormal = normal;
            mLengthToZeroPoint = d / (float)Math.Sqrt(a * a + b * b + c * c);
        }

        /// <summary>
        /// 법선 벡터와 원점으로부터 평면까지의 최단거리로 평면을 구성합니다.
        /// </summary>
        public Plane(Vector3 normal, float length)
            : this(normal.X, normal.Y, normal.Z, length) 
        { 
        }

        /// <summary>
        /// 3개의 점으로 평면을 구성합니다.
        /// </summary>
        public Plane(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            FromPoints(ref p0, ref p1, ref p2);
        }

        /// <summary>
        /// 평면을 복사합니다.
        /// </summary>
        public Plane(Plane other) 
        { 
            mNormal = other.mNormal; 
            mLengthToZeroPoint = other.mLengthToZeroPoint; 
        }
        
        /// <summary>
        /// 3개의 점으로 평면을 구성합니다.
        /// </summary>
        public void FromPoints(ref Vector3 p0, ref Vector3 p1, ref Vector3 p2)
        {
            Vector3 v1 = new Vector3(p1 - p0);
            Vector3 v2 = new Vector3(p2 - p0);

            v1.Normalize();
            v2.Normalize();

            Vector3.Cross(ref v1, ref v2, out mNormal);
            mNormal.Normalize();
            mLengthToZeroPoint = -Vector3.Dot(mNormal, p0);
        }

        /// <summary>
        /// 평면에서 point까지의 최단 거리
        /// </summary>
        public float DistanceToPoint(Vector3 point)
        {
            return Math.Abs(ProcessDiscriminat(point));
        }

        /// <summary>
        /// point와 평면간의 판별식
        /// </summary>
        public DiscriminantType Discriminant(Vector3 point)
        {
            float discriminant = ProcessDiscriminat(point);
            DiscriminantType type = DiscriminantType.Invalid;
            if (discriminant == 0)
                type = DiscriminantType.OnPoint;
            else if (discriminant > 0)
                type = DiscriminantType.FrontPoint;
            else if (discriminant < 0)
                type = DiscriminantType.BackPoint;

            return type;
        }

        private float ProcessDiscriminat(Vector3 point)
        {
            return Vector3.Dot(mNormal, point) + mLengthToZeroPoint;
        }
    }
}