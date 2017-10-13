using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperMarioBros.PhysicComponent;
using SuperMarioBros.LevelComponent;
using System.Collections.Generic;

namespace SuperMarioBros.DisplayComponent
{
    public class Mario : MovableObstacle
    {
        private bool mDoJump;
        private bool mDoCrouch;
        public bool mIsDead;
        private KeyboardState mOldKeyboardState;
        public int mNbCoins;
        public int mScore;
        private MarioState _mMarioState;
        public int mNbLife;
        private List<Rectangle[]> mMarioSpritePosition;
        private List<int[]> mMarioAnimationStepNumber;

        private Point[] mMarioSpriteSize;

        // State transition animation variables
        private bool mChangeStateAnimation;
        private MarioStateTransition mNewState;
        private List<Rectangle[]> mMarioStateTransitionPosition;
        private int mTimerTransition;
        private int mCurrentTransitionStep;

        private Vector2 mLastGoodPosition;

        private AnimationName mRunningAnimationType;

        public MarioState mMarioState
        {
            get => _mMarioState;
            private set
            {
                _mMarioState = value;
                mAnimationStartArray = mMarioSpritePosition[(int)value];
                mSpriteSize = mMarioSpriteSize[(int)value];
                mDrawnRectangle = mAnimationStartArray[mIndexDrawnSprite];
                mSize = new Vector2(mSpriteSize.X, mSpriteSize.Y);
            }
        }


        enum AnimationName
        {
            IDLE = 0,
            RUN,
            SLOWDOWN,
            JUMP,
            CROUCH, 
            NBANIMATION
        }

        public enum MarioState
        {
            SMALL = 0, 
            BIG, 
            FLOWER,
            DEAD,
            NBSTATES
        }

        private enum MarioStateTransition
        {
            SMALL2BIG,
            BIG2FLOWER,
            HIGHER2SMALL,
            NBTRANSITIONS
        }

