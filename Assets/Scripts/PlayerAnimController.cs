using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class PlayerAnimController : AnimatedEntity
{
    private string VELOCITY = "Velocity";
    private string DASH = "Dash";


    public override void SetupAnimator()
    {
        SetFloat(VELOCITY, 0);
    }
    
    public void SetVelocity(float value)
    {
        SetFloat(VELOCITY, value);
    }

    public void Dash(bool state)
    {
        SetBool(DASH, state);
    }
}
