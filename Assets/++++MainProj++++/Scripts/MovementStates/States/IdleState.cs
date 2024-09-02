using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CST
{
    public class IdleState : MovementBaseState
    {
        public override void EnterState(PlayerMove movement)
        {

        }
        public override void UpdateState(PlayerMove movement)
        {
            if (movement.dir.magnitude > 0.1f)
            {
                if (Input.GetKey(KeyCode.LeftShift)) movement.SwitchState(movement.Run);
                else movement.SwitchState(movement.Walk);
            }
        }
    }
}