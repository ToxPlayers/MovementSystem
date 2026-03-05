using Sirenix.OdinInspector;
using TickTimers;
using UnityEngine;
namespace MovementSys
{

    public class WalkMovement : MovementBase
    {
        [SerializeField] float _walkSpeed = 10f;
        [SerializeField] float _runMultiplier = 1.8f;
        [SerializeField] bool _localSpaceInput = true;
        public bool MultiplyByRigidbodyDamping = false;
        [ShowInInspector, ReadOnly] bool _runInput;
        [ShowInInspector,ReadOnly, HideInEditorMode] 
        public Vector3 LastWalkForce { get; private set; }
        public void Update()
        {
            _runInput = Input.IsSprint(); 
        }
        private void OnEnable()   {  LastWalkForce = Vector3.zero; }
        private void OnDisable()  {  LastWalkForce = Vector3.zero; } 

        public void FixedUpdate()
        {
            if (! IsGrounded || ! Handler.HasXZInput)
            {
                LastWalkForce = Vector3.zero;
                return;
            }

            var inputDir = Handler.MoveDirection;
            if (inputDir == Vector3.zero)
            {
                LastWalkForce = Vector3.zero;
                return;
            }

            var speed = _walkSpeed;
            if (_runInput)
                speed *= _runMultiplier;
            if (MultiplyByRigidbodyDamping && IsGrounded)
                speed *= Rb.linearDamping;

            LastWalkForce = inputDir * speed;
            if (_localSpaceInput)
                LastWalkForce = Rb.rotation * LastWalkForce;
            Move(Handler.AlignWithSlope(LastWalkForce));
        } 
    }
}