        public Mario()
        {
            mMarioAnimationStepNumber = new List<int[]>();
            mMarioAnimationStepNumber.Add(new int[(int)AnimationName.NBANIMATION]);
            mMarioAnimationStepNumber[0][(int)AnimationName.IDLE] = 1;
            mMarioAnimationStepNumber[0][(int)AnimationName.RUN] = 3;
            mMarioAnimationStepNumber[0][(int)AnimationName.SLOWDOWN] = 1;
            mMarioAnimationStepNumber[0][(int)AnimationName.JUMP] = 1;
            mMarioAnimationStepNumber[0][(int)AnimationName.CROUCH] = 1;

            mMarioAnimationStepNumber.Add(new int[(int)MarioStateTransition.NBTRANSITIONS]);
            mMarioAnimationStepNumber[1][(int)MarioStateTransition.BIG2FLOWER] = 10;
            mMarioAnimationStepNumber[1][(int)MarioStateTransition.HIGHER2SMALL] = 2;
            mMarioAnimationStepNumber[1][(int)MarioStateTransition.SMALL2BIG] = 2;

            mSpriteAnimationStepNumber = mMarioAnimationStepNumber[0];

            mMarioSpritePosition = new List<Rectangle[]>();
            mMarioSpriteSize = new Point[(int)MarioState.NBSTATES];

            mMarioSpritePosition.Add(new Rectangle[(int)AnimationName.NBANIMATION]);
            mMarioSpriteSize[(int)MarioState.SMALL] = new Point(16, 16);
            mMarioSpritePosition[(int)MarioState.SMALL][(int)AnimationName.IDLE] = new Rectangle(new Point(80, 34), mMarioSpriteSize[(int)MarioState.SMALL]);
            mMarioSpritePosition[(int)MarioState.SMALL][(int)AnimationName.RUN] = new Rectangle(new Point(97, 34), mMarioSpriteSize[(int)MarioState.SMALL]);
            mMarioSpritePosition[(int)MarioState.SMALL][(int)AnimationName.SLOWDOWN] = new Rectangle(new Point(148, 34), mMarioSpriteSize[(int)MarioState.SMALL]);
            mMarioSpritePosition[(int)MarioState.SMALL][(int)AnimationName.JUMP] = new Rectangle(new Point(165, 34), mMarioSpriteSize[(int)MarioState.SMALL]);

            mMarioSpriteSize[(int)MarioState.BIG] = new Point(16, 32);
            mMarioSpritePosition.Add(new Rectangle[(int)AnimationName.NBANIMATION]);
            mMarioSpritePosition[(int)MarioState.BIG][(int)AnimationName.IDLE] = new Rectangle(new Point(80, 1), mMarioSpriteSize[(int)MarioState.BIG]);
            mMarioSpritePosition[(int)MarioState.BIG][(int)AnimationName.RUN] = new Rectangle(new Point(97, 1), mMarioSpriteSize[(int)MarioState.BIG]);
            mMarioSpritePosition[(int)MarioState.BIG][(int)AnimationName.SLOWDOWN] = new Rectangle(new Point(148, 1), mMarioSpriteSize[(int)MarioState.BIG]);
            mMarioSpritePosition[(int)MarioState.BIG][(int)AnimationName.JUMP] = new Rectangle(new Point(165, 1), mMarioSpriteSize[(int)MarioState.BIG]);
            mMarioSpritePosition[(int)MarioState.BIG][(int)AnimationName.CROUCH] = new Rectangle(new Point(182, 1), mMarioSpriteSize[(int)MarioState.BIG]);

            mMarioSpritePosition.Add(new Rectangle[(int)AnimationName.NBANIMATION]);
            mMarioSpriteSize[(int)MarioState.FLOWER] = new Point(16, 32);
            mMarioSpritePosition[(int)MarioState.FLOWER][(int)AnimationName.IDLE] = new Rectangle(new Point(80, 129), mMarioSpriteSize[(int)MarioState.FLOWER]);
            mMarioSpritePosition[(int)MarioState.FLOWER][(int)AnimationName.RUN] = new Rectangle(new Point(97, 129), mMarioSpriteSize[(int)MarioState.FLOWER]);
            mMarioSpritePosition[(int)MarioState.FLOWER][(int)AnimationName.SLOWDOWN] = new Rectangle(new Point(148, 129), mMarioSpriteSize[(int)MarioState.FLOWER]);
            mMarioSpritePosition[(int)MarioState.FLOWER][(int)AnimationName.JUMP] = new Rectangle(new Point(165, 129), mMarioSpriteSize[(int)MarioState.FLOWER]);
            mMarioSpritePosition[(int)MarioState.FLOWER][(int)AnimationName.CROUCH] = new Rectangle(new Point(182, 129), mMarioSpriteSize[(int)MarioState.FLOWER]);


            mMarioSpriteSize[(int)MarioState.DEAD] = new Point(16, 16);
            mMarioSpritePosition.Add(new Rectangle[1]);
            mMarioSpritePosition[(int)MarioState.DEAD][(int)AnimationName.IDLE] = new Rectangle(new Point(182, 34), mMarioSpriteSize[(int)MarioState.SMALL]);


            mMarioStateTransitionPosition = new List<Rectangle[]>();
            mMarioStateTransitionPosition.Add(new Rectangle[mMarioAnimationStepNumber[1][(int)MarioStateTransition.SMALL2BIG]]);
            mMarioStateTransitionPosition.Add(new Rectangle[mMarioAnimationStepNumber[1][(int)MarioStateTransition.BIG2FLOWER]]);
            mMarioStateTransitionPosition.Add(new Rectangle[mMarioAnimationStepNumber[1][(int)MarioStateTransition.HIGHER2SMALL]]);

            mMarioStateTransitionPosition[(int)MarioStateTransition.SMALL2BIG][0] = new Rectangle(new Point(437, 1), mMarioSpriteSize[(int)MarioState.BIG]);
            mMarioStateTransitionPosition[(int)MarioStateTransition.SMALL2BIG][1] = new Rectangle(new Point(335, 1), mMarioSpriteSize[(int)MarioState.BIG]);

            
            mMarioStateTransitionPosition[(int)MarioStateTransition.BIG2FLOWER][0] = new Rectangle(new Point(80, 1), mMarioSpriteSize[(int)MarioState.BIG]);
            int yPos = 66;
            for (int i = 0; i< mMarioAnimationStepNumber[1][(int)MarioStateTransition.BIG2FLOWER]; ++i, yPos += 63)
            {
                mMarioStateTransitionPosition[(int)MarioStateTransition.BIG2FLOWER][i] = new Rectangle(new Point(80, yPos), mMarioSpriteSize[(int)MarioState.BIG]);                
            }

            mIsAnimated = true;
            mDoJump = false;
            mMarioState = MarioState.SMALL;
            mNbCoins = 0;
            mScore = 0;
            mMoveVector = new Vector2(1.0f, -1.0f);
            mNbLife = 3;

            mAnimationOffset = 1;

            mHorizontalSpeed = new Speed(100);
            mHorizontalSpeed.mAcceleration = 2;
            mHorizontalSpeed.mAllowNegativeSpeed = true;
            mVerticalSpeed = new Speed(400);
            mVerticalSpeed.mAcceleration = 50;
            mVerticalSpeed.mAllowNegativeSpeed = true;

            mDrawnRectangle = mAnimationStartArray[(int)AnimationName.IDLE];
        }

