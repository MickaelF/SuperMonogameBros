using Microsoft.Xna.Framework;


namespace SuperMarioBros.PhysicComponent
{
    public class Ray2D
    {
        private Vector2 _mPosition;
        private Vector2 _mDirection;
        private Vector2 _mInvDirection;
        private Vector2 _mIntersection;
        private int[] _mSignInvDirection;
        private float _mIntersectionDistance;

        public Vector2 mDirection
        {
            get => _mDirection;
            set
            {
                _mDirection = value;
                _mInvDirection = Vector2.One / value;
            }
        }
        public Vector2 mIntersection
        {
            get => _mIntersection;
            set
            {
                _mIntersection = value;
                _mIntersectionDistance = (mPosition - value).Length();
                _mSignInvDirection[0] = (_mIntersection.X < 0.0f) ? 1 : 0;
                _mSignInvDirection[1] = (_mIntersection.Y < 0.0f) ? 1 : 0;
            }
        }
        public Vector2 mPosition { get => _mPosition; set => _mPosition = value; }
        public Vector2 mInvDirection { get => _mInvDirection; private set => _mInvDirection = value; }
        public float mIntersectionDistance { get => _mIntersectionDistance; private set => _mIntersectionDistance = value; }
        public int[] mSignInvDirection { get => _mSignInvDirection; private set => _mSignInvDirection = value; }

        public int mIdObstacleIntersected;

        public Ray2D(Vector2 pos, Vector2 dir)
        {
            mDirection = dir;
            mPosition = pos;
            mIdObstacleIntersected = -1;
            mSignInvDirection = new int[2];
        }

        public bool IsIntersectionFound()
        {
            return mIdObstacleIntersected != -1;
        }
    }
}
