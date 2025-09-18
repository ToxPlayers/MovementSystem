using Sirenix.OdinInspector;
using UnityEngine;
namespace MovementSys
{
    public class WalkMovement : MovementBase
    {
        [SerializeField] float _walkSpeed = 10f;
        [SerializeField] float _runMultiplier = 1.8f;
        [ShowInInspector, ReadOnly] bool _runInput;
        public void Update()
        {
            _runInput = Input.IsSprint(); 
        }

        public void FixedUpdate()
        {
            if (! IsGrounded || ! Handler.HasXZInput)
                return;

            var speed = _walkSpeed;
            if (_runInput)
                speed *= _runMultiplier;
            if (IsGrounded)
                speed *= Rb.linearDamping; 
            if(speed != 0f)
                Move(speed * Handler.MoveDirection);
        } 
    }
}
