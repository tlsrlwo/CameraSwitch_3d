using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementBaseState
{
    public abstract void EnterState(PlayerMove movement);
    public abstract void UpdateState(PlayerMove movement);
}
