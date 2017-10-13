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
        private float mLastHorizontalSpeedBeforeZero;

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
                Vector2 distanceCollision = CollisionDetection();
                if (mHorizontalCollision)
                {
                    mMovementInPixel.X = distanceCollision.X;
                    mHorizontalSpeed.Stop();
                }
                if (mVerticalCollision)
                {
                    mVerticalSpeed.Stop();
                    mMovementInPixel.Y = distanceCollision.Y;
                    if (mVerticalSpeed.mCurrentSpeed > 0.0f)
                    {
                        mVerticalSpeed.SlowDown();
                        mMovementInPixel.Y = mVerticalSpeed.mCurrentSpeed * gt.ElapsedGameTime.Milliseconds / 1000.0f;
                    }
                }
                else
                {
                    mVerticalSpeed.SlowDown();
                }
                
                mPosition = new Vector2(mPosition.X + mMoveVector.X * mMovementInPixel.X, mPosition.Y + mMoveVector.Y * mMovementInPixel.Y);
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
                Vector2 distanceCollision = CollisionDetection();
                if (mHorizontalCollision)
                {
                    mHorizontalSpeed.mEvolveInPositiveNumber = !mHorizontalSpeed.mEvolveInPositiveNumber;
                    mHorizontalSpeed.SpeedToMax();
                    mMovementInPixel.X = mHorizontalSpeed.mCurrentSpeed * gt.ElapsedGameTime.Milliseconds / 1000.0f;
                }
                if (mVerticalCollision)
                {
                    mVerticalSpeed.Stop();
                    mMovementInPixel.Y = distanceCollision.Y;
                }
                else
                {
                    mVerticalSpeed.SlowDown();
                }
                mPosition = new Vector2(mPosition.X + mMoveVector.X * mMovementInPixel.X, mPosition.Y + mMoveVector.Y * mMovementInPixel.Y);
            }
            return false;
        }

        public bool Jump()
        {
            // Si on ne saute pas encore et qu'on touche le sol
            if (!mIsJumping && mIsOnGround)
            {
                mVerticalSpeed.SpeedToMax();
                mVerticalSpeed.mAcceleration = 50;
            }
            else if (mIsJumping)
            {
                // TO DO : augmenter la valeur du saut.
            }
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
                        return true;
                    }
                    return false;
                }
            }
            return true;
        }

        public Vector2 CollisionDetection()
        {
            mVerticalCollision = false;
            mHorizontalCollision = false;
            Vector2 distanceCollision = new Vector2();
            if (mIsCollidable)
            {
                var arrayObstacle = ObstacleAccessor.Instance.mObstacleList;
                // On crée trois rayons qui partent du personnage.
                float xPos = mPosition.X;
                float[] xPositionVerticalCollision = { mPosition.X + mSize.X * 0.37f, mPosition.X + mSize.X * 0.75f, mPosition.X + mSize.X };
                float horizontalRayDirection = -1.0f;

                // Si on va vers la gauche
                if (mLastHorizontalSpeedBeforeZero > 0.0f)
                {
                    xPos += mSize.X;
                    horizontalRayDirection = 1.0f;
                    xPositionVerticalCollision = new float[] { mPosition.X, mPosition.X + mSize.X * 0.37f, mPosition.X + mSize.X * 0.75f };
                }

                Ray2D[] rayForward = new Ray2D[3];
                rayForward[0] = new Ray2D(new Vector2(xPos, mPosition.Y), new Vector2(horizontalRayDirection, 0.0f));
                rayForward[1] = new Ray2D(new Vector2(xPos, mPosition.Y + mSize.Y * 0.5f), new Vector2(horizontalRayDirection, 0.0f));
                rayForward[2] = new Ray2D(new Vector2(xPos, mPosition.Y + mSize.Y), new Vector2(horizontalRayDirection, 0.0f));

                Ray2D[] rayVerticalTests = new Ray2D[3];
                float verticalPosition = mPosition.Y + mSize.Y;
                float rayDirection = 1.0f;
                // Si on est en phase d'ascension
                if (mVerticalSpeed.mCurrentSpeed > 0.0f)
                {
                    verticalPosition = mPosition.Y;
                    rayDirection = -1.0f;
                }
                rayVerticalTests[0] = new Ray2D(new Vector2(xPositionVerticalCollision[0], verticalPosition), new Vector2(0.0f, rayDirection));
                rayVerticalTests[1] = new Ray2D(new Vector2(xPositionVerticalCollision[1], verticalPosition), new Vector2(0.0f, rayDirection));
                rayVerticalTests[2] = new Ray2D(new Vector2(xPositionVerticalCollision[2], verticalPosition), new Vector2(0.0f, rayDirection));

                foreach (Obstacle obst in arrayObstacle.Values)
                {
                    if (obst.cId != cId)
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
                    int idObstacleCollided = rayUsed[0].mIdObstacleIntersected;
                    for (int i = 1; i < rayUsed.Count; ++i)
                    {
                        lowestDistanceCollision = MathHelper.Min(lowestDistanceCollision, rayUsed[i].mIntersectionDistance);
                        idObstacleCollided = rayUsed[i].mIdObstacleIntersected;
                    }
                    if (lowestDistanceCollision <= System.Math.Abs(mMovementInPixel.X))
                    {
                        distanceCollision.X = System.Math.Sign(mMovementInPixel.X) * lowestDistanceCollision;
                        mHorizontalCollision = true;
                        Obstacle obst = arrayObstacle[idObstacleCollided];
                        this.CollisionEffect(obst, (mHorizontalSpeed.mCurrentSpeed > 0.0f) ? CollisionWay.LEFT : CollisionWay.RIGHT);
                        obst.CollisionEffect(this, (mHorizontalSpeed.mCurrentSpeed > 0.0f) ? CollisionWay.RIGHT : CollisionWay.LEFT);
                    }
                }

                rayUsed.Clear();
                foreach (Ray2D r in rayVerticalTests)
                {
                    if (r.IsIntersectionFound() && !float.IsNaN(r.mIntersectionDistance))
                    {
                        rayUsed.Add(r);
                    }
                }
                if (rayUsed.Count > 0)
                {
                    float lowestDistanceCollision = rayUsed[0].mIntersectionDistance;
                    int idObstacleCollided = rayUsed[0].mIdObstacleIntersected;
                    for (int i = 1; i < rayUsed.Count; ++i)
                    {
                        if (lowestDistanceCollision > rayUsed[i].mIntersectionDistance)
                        {
                            lowestDistanceCollision = rayUsed[i].mIntersectionDistance;
                            idObstacleCollided = rayUsed[i].mIdObstacleIntersected;
                        }
                    }
                    if (lowestDistanceCollision <= System.Math.Abs(mMovementInPixel.Y))
                    {
                        distanceCollision.Y = System.Math.Sign(mMovementInPixel.Y) * lowestDistanceCollision;
                        Obstacle obst = arrayObstacle[idObstacleCollided];
                        mVerticalCollision = true;
                        mIsOnGround = (mVerticalSpeed.mCurrentSpeed <= 0.0f);
                        obst.CollisionEffect(this, (mVerticalSpeed.mCurrentSpeed > 0.0f) ? CollisionWay.BELOW : CollisionWay.ABOVE);
                        this.CollisionEffect(obst, (mVerticalSpeed.mCurrentSpeed > 0.0f) ? CollisionWay.ABOVE : CollisionWay.BELOW);
                    }
                }
            }
            return distanceCollision;
        }
    }
}
