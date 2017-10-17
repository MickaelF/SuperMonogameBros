using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SuperMarioBros.PhysicComponent
{
    public struct FRectangle
    {
    
        public Vector2 mPosition;
        public Vector2 mSize;

        public FRectangle(float Xpos, float Ypos, float width, float height)
        {
            mPosition = new Vector2(Xpos, Ypos);
            mSize = new Vector2(width, height);
        }

        public FRectangle(Vector2 pos, Vector2 size)
        {
            mPosition = pos;
            mSize = size;
        }

        public bool Intersect(FRectangle other)
        {
            FRectangle rect = new FRectangle();
            return Intersect(other, ref rect);
        }

        public bool Intersect(FRectangle other, ref FRectangle intersection)
        {
            float intersectionLeft = System.Math.Max(mPosition.X, other.mPosition.X);
            float intersectionRight = System.Math.Min(mPosition.X + mSize.X, other.mPosition.X + other.mSize.X);
            float intersectionTop = System.Math.Max(mPosition.Y, other.mPosition.Y);
            float intersectionBottom = System.Math.Min(mPosition.Y + mSize.Y, other.mPosition.Y + other.mSize.Y);
            if ((intersectionLeft < intersectionRight) && (intersectionTop < intersectionBottom))
            {
                intersection = new FRectangle(intersectionLeft, intersectionTop, intersectionRight - intersectionLeft, intersectionBottom - intersectionTop);
                return true;
            }
            return false;
        }
    }
    public class Obstacle
    {
        //! Next Id Value
        static int sNextId = 0;
        //! Id of the obstacle. Cannot be changed. Only way to identify obstacles
        private int _cId;

        //! Obstacle top-left position (for display)
        private Vector2 _mPosition;
        //! Obstacle size (for display)
        private Vector2 _mSize;
        //! Bounding-box rectange (for collision)
        private FRectangle _mBoundingBox;
        //! Encapsulation of _cId
        public int cId { get => _cId; private set => _cId = value; }
        //! Encapsulation of _mPosition
        public Vector2 mPosition
        {
            get => _mPosition;
            set
            {
                _mBoundingBox.mPosition += (value - _mPosition);
                _mPosition = value;
            }
        }
        //! Encapsulation of _mSize
        public Vector2 mSize { get => _mSize; set => _mSize = value; }
        //! Define if the obstacle is collidable or not. May be encapsulated (TO DO?)
        public bool mIsCollidable;

        //! Declare delegate - required signature of Event func. Return true when the func have to be removed from the event list
        public delegate bool Event();
        //! Event called when there is no event in the event list
        protected Event mPrimaryEvent;
        //! Event list to call
        private List<Event> mEventList;

        //! From where the collision happen
        public enum CollisionWay
        {
            LEFT, RIGHT, ABOVE, BELOW
        }

        public Obstacle()
        {
            cId = sNextId++;
            mEventList = new List<Event>();
            mIsCollidable = true;
            ObstacleAccessor.Instance.Add(this);
            mPrimaryEvent = delegate { return true; };
            _mBoundingBox = new FRectangle();
        }

        public void AddEventFront(Event e)
        {
            mEventList.Insert(0, e);
        }

        public void AddEvent(Event e)
        {
            mEventList.Add(e);
        }
        
        public void RemoveEvent(Event e)
        {
            mEventList.Remove(e);
        }

        public void SetBoundingBoxSize(Vector2 p)
        {
            _mBoundingBox.mSize = p;
        }

        public void DefinePositionOffset(Vector2 p)
        {
            _mBoundingBox.mPosition += p;
        }

        //! The effect of the collision. Must be override if there is special event linked to the collision.
        public virtual void CollisionEffect(Obstacle obst, CollisionWay way)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            if (mEventList.Count > 0)
            {
                if(mEventList[0]())
                {
                    mEventList.Remove(mEventList[0]);
                }
            }
            if (mEventList.Count == 0)
            {
                mPrimaryEvent();
            }
        }

        public bool DeleteObstacle()
        {
            ObstacleAccessor.Instance.Remove(cId);
            return true;
        }


        public bool Intersect(Obstacle other)
        {
            FRectangle rect = new FRectangle();
            return Intersect(other, ref rect);
        }

        public bool Intersect(Obstacle other, ref FRectangle intersection)
        {
            if (mIsCollidable && other != this)
            {
                return _mBoundingBox.Intersect(other._mBoundingBox, ref intersection);
            }
            return false;
        }

        public bool Intersect(ref Ray2D ray)
        {
            if (mIsCollidable)
            {
                if (ray.mDirection == Vector2.Zero)
                {
                    return false;
                }
                if (Contains(ray.mPosition))
                {
                    ray.mIdObstacleIntersected = cId;
                    ray.mIntersection = ray.mPosition;
                    if (ray.mDirection.X != 0.0f)
                    {
                        if(ray.mDirection.X > 0.0f)
                        {
                            ray.mIntersection = new Vector2(mPosition.X, ray.mIntersection.Y);
                        }
                        else
                        {
                            ray.mIntersection = new Vector2(mPosition.X + mSize.X, ray.mIntersection.Y);
                        }
                    }
                    else if (ray.mDirection.Y != 0.0f)
                    {
                        if (ray.mDirection.Y > 0.0f)
                        {
                            ray.mIntersection = new Vector2(ray.mIntersection.X, mPosition.Y);
                        }
                        else
                        {
                            ray.mIntersection = new Vector2(ray.mIntersection.X, mPosition.Y + mSize.Y);;
                        }
                    }
                }
                Vector2[] bounds = new Vector2[2];
                bounds[0] = mPosition;
                bounds[1] = mPosition + mSize;
                float tmin = (bounds[ray.mSignInvDirection[0]].X - ray.mPosition.X) * ray.mInvDirection.X;
                float tmax = (bounds[1 - ray.mSignInvDirection[1]].X - ray.mPosition.X) * ray.mInvDirection.X;
                float tymin = (bounds[ray.mSignInvDirection[0]].Y - ray.mPosition.Y) * ray.mInvDirection.Y;
                float tymax = (bounds[1 - ray.mSignInvDirection[1]].Y - ray.mPosition.Y) * ray.mInvDirection.Y;

                if ((tmin > tymax) || (tymin > tmax))
                {
                    return false;
                }
                else
                {
                    tmin = MathHelper.Max(tmin, tymin);
                    tmax = MathHelper.Min(tmax, tymax);

                    float coefficient = (tmax < tmin) ? tmax : tmin;

                    Vector2 intersection = ray.mPosition + ray.mDirection * coefficient;
                    if (ray.IsIntersectionFound())
                    {
                        if ((intersection - ray.mPosition).Length() < ray.mIntersectionDistance)
                        {
                            ray.mIdObstacleIntersected = cId;
                            ray.mIntersection = intersection;
                            return true;
                        }
                    }
                    else
                    {
                        ray.mIdObstacleIntersected = cId;
                        ray.mIntersection = intersection;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Contains(Vector2 pos)
        {
            if ((pos.X > mPosition.X && pos.X < mPosition.X + mSize.X)
                && (pos.Y > mPosition.Y && pos.Y < mPosition.Y + mSize.Y))
            {
                return true;
            }
            return false;
        }
    }
}
