using UnityEngine;
namespace MovementSys
{
    public abstract class MovementInputBase : MonoBehaviour
    {
        public abstract bool IsSprint();
        public abstract bool IsDash();
        public abstract bool IsBrake();
        public abstract Vector2 GetLocalMoveDirXZ();
        public abstract float GetYInput();
        public bool IsUp() => GetYInput() > 0;
        public bool IsDown() => GetYInput() < 0;
        public virtual Vector3 GetLocalMoveDirXYZ()
        {
            var mov = GetLocalMoveDirXZ(); 
            return mov.XZToXYZ(GetYInput());
        }
    }
}
 