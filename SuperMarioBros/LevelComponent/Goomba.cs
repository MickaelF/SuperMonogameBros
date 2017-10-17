using SuperMarioBros.DisplayComponent;
using SuperMarioBros.PhysicComponent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperMarioBros.LevelComponent
{
    public class Goomba : MovableObstacle
    {
        enum GoombaState
        {
            WALK,
            CRUSHED
        }
        private bool mCrushed;
        private int mCrushedTimer;
        public Goomba(Point position)
        {
            mPrimaryEvent = base.AlwaysMove;
            mCrushed = false;
            mHorizontalSpeed = new Speed(50);
            mHorizontalSpeed.mAllowNegativeSpeed = true;
            mHorizontalSpeed.SpeedToMax();
            mVerticalSpeed = new Speed(100);
            mVerticalSpeed.mAllowNegativeSpeed = true;
            mVerticalSpeed.mAcceleration = 10;
            mSize = new Vector2(16.0f, 16.0f);
            mSpriteSize = new Point(16, 16);
            SetBoundingBoxSize(new Vector2(16, 16));
            mPosition = new Vector2(position.X, position.Y);

            mSpriteAnimationStepNumber = new int[2];
            mSpriteAnimationStepNumber[(int)GoombaState.WALK] = 2;
            mSpriteAnimationStepNumber[(int)GoombaState.CRUSHED] = 1;

            mAnimationStartArray = new Rectangle[2];
            mAnimationStartArray[(int)GoombaState.WALK] = new Rectangle(0, 0, 16, 16);
            mAnimationStartArray[(int)GoombaState.CRUSHED] = new Rectangle(32, 0, 16, 16);

            mIsAnimated = true;
            SetTimeBetweenAnimation(50);
        }

        public override void Update(GameTime gameTime)
        {
            if (mCrushed)
            {
                mCrushedTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (mCrushedTimer > 1000)
                {
                    ObstacleAccessor.Instance.Remove(cId);
                }
            }
            else
            {
                base.Update(gameTime);
            }
        }

        public override void CollisionEffect(Obstacle obst, CollisionWay way)
        {
            if (obst is Mario)
            {
                switch(way)
                {
                    case CollisionWay.LEFT:
                    case CollisionWay.RIGHT:
                    case CollisionWay.BELOW:
                        (obst as Mario).MarioGotHit();
                        break;
                    case CollisionWay.ABOVE:
                        mCrushed = true;
                        mIndexDrawnSprite = (int)GoombaState.CRUSHED;
                        mIsCollidable = false;
                        (obst as Mario).AddEvent((obst as Mario).Jump);
                        break;
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
