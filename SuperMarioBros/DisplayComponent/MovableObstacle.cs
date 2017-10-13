using Microsoft.Xna.Framework;
using SuperMarioBros.PhysicComponent;
using System.Collections.Generic;
namespace SuperMarioBros.DisplayComponent
{
    public class MovableObstacle : DrawableObstacle
    {
        private Vector2 _mMoveVector;
        protected bool mIsMovable;

        public Vector2 mMoveVector { get => _mMoveVector; set => _mMoveVector = value; }
        public bool mIsFalling { get => _mIsFalling; set => _mIsFalling = value; }

        public Speed mVerticalSpeed;
        public Speed mHorizontalSpeed;

        public Vector2 mMovementInPixel;
        
        private bool _mIsFalling;

        protected bool mVerticalCollision;
        protected bool mHorizontalCollision;

        public MovableObstacle ()
        {
            mMovementInPixel = new Vector2(0.0f, 0.0f);
            mMoveVector = new Vector2(1.0f, -1.0f);
            mIsMovable = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (mIsMovable)
            {
                mPosition = new Vector2(mPosition.X + mMoveVector.X * mMovementInPixel.X, mPosition.Y + mMoveVector.Y * mMovementInPixel.Y);
            }
        }

        public void InverseInX()
        {
            _mMoveVector.X = -_mMoveVector.X;
        }

        public void InverseInY()
        {
            _mMoveVector.Y = -_mMoveVector.Y;
        }

        public void CollisionDetection()
        {
            mVerticalCollision = false;
            mHorizontalCollision = false;

            var arrayObstacle = ObstacleAccessor.Instance.mObstacleList;
            // On crée trois rayons qui partent du personnage.
            float xPos = mPosition.X;
            float horizontalRayDirection = -1.0f;

            if (mHorizontalSpeed.mCurrentSpeed >= 0.0f)
            {
                xPos += mSize.X * 0.75f;
                horizontalRayDirection = 1.0f;
            }

            Ray2D[] rayForward = new Ray2D[3];
            rayForward[0] = new Ray2D(new Vector2(xPos, mPosition.Y), new Vector2(horizontalRayDirection, 0.0f));
            rayForward[1] = new Ray2D(new Vector2(xPos, mPosition.Y + mSize.Y * 0.5f), new Vector2(horizontalRayDirection, 0.0f));
            rayForward[2] = new Ray2D(new Vector2(xPos, mPosition.Y + mSize.Y), new Vector2(horizontalRayDirection, 0.0f));

            Ray2D[] rayVerticalTests = new Ray2D[3];
            float verticalPosition = (mVerticalSpeed.mCurrentSpeed > 0) ? mPosition.Y : mPosition.Y + mSize.Y;
            float rayDirection = (mVerticalSpeed.mCurrentSpeed > 0) ? -1.0f : 1.0f;
            rayVerticalTests[0] = new Ray2D(new Vector2(mPosition.X + mSize.X * 0.05f, verticalPosition), new Vector2(0.0f, rayDirection));
            rayVerticalTests[1] = new Ray2D(new Vector2(mPosition.X + mSize.X * 0.95f, verticalPosition), new Vector2(0.0f, rayDirection));
            rayVerticalTests[2] = new Ray2D(new Vector2(mPosition.X + mSize.X * 0.5f, verticalPosition), new Vector2(0.0f, rayDirection));

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
                    mMovementInPixel.X = System.Math.Sign(mMovementInPixel.X) * lowestDistanceCollision;
                    mHorizontalCollision = true;
                    Obstacle obst = arrayObstacle[idObstacleCollided];
                    this.CollisionEffect(obst, (mHorizontalSpeed.mCurrentSpeed > 0.0f) ? CollisionWay.LEFT : CollisionWay.RIGHT);
                    obst.CollisionEffect(this, (mHorizontalSpeed.mCurrentSpeed > 0.0f) ? CollisionWay.RIGHT : CollisionWay.LEFT);
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
                    mIsFalling = false;
                    mMovementInPixel.Y = System.Math.Sign(mMovementInPixel.Y) * lowestDistanceCollision;
                    Obstacle obst = arrayObstacle[idObstacleCollided];
                    mVerticalCollision = true;
                    obst.CollisionEffect(this, (mVerticalSpeed.mCurrentSpeed > 0.0f) ? CollisionWay.BELOW : CollisionWay.ABOVE);
                    this.CollisionEffect(obst, (mVerticalSpeed.mCurrentSpeed > 0.0f) ? CollisionWay.ABOVE : CollisionWay.BELOW);
                }
            }
        }
    }
}
