#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using HideInEditMode = Sirenix.OdinInspector.HideInEditorModeAttribute;
#else
using TriInspector;
using HideInEdit = TriInspector.HideInEditModeAttribute;
#endif

using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace MovementSys
{
    [DefaultExecutionOrder(-100)]
    [ExecuteInEditMode]
    public class GroundedCaster : MonoBehaviour
	{
        public LayerMask GroundLayer;
        static Ray _rayDown = new Ray(Vector3.zero, Vector3.down);
        static Ray _rayUp = new Ray(Vector3.zero, Vector3.up);

        [ShowInInspector, ReadOnly, PropertyOrder(99)] GameBoolTimer _groundedTimer;
        RaycastHit _groundedRayHit;
        public RaycastHit GroundedRayHit => _groundedRayHit;
        [SerializeField, Get] CapsuleCollider _capsule;
        [SerializeField] float _sphereCastDistance = 0.3f, _downOffset = -0.02f;
        public float GroundedChangeTime => _groundedTimer.TimeRunning;
        [ShowInInspector, PropertyOrder(98)] public bool IsGrounded => _groundedTimer.Value;
         
        readonly List<Ref<GameTimer>> _disablesGrounded = new();

        public void DisableGrounded(Ref<GameTimer> timer) 
        {
            if(!timer.Value.TimerOver)
                _disablesGrounded.Add(timer);
        }

        private void OnValidate()
        {
            if(!_capsule) _capsule = GetComponent<CapsuleCollider>();
            _groundedTimer.Timer.timerType =  GameTimer.TimerType.Scaled;
        }

        private void Awake()
        {
            Update();
        }
         
        private void Update()
        {
            for (int i = 0; i < _disablesGrounded.Count; i++)
            {
                var isNull = _disablesGrounded[i] == null;
                if (isNull || _disablesGrounded[i].Value.TimerOver)
                {
                    if (isNull && !Application.isPlaying)
                        continue;

                    _disablesGrounded.RemoveAt(i);
                    i--; 
                }
            }
            UpdateGrounded();
        }
         
        public bool UpdateGrounded()
        {
            var groundCast = GetGroundCast(out _groundedRayHit);
            return _groundedTimer.Value = _disablesGrounded.Count == 0 && groundCast;
        }

        bool GetGroundCast(out RaycastHit hit)
        {
            var castPos = GetCastPosition();
            _rayDown.origin = castPos;
            if (Physics.Raycast(_rayDown, out hit, _sphereCastDistance, GroundLayer)) 
                return true; 

            _rayDown.origin = castPos;

            if (Physics.SphereCast(_rayDown, _capsule.radius, out hit, _sphereCastDistance, GroundLayer))
            {
                _rayDown.origin = hit.point + Vector3.up * 0.01f;
                return hit.collider.Raycast(_rayDown, out hit, 0.02f );
            }
            return false;
        }

        static public bool FixUnderMapPos(Vector3 pos, out Vector3 outPos, LayerMask groundLayer)
        {
            if ( !HasGroundBelow(pos, groundLayer) && NavMesh.FindClosestEdge(pos, out NavMeshHit hit, -1))
            {
                outPos = hit.position;
                return true;
            }
            outPos = pos;
            return false;
        }
        public void FixUnderMap()
        {
            var minPos = _capsule.bounds.min;
            if (FixUnderMapPos(transform.position, out Vector3 fixedpos, GroundLayer))
            {
                var offset = minPos.y - fixedpos.y;
                var pos = transform.position;
                pos.y += offset;
                transform.position = pos;
            }
        }
        static public bool HasGroundBelow(Vector3 pos, LayerMask groundLayer)
        {
            _rayDown.origin = pos;
            return Physics.Raycast(_rayDown, Mathf.Infinity, groundLayer);
        }
        static public bool HasGroundAbove(Vector3 pos, out RaycastHit hit, LayerMask groundLayer)
        {
            _rayUp.origin = pos;
            return Physics.Raycast(_rayUp, out hit, Mathf.Infinity, groundLayer);
        }

        Vector3 GetCastPosition()
        {
            var castPos = _capsule.bounds.min;
            castPos.x += _capsule.bounds.extents.x;
            castPos.z += _capsule.bounds.extents.z;
            castPos.y += _downOffset;
            castPos.y += _sphereCastDistance;
            castPos += Vector3.up * _capsule.radius;
            return castPos;
        }

#if UNITY_EDITOR  
        private void OnDrawGizmos()
        {
            if (!enabled || ! _capsule)
                return;

            Gizmos.color = IsGrounded ? Color.magenta : Color.red;

            var castPos = GetCastPosition();
            Gizmos.DrawWireSphere(castPos, _capsule.radius);
            var desiredGroundPos = castPos + Vector3.down * _sphereCastDistance;
            Gizmos.DrawWireSphere(desiredGroundPos, _capsule.radius);

        }
#endif
    }
}

