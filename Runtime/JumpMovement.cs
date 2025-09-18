using Sirenix.OdinInspector;
using System;
using UnityEngine;
namespace MovementSys
{
    public class JumpMovement : MovementBase
    {
        [ReadOnly] public GameTimer JumpActiveTimer;
        [Range(0f, 1f)] public float JumpForwardAngle, RunJumpAngle;
        public float JumpForce = 3f;
        [SerializeField] GameTimer _jumpInputForgiveTimer, _jumpDisableGroundedTimer;
        Vector3 _jumpDir;

        private void OnValidate()
        {
            JumpActiveTimer.timerType = GameTimer.TimerType.Scaled;
        }

        private void OnEnable()
        {
            JumpActiveTimer.ForceTimerOver();
        }

        private void Update()
        {
            var jumpInput = Input.GetYInput() > 0;
            var mov = Input.GetLocalMoveDirXZ();
            var hasMoveInput = !mov.IsZero();

            if (jumpInput)
            {
                if (IsGrounded || !_jumpInputForgiveTimer.TimerOver)
                {
                    JumpActiveTimer.Restart();
                    _jumpDisableGroundedTimer.Restart();

                    _jumpDir = Vector3.up;
                    if (hasMoveInput)
                    {
                        var jumpForwAngle = Input.IsSprint() ? RunJumpAngle : JumpForwardAngle;
                        var jumpLocalDir = Vector3.Lerp(Vector3.forward, Vector3.up, jumpForwAngle).normalized;
                        _jumpDir = transform.TransformDirection(jumpLocalDir);
                    }

                    Push(_jumpDir * JumpForce);
                }
                else _jumpInputForgiveTimer.Restart();
            }

            if (JumpActiveTimer.TimerOver)
                return;

            if (IsGrounded && _jumpDisableGroundedTimer.TimerOver)
                JumpActiveTimer.ForceTimerOver();
        }
    }
}
