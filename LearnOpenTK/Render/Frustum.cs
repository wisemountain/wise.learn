using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace LearnOpenTK.Render
{
    /// <summary>
    /// 각 평면의 Normal이 절두체의 안쪽으로 존재하도록 구성됩니다.
    /// Camera 전용으로 사용됩니다.
    /// </summary>
    public class Frustum
    {
        public Frustum(CameraData cam)
        {
            mCameraData = cam;
            mPlNear = new Plane();
            mPlFar = new Plane();
            mPlLeft = new Plane();
            mPlRight = new Plane();
            mPlTop = new Plane();
            mPlBottom = new Plane();
        }

        public void CalcPlanes(ref Matrix4 matView)
        {
            // todo : matView의 값이 nan이 들어왔을때 예외발생
            Matrix4 matInvert;
            Matrix4.Invert(ref matView, out matInvert);

            // Debug 모드에서 절두체를 렌더링하기위애 데이터를 저장합니다.
            mLeftTopFar = Vector3.TransformPosition(mCameraData.LeftTopFar, matInvert);
            mLeftBottomFar = Vector3.TransformPosition(mCameraData.LeftBottomFar, matInvert);
            mRightTopFar = Vector3.TransformPosition(mCameraData.RightTopFar, matInvert);
            mRightBottomFar = Vector3.TransformPosition(mCameraData.RightBottomFar, matInvert);

            mLeftTopNear = Vector3.TransformPosition(mCameraData.LeftTopNear, matInvert);
            mLeftBottomNear = Vector3.TransformPosition(mCameraData.LeftBottomNear, matInvert);
            mRightTopNear = Vector3.TransformPosition(mCameraData.RightTopNear, matInvert);
            mRightBottomNear = Vector3.TransformPosition(mCameraData.RightBottomNear, matInvert);

            mPlFar.FromPoints(ref mRightTopFar, ref mLeftTopFar, ref mRightBottomFar);
            mPlNear.FromPoints(ref mLeftTopNear, ref mRightTopNear, ref mRightBottomNear);
            mPlLeft.FromPoints(ref mLeftTopFar, ref mLeftTopNear, ref mLeftBottomNear);
            mPlRight.FromPoints(ref mRightTopNear, ref mRightTopFar, ref mRightBottomFar);
            mPlTop.FromPoints(ref mLeftTopFar, ref mRightTopFar, ref mRightTopNear);
            mPlBottom.FromPoints(ref mRightBottomNear, ref mLeftBottomNear, ref mLeftBottomFar);

            // normal test
            plNearCenterStart = new Vector3((mCameraData.LeftTopNear.X + mCameraData.RightTopNear.X) * 0.5f, (mCameraData.LeftTopNear.Y + mCameraData.LeftBottomNear.Y) * 0.5f, mCameraData.LeftTopNear.Z);
            plNearCenterStart = Vector3.TransformPosition(plNearCenterStart, matInvert);

            Matrix4 matTrans = Matrix4.CreateTranslation(mPlNear.Normal * 5.0f);
            plNearCenterEnd = Vector3.TransformPosition(plNearCenterStart, matTrans);
        }

        private Vector3 plNearCenterStart;
        private Vector3 plNearCenterEnd;
        private CameraData mCameraData;
        private Plane mPlNear, mPlFar, mPlLeft, mPlRight, mPlTop, mPlBottom;
        private Vector3 mLeftTopFar, mLeftBottomFar, mRightTopFar, mRightBottomFar;
        private Vector3 mLeftTopNear, mLeftBottomNear, mRightTopNear, mRightBottomNear;
    }
}