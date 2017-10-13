using Microsoft.Xna.Framework;

namespace SuperMarioBros.PhysicComponent
{
    public class Speed
    {
        private float _mSpeedLimit;
        private float mSpeedMinimum;
        private float _mAcceleration;
        private float _mCurrentSpeed;
        private bool _mAllowNegativeSpeed;

        public float mAcceleration { get => _mAcceleration; set => _mAcceleration = value; }
        public float mCurrentSpeed { get => _mCurrentSpeed; private set => _mCurrentSpeed = value; }
        public float mSpeedLimit
        {
            get => _mSpeedLimit;
            set
            {
                _mSpeedLimit = value;
                if (mAllowNegativeSpeed)
                {
                    mSpeedMinimum = -value;
                }
            }

        }
        public bool  mAllowNegativeSpeed
        {
            get => _mAllowNegativeSpeed;
            set
            {
                _mAllowNegativeSpeed = value;
                if (value)
                {
                    mSpeedMinimum = -mSpeedLimit;
                }
                else
                {
                    mSpeedMinimum = 0.0f;
                }
            }
        }
        private bool _mInPositiveValue;
        public bool mInPositiveValue { get => _mInPositiveValue; private set => _mInPositiveValue = value; }

        public bool mEvolveInPositiveNumber;

        public Speed(float speedLimit)
        {
            mCurrentSpeed = 0.0f;
            mSpeedLimit = speedLimit;
            mAllowNegativeSpeed = false;
            mEvolveInPositiveNumber = true;
            mInPositiveValue = true;
        }
        
        private void AugmentSpeed()
        {
            mCurrentSpeed = MathHelper.Clamp(mCurrentSpeed + mAcceleration, mSpeedMinimum, mSpeedLimit);
        }

        private void ReduceSpeed()
        {
            mCurrentSpeed = MathHelper.Clamp(mCurrentSpeed - mAcceleration, mSpeedMinimum, mSpeedLimit);
        }

        public void SpeedUp()
        { 
            if(mEvolveInPositiveNumber)
            {
                AugmentSpeed();
            }
            else
            {
                ReduceSpeed();
            }
        }

        public void SlowDown()
        {
            if(mEvolveInPositiveNumber)
            {
                ReduceSpeed();
            }
            else
            {
                AugmentSpeed();
            }
        }

        public void SpeedToMax()
        {
            if (mEvolveInPositiveNumber)
            {
                mCurrentSpeed = mSpeedLimit;
            }
            else
            {
                mCurrentSpeed = mSpeedMinimum;
            }
        }

        public void SpeedToMin()
        {
            mCurrentSpeed = -mSpeedLimit;
        }

        public void Stop()
        {
            mCurrentSpeed = 0.0f;
        }

        public void DoubleSpeedLimit()
        {
            mSpeedLimit *= 2.0f;
        }

        public void SpeedLimitToNormal()
        {
            mSpeedLimit *= 0.5f;
        }
    }
}
