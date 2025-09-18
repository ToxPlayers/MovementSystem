using UnityEngine;
namespace MovementSys
{
    public class AirborneGravityMovement : MovementBase
    {
        public float AirborneStrafeForce;
        public float AirborneRunMultiplier;
        [SerializeField] AnimationCurve _gravityStrengthOverTime;
        bool _hasMoveInput;
        bool _runInput;
        private void Update()
        {
            var mov = Input.GetLocalMoveDirXZ();
            _hasMoveInput = ! mov.IsZero();
            _runInput = Input.IsSprint();
        }

        private void FixedUpdate()
        {
            if (IsGrounded)
                return;

            Move(Gravity * _gravityStrengthOverTime.Evaluate(Grounded.GroundedChangeTime));

            if (_hasMoveInput && AirborneStrafeForce != 0f)
            {
                var force = AirborneStrafeForce;
                if (_runInput)
                    force *= AirborneRunMultiplier;

                Move(MoveDirection * force);
            }
        }
    }
}
