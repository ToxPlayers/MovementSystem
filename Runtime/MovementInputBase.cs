using UnityEngine;
namespace MovementSys
{
    public interface IMovementInput {
        public abstract bool IsSprint();
        public abstract bool IsDodge();
        public abstract bool IsBrake();
        public abstract Vector2 GetLocalMoveDirXZ();
        public abstract float GetYInput();
        public virtual Vector3 GetLocalMoveDirXYZ() {
            var mov = GetLocalMoveDirXZ();
            return mov.XZToXYZ(GetYInput());
        }
    } 
     
}
 