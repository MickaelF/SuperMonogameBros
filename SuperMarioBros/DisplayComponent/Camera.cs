using Microsoft.Xna.Framework;

namespace SuperMarioBros.DisplayComponent
{
    public class Camera
    {
        public Vector2 mCenter { get; private set; }
        public float mZoom { get; private set; }
        public float mRotation { get; private set; }
        public Point mViewportSize;

        public Vector2 mViewportCenter
        {
            get
            {
                return new Vector2(mViewportSize.X * 0.5f, mViewportSize.Y * 0.5f);
            }
        }

        public Matrix mTranslationMatrix
        {
            get
            {
                return Matrix.CreateTranslation(-(int)mCenter.X,
                   -(int)mCenter.Y, 0) *
                   Matrix.CreateRotationZ(mRotation) *
                   Matrix.CreateScale(new Vector3(mZoom, mZoom, 1)) *
                   Matrix.CreateTranslation(new Vector3(mViewportCenter, 0));
            }
        }

        public Camera()
        {
            mZoom = 1.0f;
            mRotation = 0.0f;
        }

        public void ZoomUp(float val)
        {
            mZoom += val;
        }

        public void ZoomDown(float val)
        {
            mZoom = MathHelper.Max(mZoom - val, 0.25f);
        }

        public void Move(Vector2 move)
        {
            mCenter += move;
        }

        public Vector2 WorldToScreenPosition(Vector2 worldPos)
        {
            return Vector2.Transform(worldPos, mTranslationMatrix);
        }

        public Vector2 ScreenToWorldPosition(Vector2 screenPos)
        {
            return Vector2.Transform(screenPos, Matrix.Invert(mTranslationMatrix));
        }
        public void CenterOn(Vector2 center)
        {
            mCenter = center;
        }
    }
}
