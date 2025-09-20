using TickTimers;
using Sirenix.OdinInspector;
using System;
using System.Diagnostics;
using UnityEngine;
namespace MovementSys
{
    public class JumpMovement : MovementBase, GroundedCaster.IDisableGrounded
    {
        [Range(0f, 1f)] public float JumpForwardAngle = 1f, RunJumpAngle = 0.5f;
        public float JumpForce = 3f;
        [SerializeField] float _jumpInputForgiveGroundedTime = 0.08f;
        [SerializeField]
        DurationTimer _jumpDisableGroundedTimer = new()
        {
            StopOnTimerOver = true,
            MaxTime = 0.1f
        };
        Vector3 _jumpDir; 

        private void OnEnable()
        {
            _jumpDisableGroundedTimer.StopOnTimerOver = true;
        }

        public bool GetIsGroundedDisabled() => ! _jumpDisableGroundedTimer.IsTimerOver;

        private void Update()
        { 
            var jumpInput = Input.GetYInput() > 0;
            if (!jumpInput)
                return;

            var mov = Input.GetLocalMoveDirXZ();
            var hasMoveInput = !mov.IsZero();
            if (IsGrounded && Grounded.GroundedChangeTime > _jumpInputForgiveGroundedTime
                && _jumpDisableGroundedTimer.IsTimerOver)
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
                _jumpDisableGroundedTimer.Stop();
        }

    }
}
