#if UNITY_EDITOR  
#endif
using UnityEngine; 
namespace MovementSys
{
    public class MovementInputFollowTransform :  MovementInputBase
    {
        public Transform Target;
        public override bool IsSprint() => false;
        public override bool IsDash() => false;
        public override Vector2 GetLocalMoveDirXZ()
        {
            if(Target)
                return transform.InverseTransformDirection(transform.DirectionTo(Target.position)).XZ();
            return Vector2.zero;
        }
        public override bool IsBrake() => false;
        public override float GetYInput()
        {
            if (Target)
                return transform.PosY() - Target.PosY();
            return 0f;
        }
    }
}
