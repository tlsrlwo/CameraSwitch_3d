using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CST
{
    public class RunState : MovementBaseState
    {
        public override void EnterState(PlayerMove movement)
        {
            movement.anim.SetBool("Running", true);
        }
        public override void UpdateState(PlayerMove movement)
        {
            if (Input.GetKeyUp(KeyCode.LeftShift)) ExitState(movement, movement.Walk);
            else if (movement.dir.magnitude < 0.1f) ExitState(movement, movement.Idle);

            if (movement.zInput < 0) movement.currentMoveSpeed = movement.runBackSpeed;
            else movement.currentMoveSpeed = movement.runSpeed;
        }

        void ExitState(PlayerMove movement, MovementBaseState state)
        {
            movement.anim.SetBool("Running", false);
            movement.SwitchState(state);

        }
    }
}