        public void Restart()
        {
            mIsDead = false;
            mPosition = mLastGoodPosition;
            mMarioState = MarioState.SMALL;
            ObstacleAccessor.Instance.Add(this);
        }

        public void LifeUp()
        {
            mNbLife++;
        }

        public void LifeDown()
        {
            mNbLife--;
        }

        public void StateUp()
        {
            mChangeStateAnimation = true;
            mTimerTransition = 1000;
            mCurrentTransitionStep = 0;
            if (mMarioState == MarioState.SMALL)
            {
            mPosition = new Vector2(mPosition.X, mPosition.Y - 16.0f);
                mNewState = MarioStateTransition.SMALL2BIG;
            }
            else if(mMarioState == MarioState.BIG)
            {
                mNewState = MarioStateTransition.BIG2FLOWER;
            }
        }

        public void MarioGotHit()
        {
            if(mMarioState == MarioState.SMALL)
            {
                mIsCollidable = false;
                mIndexDrawnSprite = (int)AnimationName.IDLE; 
                mVerticalSpeed.SpeedToMax();
                mHorizontalSpeed.Stop();
                LifeDown();
                mMarioState = MarioState.DEAD;
            }
            else
            {
                mChangeStateAnimation = true;
                mCurrentTransitionStep = 0;
                mTimerTransition = 1000;
                mNewState = MarioStateTransition.HIGHER2SMALL;
            }
        }

        public void CoinUp()
        {
            mNbCoins++;
            mScore += 200;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content, GraphicsDeviceManager graphics)
        {
            mSpriteSheet = content.Load<Texture2D>("MarioSpriteSheet");
            mIndexDrawnSprite = (int)AnimationName.JUMP;
        }

