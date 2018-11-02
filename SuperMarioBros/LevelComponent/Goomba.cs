using SuperMarioBros.DisplayComponent;
using SuperMarioBros.PhysicComponent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SuperMarioBros.LevelComponent
{
    public class Goomba : Enemy
    {
        private int mCrushedTimer;
        public Goomba(Point position)
            :base(position)
        {
            mPrimaryEvent = base.AlwaysMove;
            mCrushed = false;
            mHorizontalSpeed = new Speed(50);
            mHorizontalSpeed.mAllowNegativeSpeed = true;
            mHorizontalSpeed.SpeedToMax();
            mVerticalSpeed = new Speed(100);
            mVerticalSpeed.mAllowNegativeSpeed = true;
            mVerticalSpeed.mAcceleration = 10;
            SetBoundingBoxSize(new Vector2(16, 16));

            mSpriteAnimationStepNumber = new int[2];
            mSpriteAnimationStepNumber[(int)Enemy.EnemyState.WALK] = 2;
            mSpriteAnimationStepNumber[(int)Enemy.EnemyState.CRUSHED] = 1;

            mAnimationStartArray = new Rectangle[2];
            mAnimationStartArray[(int)Enemy.EnemyState.WALK] = new Rectangle(0, 0, 16, 16);
            mAnimationStartArray[(int)Enemy.EnemyState.CRUSHED] = new Rectangle(32, 0, 16, 16);
        }

        public override void Update(GameTime gameTime)
        {
            if (mCrushed)
            {
                if (mIsCollidable)
                {
                    SetCollidable(false);
                }
                mCrushedTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (mCrushedTimer > 1000)
                {
                    AddEventFront(this.DeleteObstacle);
                }
            }
            else
            {
                base.Update(gameTime);
            }
        }

        
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
