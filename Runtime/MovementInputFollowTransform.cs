#if UNITY_EDITOR  
#endif
using UnityEngine;
using UnityEngine.InputSystem;
namespace MovementSys
{
    [System.Serializable]
    public class MovementInputFollowTransform :  IMovementInput
    {
        public Transform From;
        public Transform Target;

        public Vector2 GetLocalMoveDirXZ() {
            return (From.position - Target.position).XZ().normalized;
        }

        public float GetYInput() => (From.position - Target.position).y;

        public bool IsBrake() => false;

        public bool IsDodge() => false;

        public bool IsSprint() => false;
    }
}
