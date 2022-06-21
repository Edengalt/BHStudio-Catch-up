using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class AnimatedEntity : InitedBehaviour, IAnimated
{
    protected Animator animator;

    public abstract void SetupAnimator();

    public void StopAnimation()
    {
        animator.StopPlayback();
    }

    public void StartAnim()
    {
        animator.StartPlayback();
        
    }
    public void StartAnim(string animName)
    {
        animator.Play(animName);
    }

    public void SetBool(string name, bool state)
    {
        animator.SetBool(name, state);
    }
    
    public void SetTrigger(string name)
    {
        animator.SetTrigger(name);
    }

    public void SetFloat(string name, float value)
    {
        animator.SetFloat(name,value);
    }
    
    public override void Initing()
    {
        animator = GetComponent<Animator>();
        SetupAnimator();
        Debug.Log("Animator inited" + animator);
    }
}
