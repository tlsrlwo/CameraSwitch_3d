using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CST
{
    public class PlayerMove : MonoBehaviour
    {
        public float currentMoveSpeed;
        public float walkSpeed = 3, walkBackSpeed = 2;
        public float runSpeed = 7, runBackSpeed = 5;
        public float crouchSpeed = 2, crouchBackSpeed = 1;
        public float airSpeed = 1.5f;

        [Header("Move")]
        public float xInput, zInput;
        [HideInInspector] public Vector3 dir;

        [Header("GroundCheck")]
        Vector3 spherePos;
        public LayerMask groundMask;
        public float groundYOffset;

        public float gravity = -9.81f;
        private Vector3 velocity;

        private CharacterController controller;        

        public MovementBaseState currentState;
        public IdleState Idle = new IdleState();
        public RunState Run = new RunState();
        public WalkState Walk = new WalkState();

        [HideInInspector] public Animator anim;

        //[Header("Camera")]
      

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            anim = GetComponentInChildren<Animator>();            
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;           

            SwitchState(Idle);
        }

        private void Update()
        {
            Move();
            Gravity();

            anim.SetFloat("xInput", xInput);
            anim.SetFloat("zInput", zInput);

            currentState.UpdateState(this);
        }
        private void LateUpdate()
        {
            
        }
        public void Move()
        {
            xInput = Input.GetAxis("Horizontal");
            zInput = Input.GetAxis("Vertical");

            dir = transform.forward * zInput + transform.right * xInput;
            controller.Move(dir.normalized * currentMoveSpeed * Time.deltaTime);
        }      

        public void SwitchState(MovementBaseState state)
        {
            currentState = state;
            currentState.EnterState(this);
        }

        private bool IsGrounded()
        {
            spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
            if (Physics.CheckSphere(spherePos, controller.radius - 0.05f, groundMask)) return true;
            return false;
        }

        private void Gravity()
        {
            if (!IsGrounded()) velocity.y += gravity * Time.deltaTime;
            else if (velocity.y < 0) velocity.y = -2;

            controller.Move(velocity * Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spherePos, controller.radius - 0.05f);
        }
    }
}