using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using SuperMarioBros.PhysicComponent;

namespace SuperMarioBros.DisplayComponent
{
    public class Mario : Character
    {
        private bool mDoJump;
        private KeyboardState mOldKeyboardState;
        private Vector2 mLastGoodPosition;
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
            mDoJump = false;

            mSpriteSize = new Point(16, 32);
            mHorizontalSpeed = new Speed(100);
            mHorizontalSpeed.mAcceleration = 10;
            mVerticalSpeed = new Speed(300);
            mVerticalSpeed.mAcceleration = 10;
            mVerticalSpeed.mNegativeSpeed = true;

            mSize = new Vector2(16, 32);

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

        public void Update(List<DrawableObstacle> arrayObstacle, GameTime gameTime)
        {
            CheckInput();
            if (gameTime.ElapsedGameTime.Milliseconds != 0)
            {
                mMovementInPixel = new Vector2(mHorizontalSpeed.mCurrentSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f, mVerticalSpeed.mCurrentSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            }
            SetTimeBetweenAnimation((50 * mHorizontalSpeed.mSpeedLimit) / mHorizontalSpeed.mCurrentSpeed);
            CollisionDetection(arrayObstacle);
            if (mDoJump)
            {
                mDoJump = false;
                mVerticalSpeed.SpeedToMax();
                mMovementInPixel.Y = mVerticalSpeed.mCurrentSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                mIsFalling = true;
            }
            if (mIsFalling)
            {
                mVerticalSpeed.SlowDown();
                mMoveVector = new Vector2(mMoveVector.X, -1.0f);
                mIndexDrawnSprite = (int)AnimationName.JUMP;
                if(mPosition.Y > 400.0f)
                {
                    mPosition = mLastGoodPosition;
                }
            }
            else if (mVerticalSpeed.mCurrentSpeed != 0.0f)
            {
                mVerticalSpeed.Stop();
                mMovementInPixel.Y = 0.0f;
                mMoveVector = new Vector2(mMoveVector.X, 0.0f);
            }
            else
            {
                mLastGoodPosition = mPosition;
            }
            base.Update(gameTime);
        }
        
        public void CollisionDetection(List<DrawableObstacle> arrayObstacle)
        {
            // On crée trois rayons qui partent du personnage.
            float xPos = mPosition.X;
            float bottomTestXPos = mPosition.X + mSize.X;

            if(mMoveVector.X == 1.0f)
            {
                xPos += mSize.X;
                bottomTestXPos -= mSize.X;
            }

            Ray2D[] rayForward = new Ray2D[3];
            rayForward[0] = new Ray2D(new Vector2(xPos, mPosition.Y), new Vector2(mMoveVector.X, 0.0f));
            rayForward[1] = new Ray2D(new Vector2(xPos, mPosition.Y + mSize.Y * 0.5f), new Vector2(mMoveVector.X, 0.0f));
            rayForward[2] = new Ray2D(new Vector2(xPos, mPosition.Y + mSize.Y), new Vector2(mMoveVector.X, 0.0f));

            Ray2D[] rayVerticalTests = new Ray2D[3];
            float verticalPosition = (mVerticalSpeed.mCurrentSpeed > 0) ? mPosition.Y : mPosition.Y + mSize.Y;
            float rayDirection = (mVerticalSpeed.mCurrentSpeed > 0) ? -1.0f : 1.0f;
            rayVerticalTests[0] = new Ray2D(new Vector2(mPosition.X, verticalPosition), new Vector2(0.0f, rayDirection));
            rayVerticalTests[1] = new Ray2D(new Vector2(mPosition.X + mSize.X, verticalPosition), new Vector2(0.0f, rayDirection));
            rayVerticalTests[2] = new Ray2D(new Vector2(mPosition.X + mSize.X * 0.5f, verticalPosition), new Vector2(0.0f, rayDirection));

            foreach (Obstacle obst in arrayObstacle)
            {
                for (int i = 0; i < rayForward.Length; ++i)
                {
                    obst.Intersect(ref rayForward[i]);
                }

                for (int i = 0; i < rayVerticalTests.Length; ++i)
                {
                    obst.Intersect(ref rayVerticalTests[i]);
                }
            }

            // Collision mouvement horizontal
            float nextHorizontalMovement = System.Math.Abs(mMovementInPixel.X) + 1;
            List<Ray2D> rayUsed = new List<Ray2D>();
            foreach (Ray2D r in rayForward)
            {
                if (r.IsIntersectionFound() && !float.IsNaN(r.mIntersectionDistance))
                {
                    rayUsed.Add(r);
                }
            }
            
            if (rayUsed.Count > 0)
            {
                float lowestDistanceCollision = rayUsed[0].mIntersectionDistance;
                for (int i = 1; i < rayUsed.Count; ++i)
                {
                    lowestDistanceCollision = MathHelper.Min(lowestDistanceCollision, rayUsed[i].mIntersectionDistance);
                }
                if (lowestDistanceCollision == 0.0f)
                {
                    mMovementInPixel.X = 0;
                }
                else if (lowestDistanceCollision < System.Math.Abs(mMovementInPixel.X))
                {
                    mMovementInPixel.X = System.Math.Sign(mMovementInPixel.X) * lowestDistanceCollision;
                }
            }

            // Collision mouvement vertical
            mIsFalling = true;
            rayUsed.Clear();
            foreach (Ray2D r in rayVerticalTests)
            {
                if (r.IsIntersectionFound() && !float.IsNaN(r.mIntersectionDistance))
                {
                    rayUsed.Add(r);
                }
            }
            if(rayUsed.Count > 0)
            {
                float lowestDistanceCollision = rayUsed[0].mIntersectionDistance;
                int idObstacleCollided = rayUsed[0].mIdObstacleIntersected;
                for (int i = 1; i < rayUsed.Count; ++i)
                {
                    if(lowestDistanceCollision > rayUsed[i].mIntersectionDistance)
                    {
                        lowestDistanceCollision = rayUsed[i].mIntersectionDistance;
                        idObstacleCollided = rayUsed[i].mIdObstacleIntersected;
                    }
                }
                if (lowestDistanceCollision == 0.0f)
                {
                    mIsFalling = false;
                    if (mVerticalSpeed.mCurrentSpeed > 0.0f)
                    {
                        arrayObstacle[idObstacleCollided - 1].CollisionEffect();
                    }
                }
                else if (lowestDistanceCollision < System.Math.Abs(mMovementInPixel.Y))
                {
                    mMovementInPixel.Y = System.Math.Sign(mMovementInPixel.Y) * lowestDistanceCollision;
                }
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
                if (mMoveVector.X == -1.0f)
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
            else if(state.IsKeyUp(Keys.LeftShift) && mOldKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                // Stop Running 
                mHorizontalSpeed.SpeedLimitToNormal();
            }

            if (state.IsKeyDown(Keys.Space))
            {
                if (!mIsFalling && mOldKeyboardState.IsKeyUp(Keys.Space))
                {
                    mDoJump = true;
                }
            }
            if (state.IsKeyDown(Keys.S) || state.IsKeyDown(Keys.Down))
            {
                mIndexDrawnSprite = (int)AnimationName.CROUCH;
            }
            mOldKeyboardState = state;
        }

    }
}
