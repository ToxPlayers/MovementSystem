using UnityEngine;
namespace MovementSys
{
    public abstract class MovementBase : MonoBehaviour
    {
        [field: SerializeField, Get] public MovementHandler Handler { get; private set; }
        public Vector3 Gravity => Handler.Gravity;
        public bool IsSetup => ! Handler;
        public GroundedCaster Grounded => Handler.Grounded;
        public bool IsGrounded => Grounded.IsGrounded;
        public MovementInputBase Input => Handler.Input;
        public Rigidbody Rb => Handler.Rb;
        public Transform Tf => Handler.Rb.transform;
        public Vector3 MoveRawInput => Handler.MoveRawInput;
        public Vector3 MoveDirection => Handler.MoveDirection; 
        public void Move(Vector3 force, bool multSpeedModifier = true) => Handler.Move(force, multSpeedModifier);
        public void Push(Vector3 force) => Handler.Push(force); 
    }
}
