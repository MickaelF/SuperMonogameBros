using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SuperMarioBros.DisplayComponent;
using SuperMarioBros.PhysicComponent;

namespace SuperMarioBros.LevelComponent
{
    public class FlowerBonus : DrawableObstacle
    {
        public FlowerBonus(Vector2 origin, Texture2D spriteSheet)
        {
            mSpriteSheet = spriteSheet;
            mPosition = new Vector2(origin.X, origin.Y - 16.0f);

            mSize = new Vector2(16.0f, 16.0f);
            mSpriteSize = new Point(16, 16);
            mDrawnRectangle = new Rectangle(new Point(80, 80), mSpriteSize);
            mAnimationStartArray = new Rectangle[1];
            mAnimationStartArray[0] = mDrawnRectangle;
            mIsAnimated = true;
            mSpriteAnimationStepNumber = new int[1];
            mSpriteAnimationStepNumber[0] = 3;
            SetTimeBetweenAnimation(50.0f);
        }

        public override void CollisionEffect(Obstacle obst, CollisionWay way)
        {
            if (obst is Mario)
            {
                (obst as Mario).StateUp();
                ObstacleAccessor.Instance.Remove(this);
            }
        }
    }
}
