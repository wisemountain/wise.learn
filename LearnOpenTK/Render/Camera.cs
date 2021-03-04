using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LearnOpenTK.Render
{
    public class CameraInfo
    {
        public Vector3 Position;
        public Vector3 LookAt;
        public float Fov;
        public float Near;
        public float Far;

        public float Width;
        public float Height;
    }

    public class CameraData
    {
        public void CalcUpRightDirection(Vector3 look2Pos)
        {
            Vector3.Normalize(ref look2Pos, out Direction);

            if (Direction.Length < 0.00001f)
            {
                // Avoid very small number.
                // OpenTK의 Normalize() 함수는 길이가 0일 경우 NaN으로 지정
                return;
            }

            // Up
            Up.Normalize();


            // Right
            Vector3.Cross(ref Up, ref Direction, out Right);
            Right.Normalize();

            // Up
            Vector3.Cross(ref Direction, ref Right, out Up);
            Up.Normalize();
        }

        /// <summary>
        /// 카메라의 가로, 세로, Fov, 종횡비 변경시에만 호출됩니다.
        /// </summary>
        public Matrix4 CreateProjMatrix(float width, float height)
        {
            Aspect = width / (float)height;

            return Matrix4.CreatePerspectiveFieldOfView(Fov, Aspect, Near, Far);
        }

        /// <summary>
        /// 절두체에 사용될 8개의 모서리점의 데이터를 설정합니다.
        /// </summary>
        public void Setup8Points()
        {
            // 8개의 정점 구하기
            //float farLeft = -(float)Math.Tan(Fov * 0.5f * Aspect) * Far;
            float farLeft = -(float)System.Math.Tan(Fov * 0.5f) * Far * Aspect;
            float farTop = (float)System.Math.Tan(Fov * 0.5f) * Far;
            //float nearLeft = -(float)Math.Tan(Fov * 0.5f * Aspect) * Near;
            float nearLeft = -(float)System.Math.Tan(Fov * 0.5f) * Near * Aspect;
            float nearTop = (float)System.Math.Tan(Fov * 0.5f) * Near;

            LeftTopFar = new Vector3(farLeft, farTop, -Far);
            LeftBottomFar = new Vector3(farLeft, -farTop, -Far);
            RightTopFar = new Vector3(-farLeft, farTop, -Far);
            RightBottomFar = new Vector3(-farLeft, -farTop, -Far);

            LeftTopNear = new Vector3(nearLeft, nearTop, -Near);
            LeftBottomNear = new Vector3(nearLeft, -nearTop, -Near);
            RightTopNear = new Vector3(-nearLeft, nearTop, -Near);
            RightBottomNear = new Vector3(-nearLeft, -nearTop, -Near);
        }

        public Vector3 Direction; // 카메라가 바라보는 방향(단위벡터)
        public Vector3 Right; // 카메라의 오른쪽 방향 벡터(단위벡터)
        public Vector3 Up; // 카메라의 위쪽 벡터(단위 벡터)
        public float Fov = MathHelper.PiOver4; // 카메라의 시야각(45도로 초기 설정, 라디안)
        public float Aspect; // 카메라의 가로, 세로 비율(종횡비)
        public float Near = 1.0f; // 카메라에서 근평면까지의 거리 값
        public float Far = 1000.0f; // 카메라에서 원평면까지의 거리 값

        public Vector3 LeftTopFar, LeftBottomFar, RightTopFar, RightBottomFar;
        public Vector3 LeftTopNear, LeftBottomNear, RightTopNear, RightBottomNear;
    }

    public class Camera
    {
        private Vector3 mPosition;
        private CameraData mCamData;
        private Vector3 mLookAt; // 카메라가 바라보는 위치
        private Matrix4 mViewMatrix; // 카메라의 행렬
        private Matrix4 mProjMatrix; // 투영 행렬
        private float mDefaultPos2LookAt;
        private Frustum mFrustum;
        private float Epsilon { get { return 0.0001f; } }

        public Vector3 Position { get { return mPosition; } }

        public Vector3 LookAt { get { return mLookAt; } }

        public Vector3 Up { get { return mCamData.Up; } }

        public Matrix4 ViewMatrix { get { return mViewMatrix; } }

        public Matrix4 ProjMatrix { get { return mProjMatrix; } }

        public Vector3 Right { get { return mCamData.Right; } }

        public float Fov { get { return mCamData.Fov; } }

        public float Far { get { return mCamData.Far; } }

        public float Near { get { return mCamData.Near; } }

        public float MaxPositionLimit { get; set; }

        public Camera(CameraInfo info)
        {
            mPosition = info.Position;
            mLookAt = info.LookAt;

            mCamData = new CameraData();
            mCamData.Fov = info.Fov;
            mCamData.Up = Vector3.UnitZ;
            mCamData.Far = info.Far;
            mCamData.Near = info.Near;
            mDefaultPos2LookAt = (mPosition - mLookAt).Length;

            mFrustum = new Frustum(mCamData);

            mProjMatrix = mCamData.CreateProjMatrix(info.Width, info.Height);
            
            MaxPositionLimit = 10000.0f;

            CalculateViewMatrix();
        }
        public void ResizeViewport(float width, float height)
        {
            mProjMatrix = mCamData.CreateProjMatrix(width, height);
        }
        /// <summary>
        /// 카메라 View를 적용합니다.
        /// </summary>
        public void LoadViewMatrix()
        {
            GL.LoadMatrix(ref mViewMatrix);
        }
        /// <summary>
        /// 카메라를 v값만큼 이동시킵니다.
        /// </summary>
        /// <param name="v">이동벡터값</param>
        public void Move(Vector3 v)
        {
            mPosition += v;
            mLookAt += v;

            //
            CalculateViewMatrix();
        }
        /// <summary>
        /// 카메라를 pos위치로 이동시킵니다.
        /// </summary>
        /// <param name="pos"></param>
        public void MoveAt(Vector3 pos)
        {
            mPosition += (pos - mLookAt);
            mLookAt = pos;
            CalculateViewMatrix();
        }
        /// <summary>
        /// 화면 기준 카메라를 오른쪽(음수:왼쪽)으로 이동시킵니다.
        /// </summary>
        /// <param name="f">이동량</param>
        public void MoveRight(float f)
        {
            float factor = (mPosition - mLookAt).Length * f / mDefaultPos2LookAt;
            mPosition += (mCamData.Right * factor);
            mLookAt += (mCamData.Right * factor);

            CalculateViewMatrix();
        }
        public void MoveUp(float f)
        {
            float factor = (mPosition - mLookAt).Length * f / mDefaultPos2LookAt;
            mPosition += (Vector3.UnitZ * factor);
            mLookAt += (Vector3.UnitZ * factor);

            CalculateViewMatrix();
        }
        /// <summary>
        /// 화면 기준 카메라를 앞(음수:뒤)으로 이동시킵니다.
        /// </summary>
        /// <param name="f"></param>
        public void MoveFront(float f)
        {
            Vector3 look2Pos = new Vector3(mLookAt - mPosition);

            float factor = look2Pos.Length * f / mDefaultPos2LookAt;
            look2Pos.Z = 0.0f;
            look2Pos.Normalize();

            mPosition += (look2Pos * factor);
            mLookAt += (look2Pos * factor);

            CalculateViewMatrix();
        }
        /// <summary>
        /// 카메라를 LookAt방향으로 이동 시킵니다.
        /// </summary>
        /// <param name="f">이동량</param>
        public void Zoom(float f)
        {
            Vector3 look2Pos = new Vector3(mPosition - mLookAt);

            if (f > 0)
            {
                // 본 인덱스 보려면 근접할 필요가 있어 매우 가깝게 접근 가능하게 함
                while (look2Pos.Length < f + mCamData.Near)
                {
                    f -= 0.001f;
                }

                if (f <= 0)
                {
                    return;
                }
            }

            mPosition += mCamData.Direction * f * ZoomSpeed;
            //
            CalculateViewMatrix();
        }
        /// <summary>
        /// 카메라를 회전시킵니다.
        /// </summary>
        /// <param name="angle">회전값(x, y)</param>
        public void Rotate(Vector2 angle)
        {
            // Right Vector를 축으로 회전
            Vector3 look2Pos = new Vector3(mPosition - mLookAt);
            float length = look2Pos.Length;

            Matrix4 matAxisRight = new Matrix4();
            Vector3 rightAxis = mCamData.Right;

            Matrix4.CreateFromAxisAngle(rightAxis, -angle.Y, out matAxisRight);
            look2Pos.Normalize();
            look2Pos = Vector3.TransformPosition(look2Pos, matAxisRight);
            mPosition = (look2Pos * length) + mLookAt;

            CalculateViewMatrix();

            // Up Vector를 축으로 회전
            look2Pos = new Vector3(mPosition - mLookAt);
            length = look2Pos.Length;

            Matrix4 matAxisUp = new Matrix4();
            Vector3 upAxis = mCamData.Up;

            Matrix4.CreateFromAxisAngle(upAxis, angle.X, out matAxisUp);
            look2Pos.Normalize();
            look2Pos = Vector3.TransformPosition(look2Pos, matAxisUp);
            mPosition = (look2Pos * length) + mLookAt;

            // 
            mCamData.Up = Vector3.UnitZ;

            if (IsParallelWithUp(look2Pos))
            {
                // 움직이지 않는 게 제어에 더 낫다. 
                return;
            }

            CalculateViewMatrix();
        }

        public float ZoomSpeed
        {
            get { return 10; }
        }



        /// <summary>
        /// 카메라 행렬을 계산합니다.
        /// </summary>
        private void CalculateViewMatrix()
        {
            mCamData.CalcUpRightDirection(new Vector3(mLookAt - mPosition));

            mViewMatrix = Matrix4.LookAt(mPosition, mLookAt, mCamData.Up);

            mCamData.Setup8Points();
            mFrustum.CalcPlanes(ref mViewMatrix);
        }
        private bool IsParallelWithUp(Vector3 dir)
        {
            // 회전 시 UP 벡터와 방향이 평행하면 외적 값이 0이 된다. 
            float diff = System.Math.Abs(Vector3.Dot(dir, mCamData.Up)) - 1;
            return System.Math.Abs(diff) < Epsilon;
        }


        
    }
}
