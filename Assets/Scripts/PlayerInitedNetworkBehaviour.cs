using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Interfaces;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public enum State
{
    Stunned = 0,
    Active = 1,
    Dash = 2,
    
}
public class PlayerInitedNetworkBehaviour : InitedNetworkBehaviour, ICollisionInteractiveWithEndAction
{
    [SyncVar(hook = nameof(OnStateChanged))]private State state = State.Active;
    
    [SerializeField] private CameraController cameraPrefab;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float speedDynamicChange;
    [SerializeField] private float maxSpeed;
    [Space] // Decided to make Dash realisation based on physics, not on transform.position teleportation
    [Range(1,10)][SerializeField] private float dashRange;
    [SerializeField] private float dashCooldown;

    [SyncVar]public string ID;
    
    private PlayerAnimController animator;
    private Rigidbody rigid;
    private CollisionTracker collisionTracker;
    private Transform camera;
    private Vector3 direction = Vector3.zero;
    private float maxSpeedBackup;
    private float dashCooldownTimer;
    private float dashDuration = 1f;
    private float maxDashSpeed
    {
        get
        {
            float value = dashSpeed * 3;
            return value;
        }
    }
    private float dashSpeed
    {
        get
        {
            float value = dashRange * 3;
            return value;
        }
    }
    
    
    public override void Initing()
    {
        dashRange = GlobalGameSettings.Instance.dashRange;
        dashDuration = 0.2f + (dashRange / 10f);
        animator = GetComponentInChildren<PlayerAnimController>();
        rigid = GetComponent<Rigidbody>();
        collisionTracker = GetComponent<CollisionTracker>();
        Debug.Log("player login inited");
        EventManager.Instance.AddListener<SessionEnded>(OnSessionEnded);
        EventManager.Instance.AddListener<SessionStarted>(OnSessionStarted);
    }
    
    public override void OnStartAuthority()
    {
        CameraController _cam = Instantiate(cameraPrefab);
        _cam.target = cameraTarget;
        camera = _cam.transform;
        ID = "Player" + Random.Range(0, 999);
        EventManager.Instance.Raise(new AddPlayer(ID, this));
    }
    
    void FixedUpdate()
    {
        if(!isLocalPlayer)  return;
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.fixedDeltaTime;
        if (Input.anyKey)
        {
            direction = Vector3.zero;
            switch (state)
            {
                case State.Stunned:
                    break;
                case State.Active:
                    if (Input.GetKey(KeyCode.W))
                        direction += Vector3.ProjectOnPlane(camera.forward,Vector3.up).normalized;
                    if (Input.GetKey(KeyCode.S))
                        direction -= Vector3.ProjectOnPlane(camera.forward,Vector3.up).normalized;
                    if (Input.GetKey(KeyCode.A))
                        direction -= Vector3.ProjectOnPlane(camera.right,Vector3.up).normalized;
                    if (Input.GetKey(KeyCode.D))
                        direction += Vector3.ProjectOnPlane(camera.right, Vector3.up).normalized;
                    if (Input.GetKey(KeyCode.Mouse0) && dashCooldownTimer <= 0)
                    {
                        DashBegin(direction);
                        return;
                    }
                    break;
                case State.Dash:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (direction != Vector3.zero)
            {
                rigid.AddForce(direction.normalized * speedDynamicChange);
                rigid.velocity = Vector3.ClampMagnitude(rigid.velocity, maxSpeed);
            }
            
            
        }
        if(direction == Vector3.zero || rigid.velocity.magnitude < 0.1f)
        {
            animator.SetVelocity(0);
            return;
        }
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(direction), 0.1f);
        animator.SetVelocity(rigid.velocity.magnitude/maxSpeed);
    }

    #region Events

    private void OnSessionStarted(SessionStarted e)
    {
        if (NetworkManager.singleton != null)
            transform.position = NetworkManager.singleton.GetStartPosition().position;
        else
            transform.position = Vector3.zero;
        state = State.Active;
        
    }
    
    private void OnSessionEnded(SessionEnded e)
    {
        state = State.Stunned;
    }

    #endregion
    
    #region Dash
    
    public void DashBegin(Vector3 _direction)
    {
        dashCooldownTimer = dashCooldown;
        state = State.Dash;
        animator.Dash(true);
        collisionTracker.Dash(true);
        maxSpeedBackup = maxSpeed;
        maxSpeed = maxDashSpeed;
        if (_direction == Vector3.zero) _direction = transform.forward;
        rigid.AddForce(_direction * dashSpeed,ForceMode.VelocityChange);
        StartCoroutine(DashRoutine());
    }

    public IEnumerator DashRoutine()
    {
        yield return new WaitForSecondsRealtime(dashDuration);
        DashEnd();
    }

    public void DashEnd()
    {
        animator.Dash(false);
        collisionTracker.Dash(false);
        maxSpeed = maxSpeedBackup;
        StopAllCoroutines();
        rigid.velocity = Vector3.zero;
        state = State.Active;
    }
    #endregion
    
    public void OnStateChanged(State _old, State _new)
    {
        state = _new;
    }
    
    [Server]
    public void ChangeState(State state)
    {
        this.state = state;
    }
    
    [Command(requiresAuthority = false)]
    public void CmdChangeState(State state)
    {
        ChangeState(state);
    }
    
    public void OnCollision(CollisionTracker col)
    {
        if(state == State.Dash) return;
        
        col.CanStun();
        if(isServer)
            ChangeState(State.Stunned);
        if(isClient)
            CmdChangeState(State.Stunned);
    }

    public void EndAction()
    {
        if(isServer)
            ChangeState(State.Active);
        if(isClient)
            CmdChangeState(State.Active);
    }

    private void OnDestroy()
    {
        //Destroy(camera.gameObject);
    }
}