        public override void CollisionEffect(Obstacle obst, CollisionWay way)
        {
            if (obst.mIsCollidable && mIsCollidable)
            {
                if (way == CollisionWay.ABOVE || way == CollisionWay.BELOW)
                {
                    mVerticalSpeed.Stop();
                }
                else
                {
                    mHorizontalSpeed.Stop();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (mMarioState == MarioState.DEAD)
            {
                if (gameTime.ElapsedGameTime.Milliseconds != 0)
                {
                    mMovementInPixel = new Vector2(mHorizontalSpeed.mCurrentSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f, mVerticalSpeed.mCurrentSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                }
                mVerticalSpeed.SlowDown();
                if(mPosition.Y > 600)
                {
                    mIsDead = true;
                }
                base.Update(gameTime);
            }
            else
            {
                if (mChangeStateAnimation)
                {
                    if (mTimerTransition > 0)
                    {
                        mMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                        if (mMilliseconds > 40)
                        {
                            mMilliseconds = 0;
                            mCurrentTransitionStep++;
                            if (mCurrentTransitionStep >= mMarioAnimationStepNumber[1][(int)mNewState])
                            {
                                mCurrentTransitionStep = 0;
                            }
                            mDrawnRectangle = mMarioStateTransitionPosition[(int)mNewState][mCurrentTransitionStep];
                        }
                        mTimerTransition -= gameTime.ElapsedGameTime.Milliseconds;
                    }
                    else
                    {
                        mChangeStateAnimation = false;
                        switch (mNewState)
                        {
                            case MarioStateTransition.SMALL2BIG:
                                mMarioState = MarioState.BIG;
                                mDrawnRectangle = mAnimationStartArray[mIndexDrawnSprite];
                                break;
                            case MarioStateTransition.BIG2FLOWER:
                                mMarioState = MarioState.FLOWER;
                                mDrawnRectangle = mAnimationStartArray[mIndexDrawnSprite];
                                break;
                        }
                    }
                }
                else
                {
                    CheckInput();
                    if (gameTime.ElapsedGameTime.Milliseconds != 0)
                    {
                        mMovementInPixel = new Vector2(mHorizontalSpeed.mCurrentSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f, mVerticalSpeed.mCurrentSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                    }
                    SetTimeBetweenAnimation((50 * mHorizontalSpeed.mSpeedLimit) / System.Math.Abs(mHorizontalSpeed.mCurrentSpeed));
                    CollisionDetection();
                    if(mMarioState != MarioState.DEAD)
                    {
                        if (mDoJump)
                        {
                            mDoJump = false;
                            mVerticalSpeed.SpeedToMax();
                            mVerticalSpeed.mAcceleration = 50;
                            mMovementInPixel.Y = mVerticalSpeed.mCurrentSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                            mIsFalling = true;
                        }
                        if (mIsFalling)
                        {
                            mVerticalSpeed.SlowDown();
                            mIndexDrawnSprite = (int)AnimationName.JUMP;
                        }
                        else
                        {
                            mLastGoodPosition = mPosition;
                        }
                        DefineCurrentAnimation();
                        base.Update(gameTime);
                    }                    
                }
            }
            
        }

        public void DefineCurrentAnimation()
        {
            if(mIsFalling)
            {
                mIndexDrawnSprite = (int)AnimationName.JUMP;
            }
            else if(mDoCrouch && mMarioState != MarioState.SMALL)
            {
                mIndexDrawnSprite = (int)AnimationName.CROUCH;
            }
            else
            {
                if (mHorizontalSpeed.mCurrentSpeed == 0.0f)
                {
                    mIndexDrawnSprite = (int)AnimationName.IDLE;
                }
                else
                {
                    mIndexDrawnSprite = (int)mRunningAnimationType;
                    if(mRunningAnimationType != AnimationName.SLOWDOWN)
                    {
                        if (mHorizontalSpeed.mCurrentSpeed > 0.0f)
                        {
                            TextureFaceRight();
                        }
                        else
                        {
                            TextureFaceLeft();
                        }
                    }
                    else
                    {
                        if (mHorizontalSpeed.mCurrentSpeed < 0.0f)
                        {
                            TextureFaceRight();
                        }
                        else
                        {
                            TextureFaceLeft();
                        }
                    }
                    
                }
            }
        }
        

        public void CheckInput()
        {
            KeyboardState state = Keyboard.GetState();

            mDoCrouch = false;
            bool moveButtonPressed = false;
            // Go Left
            if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.Q))
            {
                moveButtonPressed = true;
                if (mHorizontalSpeed.mEvolveInPositiveNumber)
                {
                    if (mHorizontalSpeed.mCurrentSpeed <= 0.0f)
                    {
                        mHorizontalSpeed.mEvolveInPositiveNumber = false;
                    }
                    else
                    {
                        mHorizontalSpeed.SlowDown();
                        mRunningAnimationType = AnimationName.SLOWDOWN;
                    }
                }
                else
                {
                    mHorizontalSpeed.SpeedUp();
                    mRunningAnimationType = AnimationName.RUN;
                }

            }
            // Go Right
            else if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
            {
                moveButtonPressed = true;
                if (!mHorizontalSpeed.mEvolveInPositiveNumber)
                {
                    if (mHorizontalSpeed.mCurrentSpeed >= 0.0f)
                    {
                        mHorizontalSpeed.mEvolveInPositiveNumber = true;
                    }
                    else
                    {
                        mHorizontalSpeed.SlowDown();
                        mRunningAnimationType = AnimationName.SLOWDOWN;
                    }
                }
                else
                {
                    mHorizontalSpeed.SpeedUp();
                    mRunningAnimationType = AnimationName.RUN;
                }
            }

            if (state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.S))
            {
                moveButtonPressed = false;
                mDoCrouch = true;
            }

            if (!moveButtonPressed)
            {
                mHorizontalSpeed.SlowDown();
                if (mHorizontalSpeed.mEvolveInPositiveNumber)
                {
                    if (mHorizontalSpeed.mCurrentSpeed <= 0.0f)
                    {
                        mHorizontalSpeed.Stop();
                    }
                }
                else
                {
                    if (mHorizontalSpeed.mCurrentSpeed >= 0.0f)
                    {
                        mHorizontalSpeed.Stop();
                    }
                }
            }
                
            if (state.IsKeyDown(Keys.LeftShift) && mOldKeyboardState.IsKeyUp(Keys.LeftShift))
            {
                // Run
                mHorizontalSpeed.DoubleSpeedLimit();
            }
            else if (state.IsKeyUp(Keys.LeftShift) && mOldKeyboardState.IsKeyDown(Keys.LeftShift))
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
                else if (mIsFalling && mOldKeyboardState.IsKeyDown(Keys.Space)) {
                    if (mVerticalSpeed.mAcceleration > 10 && mVerticalSpeed.mCurrentSpeed > 0.0f)
                    {
                        mVerticalSpeed.mAcceleration -= 10;
                    }
                }
            }
            
            mOldKeyboardState = state;
        }
    }
}
