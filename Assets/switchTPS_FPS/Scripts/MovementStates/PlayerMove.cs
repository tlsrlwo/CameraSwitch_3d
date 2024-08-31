using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float currentMoveSpeed;
    public float walkSpeed = 3, walkBackSpeed = 2;
    public float runSpeed = 7, runBackSpeed = 5;
    public float crouchSpeed = 2, crouchBackSpeed = 1;
    public float airSpeed = 1.5f;

    [Header("Move")]
    public float xInput, zInput;
    [HideInInspector]public Vector3 dir;

    [Header("GroundCheck")]
    public Vector3 spherePos;
    public LayerMask groundMask;
    public float groundYOffset = 0.01f;

    public float gravity = -9.81f;
    private Vector3 velocity;

    CharacterController controller;

    public MovementBaseState currentState;
    public IdleState Idle = new IdleState();
    public RunState Run = new RunState();
    public WalkState Walk = new WalkState();

    [HideInInspector] public Animator anim;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

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
        if (Physics.CheckSphere(spherePos, controller.radius - groundYOffset, groundMask)) return true;
        return false;
    }

    private void Gravity()
    {
        if (!IsGrounded()) velocity.y += gravity * Time.deltaTime;
        else if (velocity.y < 0) velocity.y = -2;

        controller.Move(velocity * Time.deltaTime);
    }



}
