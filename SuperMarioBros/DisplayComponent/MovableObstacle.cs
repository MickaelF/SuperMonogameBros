using Microsoft.Xna.Framework;
using SuperMarioBros.PhysicComponent;
using System.Collections.Generic;
namespace SuperMarioBros.DisplayComponent
{
    public class MovableObstacle : DrawableObstacle
    {
        //! Direction in which the obstacle will be able to move. Commonly (1.0, -1.0) (go right, go up)
        private Vector2 _mMoveVector;
        //! If the obstacle is able to move.
        protected bool mIsMovable;

        //! Boolean indicating if obstacle is already jumping
        private bool mIsJumping;
        //! Boolean indicating if obstacle is on the ground.
        private bool mIsOnGround;
        //! Vector2 of the position to go before stopping the jump
        protected Vector2 mTopLeftStopFallPosition;

        //! Encapsulation of _mMoveVector
        public Vector2 mMoveVector { get => _mMoveVector; set => _mMoveVector = value; }
        public Speed mVerticalSpeed;
        public Speed mHorizontalSpeed;

        public Vector2 mMovementInPixel;
        
        private bool _mVerticalCollision;
        public bool mVerticalCollision { get => _mVerticalCollision; set => _mVerticalCollision = value; }
        private bool _mHorizontalCollision;
        public bool mHorizontalCollision { get => _mHorizontalCollision; set => _mHorizontalCollision = value; }

        public MovableObstacle ()
        {
            mMovementInPixel = new Vector2(0.0f, 0.0f);
            mMoveVector = new Vector2(1.0f, -1.0f);
            mHorizontalSpeed = new Speed(0);
            mVerticalSpeed = new Speed(0);
            mIsMovable = true;
            mIsJumping = false;
            mIsOnGround = false;
        }

        public bool Move()
        {          
            if (mIsMovable)
            {
                GameTime gt = ObstacleAccessor.Instance.mGameTime;
                if (gt.ElapsedGameTime.Milliseconds != 0)
                {
                    mMovementInPixel = new Vector2(mHorizontalSpeed.mCurrentSpeed * gt.ElapsedGameTime.Milliseconds / 1000.0f, mVerticalSpeed.mCurrentSpeed * gt.ElapsedGameTime.Milliseconds / 1000.0f);
                }
                CollisionDetection();
                if (mHorizontalCollision)
                {
                    mHorizontalSpeed.Stop();
                }

                if (!mVerticalCollision)
                {
                    mVerticalSpeed.SlowDown();
                }
                else
                {
                    if (mMovementInPixel.Y > 0.0)
                    {
                        mVerticalSpeed.Stop();
                        mVerticalSpeed.SlowDown();
                    }
                }
            }
            return false;
        }

        public bool AlwaysMove()
        {
            if (mIsMovable)
            {
                GameTime gt = ObstacleAccessor.Instance.mGameTime;
                if (gt.ElapsedGameTime.Milliseconds != 0)
                {
                    mMovementInPixel = new Vector2(mHorizontalSpeed.mCurrentSpeed * gt.ElapsedGameTime.Milliseconds / 1000.0f, mVerticalSpeed.mCurrentSpeed * gt.ElapsedGameTime.Milliseconds / 1000.0f);
                }
                CollisionDetection();
                if (mHorizontalCollision)
                {
                    mHorizontalSpeed.mEvolveInPositiveNumber = !mHorizontalSpeed.mEvolveInPositiveNumber;
                    mHorizontalSpeed.SpeedToMax();
                }
                if (mVerticalCollision)
                {
                    mVerticalSpeed.Stop();
                }
                else
                {
                    mVerticalSpeed.SlowDown();
                }
            }
            return false;
        }

        public bool Jump()
        {
            mVerticalSpeed.SpeedToMax();
            mVerticalSpeed.mAcceleration = 50;
            return true;
        }

        public bool JumpUntilPosition()
        {
            if (mTopLeftStopFallPosition != null)
            {
                if(!mIsJumping)
                {
                    mVerticalSpeed.SpeedToMax();
                    mVerticalSpeed.mAcceleration = 10;
                    mIsJumping = true;
                    return false;
                }
                else
                {
                    GameTime gt = ObstacleAccessor.Instance.mGameTime;
                    if (gt.ElapsedGameTime.Milliseconds != 0)
                    {
                        mMovementInPixel = new Vector2(mHorizontalSpeed.mCurrentSpeed * gt.ElapsedGameTime.Milliseconds / 1000.0f, mVerticalSpeed.mCurrentSpeed * gt.ElapsedGameTime.Milliseconds / 1000.0f);
                    }
                    mPosition = new Vector2(mPosition.X + mMoveVector.X * mMovementInPixel.X, mPosition.Y + mMoveVector.Y * mMovementInPixel.Y);
                    mVerticalSpeed.SlowDown();
                    float lengthLeft = (mPosition - mTopLeftStopFallPosition).Length();
                    if (lengthLeft < System.Math.Abs(mMovementInPixel.Y))
                    {
                        mMovementInPixel.Y = System.Math.Sign(mMovementInPixel.Y) * lengthLeft;
                    }
                    if (lengthLeft == 0.0f)
                    {
                        mIsJumping = false;
                        return true;
                    }
                    return false;
                }
            }
            return true;
        }

        public void CollisionDetection()
        {
            mVerticalCollision = false;
            mHorizontalCollision = false;
            if (mIsCollidable)
            {   
                if (mMovementInPixel.X != 0.0f)
                {
                    mPosition = new Vector2(mPosition.X + mMoveVector.X * mMovementInPixel.X, mPosition.Y);
                    FRectangle rect = new FRectangle();
                    foreach(Obstacle o in ObstacleAccessor.Instance.mObstacleList.Values)
                    {
                        if (o != this)
                        {
                            if (Intersect(o, ref rect))
                            {
                                mHorizontalCollision = true;
                                mPosition = new Vector2(mPosition.X + mMoveVector.X * rect.mSize.X * System.Math.Sign(-mMovementInPixel.X), mPosition.Y);
                                this.CollisionEffect(o, (mMovementInPixel.X > 0.0f) ? CollisionWay.LEFT : CollisionWay.RIGHT);
                                o.CollisionEffect(this, (mMovementInPixel.X > 0.0f) ? CollisionWay.RIGHT : CollisionWay.LEFT);
                                break;
                            }
                        }
                    }
                }
                if (mMovementInPixel.Y != 0.0f)
                {
                    mPosition = new Vector2(mPosition.X, mPosition.Y + mMoveVector.Y * mMovementInPixel.Y);
                    FRectangle rect = new FRectangle();
                    foreach (Obstacle o in ObstacleAccessor.Instance.mObstacleList.Values)
                    {
                        if (o != this)
                        {
                            if (Intersect(o, ref rect))
                            {
                                mVerticalCollision = true;
                                mPosition = new Vector2(mPosition.X, mPosition.Y + mMoveVector.Y * rect.mSize.Y * System.Math.Sign(-mMovementInPixel.Y));
                                this.CollisionEffect(o, (mMovementInPixel.Y > 0.0f) ? CollisionWay.ABOVE : CollisionWay.BELOW);
                                o.CollisionEffect(this, (mMovementInPixel.Y > 0.0f) ? CollisionWay.BELOW : CollisionWay.ABOVE);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                mPosition = new Vector2(mPosition.X + mMoveVector.X * mMovementInPixel.X, mPosition.Y + mMoveVector.Y * mMovementInPixel.Y); 
            }
        }
    }
}
