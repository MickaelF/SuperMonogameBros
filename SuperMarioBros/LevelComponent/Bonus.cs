using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SuperMarioBros.DisplayComponent;
using SuperMarioBros.PhysicComponent;

namespace SuperMarioBros.LevelComponent
{
    public class Bonus : MovableObstacle
    {
        private float mStartPositionY;

        public Bonus(Vector2 origin, Texture2D spriteSheet)
        {
            AddEventFront(PlayStartAnimation);
            mStartPositionY = origin.Y - 16.0f;
            mSpriteSheet = spriteSheet;

            mMoveVector = new Vector2(1.0f, -1.0f);
            mPosition = new Vector2(origin.X, origin.Y);
            mSize = new Vector2(16.0f, 16.0f);
            SetBoundingBoxSize(new Vector2(16, 16));
            mSpriteSize = new Point(16, 16);

        }

        protected bool PlayStartAnimation()
        {           
            if (mPosition.Y > mStartPositionY)
            {
                mMovementInPixel = new Vector2(0.0f, 20 * ObstacleAccessor.Instance.mGameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            }
            else
            {
                mMovementInPixel = new Vector2(0.0f, 0.0f);
                return true;
            }
            mPosition = new Vector2(mPosition.X, mPosition.Y + mMoveVector.Y * mMovementInPixel.Y);
            return false;
        }

    }
}
