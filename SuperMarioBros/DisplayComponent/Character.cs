using Microsoft.Xna.Framework;
using SuperMarioBros.PhysicComponent;
namespace SuperMarioBros.DisplayComponent
{
    public class Character : DrawableObstacle
    {
        private Vector2 _mMoveVector;

        public Vector2 mMoveVector { get => _mMoveVector; set => _mMoveVector = value; }
        public bool mIsFalling { get => _mIsFalling; set => _mIsFalling = value; }

        public Speed mVerticalSpeed;
        public Speed mHorizontalSpeed;

        public Vector2 mMovementInPixel;
        
        private bool _mIsFalling;

        public Character ()
        {
            mMovementInPixel = new Vector2(0.0f, 0.0f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            mPosition = new Vector2(mPosition.X + mMoveVector.X * mMovementInPixel.X, mPosition.Y + mMoveVector.Y * mMovementInPixel.Y);
        }

        public void InverseInX()
        {
            _mMoveVector.X = -_mMoveVector.X;
        }

        public void InverseInY()
        {
            _mMoveVector.Y = -_mMoveVector.Y;
        }
    }
}
