using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using SuperMarioBros.PhysicComponent;

namespace SuperMarioBros.DisplayComponent
{
    public class Mario : Character
    {
        private bool mJumpPressed;
        public Camera mCamera;
        enum AnimationName
        {
            IDLE,
            RUN,
            SLOWDOWN,
            JUMP,
            CROUCH,
            NBLISTS
        }

        public Mario()
        {
            mIsAnimated = true;
            mCamera = new Camera();
            mJumpPressed = false; 

            mSpriteSize = new Point(16, 32);
            mHorizontalSpeed = new Speed(10);
            mHorizontalSpeed.mAcceleration = 1;
            mVerticalSpeed = new Speed(10);

            mSpriteAnimationStepNumber = new int[(int)AnimationName.NBLISTS];
            mSpriteAnimationStepNumber[(int)AnimationName.IDLE] = 1;
            mSpriteAnimationStepNumber[(int)AnimationName.RUN] = 3;
            mSpriteAnimationStepNumber[(int)AnimationName.SLOWDOWN] = 1;
            mSpriteAnimationStepNumber[(int)AnimationName.JUMP] = 1;
            mSpriteAnimationStepNumber[(int)AnimationName.CROUCH] = 1;

            mAnimationStartArray = new Rectangle[(int)AnimationName.NBLISTS];
            mAnimationStartArray[(int)AnimationName.IDLE] = new Rectangle(new Point(0, 0), mSpriteSize);
            mAnimationStartArray[(int)AnimationName.RUN] = new Rectangle(new Point(17, 0), mSpriteSize);
            mAnimationStartArray[(int)AnimationName.SLOWDOWN] = new Rectangle(new Point(68, 0), mSpriteSize);
            mAnimationStartArray[(int)AnimationName.JUMP] = new Rectangle(new Point(85, 0), mSpriteSize);
            mAnimationStartArray[(int)AnimationName.CROUCH] = new Rectangle(new Point(102, 0), mSpriteSize);

        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content, GraphicsDevice graphics)
        {
            mSpriteSheet = content.Load<Texture2D>("MarioSpriteSheet");
            mIndexDrawnSprite = (int)AnimationName.JUMP;
        }

        public new void Update(List<DrawableObstacle> arrayObstacle)
        {
            CheckInput();
            SetTimeBetweenAnimation((50 * mHorizontalSpeed.mSpeedLimit) / mHorizontalSpeed.mCurrentSpeed);
            CollisionDetection(arrayObstacle);
            base.Update();
        }
        
        public void CollisionDetection(List<DrawableObstacle> arrayObstacle)
        {
            bool noIntersection = true;
            foreach (Obstacle obst in arrayObstacle)
            {
                if ((mPosition - obst.mPosition).Length() < 2000.0f)
                {
                    if (Intersect(obst))
                    {
                        noIntersection = false;
                        mMoveVector = new Vector2(0.0f, 0.0f);
                    }
                }
            }
            if(noIntersection)
            {
                mIsFalling = true;
            }
        }

        public void CheckInput()
        {
            KeyboardState state = Keyboard.GetState();
            // Go Left
            if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.Q))
            {
                if (mMoveVector.X == 0.0f)
                {
                    mMoveVector = new Vector2(-1.0f, mMoveVector.Y);
                }
                else if (mMoveVector.X == 1.0f)
                {
                    mHorizontalSpeed.SlowDown();
                    if (mHorizontalSpeed.mCurrentSpeed <= 0.0f)
                    {
                        mMoveVector = new Vector2(-1.0f, mMoveVector.Y);
                    }
                    TextureFaceLeft();
                    mIndexDrawnSprite = (int)AnimationName.SLOWDOWN;
                }
                else
                {
                    TextureFaceLeft();
                    mHorizontalSpeed.SpeedUp();
                    mIndexDrawnSprite = (int)AnimationName.RUN;
                }
            }
            // Go Right
            else if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
            {
                if (mMoveVector.X == 0.0f)
                {
                    mMoveVector = new Vector2(1.0f, mMoveVector.Y);
                }
                else if (mMoveVector.X == -1.0f)
                {
                    mHorizontalSpeed.SlowDown();
                    if (mHorizontalSpeed.mCurrentSpeed <= 0.0f)
                    {
                        mMoveVector = new Vector2(1.0f, mMoveVector.Y);
                    }
                    TextureFaceRight();
                    mIndexDrawnSprite = (int)AnimationName.SLOWDOWN;
                }
                else
                {
                    TextureFaceRight();
                    mHorizontalSpeed.SpeedUp();
                    mIndexDrawnSprite = (int)AnimationName.RUN;
                }
            }
            else
            {
                mHorizontalSpeed.SlowDown();
                if (mHorizontalSpeed.mCurrentSpeed <= 0.0f)
                {
                    if (!mIsFalling)
                    {
                        mIndexDrawnSprite = (int)AnimationName.IDLE;
                    }
                    mMoveVector = new Vector2(0.0f, mMoveVector.Y);
                }
            }
            if (state.IsKeyDown(Keys.LeftShift))
            {
                // Run
                mHorizontalSpeed.DoubleSpeedLimit();
            }
            /*else if (state.IsKeyUp(Keys.LeftShift))
            {
                mHorizontalSpeed.SpeedLimitToNormal();
            }*/
            if (state.IsKeyDown(Keys.Space))
            {
                if (!mIsFalling)
                {
                    mJumpPressed = true;
                }
            }
            if (state.IsKeyDown(Keys.S) || state.IsKeyDown(Keys.Down))
            {
                mIndexDrawnSprite = (int)AnimationName.CROUCH;
            }
        }

    }
}
