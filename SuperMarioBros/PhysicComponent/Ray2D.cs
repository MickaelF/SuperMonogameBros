using Microsoft.Xna.Framework;


namespace SuperMarioBros.PhysicComponent
{
    public class Ray2D
    {
        private Vector2 _mPosition;
        private Vector2 _mDirection;

        public Vector2 mDirection { get => _mDirection; set => _mDirection = value; }
        public Vector2 mPosition { get => _mPosition; set => _mPosition = value; }

        public Ray2D(Vector2 pos, Vector2 dir)
        {
            mDirection = dir;
            mPosition = pos;
        }
    }
}
