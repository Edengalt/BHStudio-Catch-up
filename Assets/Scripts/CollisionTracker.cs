using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Interfaces;
using Mirror;
using UnityEngine;


/// <summary>
/// Tracking collision with other players triggers and setup personal trigger during Dash
/// </summary>
public class CollisionTracker : InitedNetworkBehaviour
{
    public Collider trigger;
    private ICollisionInteractiveWithEndAction trackedObj;
    private CapsuleCollider physicalCollider;
    private Rigidbody rigid;
    private bool trackCollisions = true;
    private PlayerColorController colorController;

    private void OnEnable()
    {
        physicalCollider = GetComponent<CapsuleCollider>();
        rigid = GetComponent<Rigidbody>();
        colorController = GetComponent<PlayerColorController>();
        trackedObj = GetComponent<ICollisionInteractiveWithEndAction>();
    }

    public override void Initing()
    {
        trigger.enabled = false;
        string a = String.Format($"Collision tracker Inited {physicalCollider} {rigid} {colorController} {trackedObj}");
        Debug.Log(a);
    }

    public void Dash(bool state)
    {
        trigger.enabled = state;
    }

    private string collidedID;
    public void CanStun()
    {
        EventManager.Instance.Raise(new AddPoints(collidedID, 1));
        trackCollisions = false;
        Stunned(true);
    }
    
    public void Stunned(bool state)
    {
        rigid.isKinematic = state;
        physicalCollider.enabled = !state;
        colorController.Coloring(true);
        Debug.Log("Collision");
        StartCoroutine(Stun());
        CmdStunned(state);
    }
    
    
    
    [Command(requiresAuthority = false)]
    public void CmdStunned(bool state)
    {
        rigid.isKinematic = state;
        physicalCollider.enabled = !state;
        colorController.Coloring(true);
        StartCoroutine(Stun());
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!trackCollisions || !other.isTrigger) return;
        collidedID = other.GetComponentInParent<PlayerInitedNetworkBehaviour>().ID;
        trackedObj.OnCollision(this);
        
    }
    public IEnumerator Stun()
    {
        yield return new WaitForSecondsRealtime( GlobalGameSettings.Instance.PlayerStunTime);
        EndStun();
    }
    
    public void EndStun()
    {
        Stunned(false);
        colorController.Coloring(false);
        trackCollisions = true;
        StopAllCoroutines();
        trackedObj.EndAction();
        CmdEndStun();
    }
    
    [Command(requiresAuthority = false)]
    public void CmdEndStun()
    {
        Stunned(false);
        colorController.Coloring(false);
        trackCollisions = true;
        StopAllCoroutines();
        trackedObj.EndAction();
    }
}
