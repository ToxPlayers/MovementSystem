using Sirenix.OdinInspector; 
using System;
using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
namespace MovementSys
{ 
    [DefaultExecutionOrder(-99)]
    public class MovementHandler : MonoBehaviour
    {
        protected const float MaxPossibleVelocity = 1000; 
        [ShowInInspector, HideInEditorMode] public Vector3 VelocityResult => Rb ? Rb.linearVelocity : Vector3.zero;
        [ShowInInspector, HideInEditorMode] public float VelocitySpeed => VelocityResult.magnitude;
        [field: SerializeReference] public StatValue SpeedMultiplier { get; private set; } = new StatValue(1f);
        [field: SerializeField, GetChild, Required] public GroundedCaster Grounded { get; private set; }
        [field: SerializeField, Get, Required] public Rigidbody Rb { get; private set; } 
        [field: SerializeField, Get, PropertyOrder(-10), Required] public MovementInputBase Input { get; private set; } 
         
        protected virtual void Awake()
        {
            Rb.maxLinearVelocity = MaxPossibleVelocity; 
        }

        [SerializeField] float _groundedDamping;
        public bool IsOnSlope { get; private set; }
        public float SlopeAngle { get; private set; }
        [SerializeField] float _maxSlopeAngle = 40f;
        public bool UseCustomGravity;
        [ShowIf(nameof(UseCustomGravity))] public Vector3 CustomGravity;
        public Vector3 MoveRawInput { get; private set; }
        public Vector3 MoveDirection { get; private set; }

        public bool HasXZInput { get; private set; }
        #region Update 
        void Update()
        {
            UpdateInput();
            UpdateSlopeDir(); 
        }

        private void UpdateInput()
        {
            MoveRawInput = Input.GetLocalMoveDirXYZ();
            var xzInput = MoveRawInput.XZ();
            HasXZInput = ! xzInput.IsZero();
        }

        public bool HasMoveInput { get; private set; }
        public Vector3 Gravity
        {
            get
            {
                if(UseCustomGravity)
                    return CustomGravity;
                return Physics.gravity;
            }
        }

        void UpdateSlopeDir()
        {
            Rb.linearDamping = Grounded.IsGrounded ? _groundedDamping : 0f;
            if (Grounded.IsGrounded)
            {
                var slopeHit = Grounded.GroundedRayHit;
                SlopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                IsOnSlope = SlopeAngle != 0f && SlopeAngle < _maxSlopeAngle;
                MoveDirection = AlignWithSlope(MoveRawInput).normalized;
            }
            else
            {
                IsOnSlope = false;
                MoveDirection = MoveRawInput;
            }
        }


        #endregion

        public Vector3 AlignWithSlope(Vector3 vector)
        {
            if(IsOnSlope)
                return Vector3.ProjectOnPlane(vector, Grounded.GroundedRayHit.normal);
            return vector;
        }
        public void Move(Vector3 vel, bool multSpeedModifier = true)
        {
            if (multSpeedModifier)
                vel *= SpeedMultiplier; 
            Rb.AddForce(vel , ForceMode.Acceleration);
        }

        [Button]
        public void Push(Vector3 vel)
        {
            Rb.AddForce(vel, ForceMode.VelocityChange);
        }
         
          
        #region Debug

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                var pos = transform.position;
                Gizmos.DrawLine(pos, pos + Rb.linearVelocity);

                var groundRay = Grounded.GroundedRayHit;
                Gizmos.DrawLine(groundRay.point, groundRay.point + MoveDirection);
            }  
        }

        void OnDrawGizmos()
        { 
            if(Application.isPlaying)
            {
                var maxSlopeAngle = Quaternion.Euler(-_maxSlopeAngle, 0, 0);
                var slopeAngleArrow = transform.position + (maxSlopeAngle * transform.forward * 2);

                GizmosU.GizmosArrow(transform.position, slopeAngleArrow, Color.magenta, 0.3f, 0.05f);
            }
        }


        //void OnGUI()
        //{
        //    GUILayout.BeginVertical();
        //    GUILayout.Space(200);
        //    SirenixEditorGUI.BeginBox("Movement");
        //    GUILayout.Label("Velocity: " + _rb.velocity);
        //    GUILayout.Label("Velocity Magnitude: " + _rb.velocity.magnitude.ToString("00.00"));
        //    var isGrounded = _grounded.IsGrounded;
        //    GUI.color = isGrounded ? Color.green : Color.red;
        //    GUILayout.Label("IsGrounded: " + isGrounded);
        //    GUI.color = _isDashing ? Color.green : Color.red;
        //    GUILayout.Label("IsDashing: " + _isDashing);
        //    GUI.color = _isDashCooldown ? Color.green : Color.red;
        //    GUILayout.Label("IsDashcooldown: " + _isDashCooldown);
        //    SirenixEditorGUI.EndBox();
        //    GUILayout.EndVertical(); 
        //}
#endif

        #endregion
    }
}
