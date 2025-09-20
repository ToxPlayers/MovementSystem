using ScaledTimers;
using Sirenix.OdinInspector;
using System;
using System.Diagnostics;
using UnityEngine;
namespace MovementSys
{
    public class JumpMovement : MovementBase, GroundedCaster.IDisableGrounded
    {
        [Range(0f, 1f)] public float JumpForwardAngle, RunJumpAngle;
        public float JumpForce = 3f;
        [SerializeField] RealTimer _jumpInputForgiveTimer, _jumpDisableGroundedTimer;
        Vector3 _jumpDir;

        private void OnValidate()
        {
        }

        private void OnEnable()
        {
            _jumpDisableGroundedTimer.ForceTimerOver();
        }

        public bool GetIsGroundedDisabled() => ! _jumpDisableGroundedTimer.TimerOver;

        private void Update()
        {
            var jumpInput = Input.GetYInput() > 0;
            var mov = Input.GetLocalMoveDirXZ();
            var hasMoveInput = !mov.IsZero();

            if (!jumpInput)
                return;

            if (IsGrounded || ! _jumpInputForgiveTimer.TimerOver)
            {
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
            else 
                _jumpInputForgiveTimer.Restart();
             
        }

    }
}
