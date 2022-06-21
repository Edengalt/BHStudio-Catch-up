using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    public Transform target;
    [SerializeField] private float distance = 5;
    [SerializeField] private float camHeight = 3;
   // [SerializeField] private float mouseSensitivity = 2;

   private float damping = 5000;

   private Quaternion currentRotationY;
   private Quaternion targetRotation;

    void Awake()
    {
        currentRotationY = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
    private void Update()
    {
        targetRotation = Quaternion.Euler(0, (currentRotationY).eulerAngles.y,0);
        Vector3 offset = targetRotation * (-Vector3.forward * distance + (Vector3.up * camHeight));
        transform.position = target.transform.position + offset;
        
        float Xrot = Quaternion.LookRotation(target.transform.position - transform.position).eulerAngles.x;
        targetRotation = Quaternion.Euler(Xrot, (currentRotationY).eulerAngles.y,0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.smoothDeltaTime * damping);
        
    }

    public void Rollout()
    {
        Destroy(gameObject);
    }
    
    public void Move(float yDelta)
    {
        currentRotationY = currentRotationY * Quaternion.Euler(0, yDelta, 0);
    }

    
    
}

