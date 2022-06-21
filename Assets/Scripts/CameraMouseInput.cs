using System;
using Mirror;
using UnityEngine;

public class CameraMouseInput : NetworkBehaviour
{
    private CameraController _cam;

    [Range(0.1f, 10)] public float mouseSensitivity = 2.5f;
    private bool lockCamera = false;
    private void OnEnable()
    {
        _cam = GetComponent<CameraController>();
        mouseSensitivity = GlobalGameSettings.Instance.mouseSensitivity;
        Cursor.lockState = CursorLockMode.Locked;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.None;
            lockCamera = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.Locked;
            lockCamera = false;
        }
        if(lockCamera) return;
        float moveDelta = Input.GetAxis("Mouse X") * mouseSensitivity ;
        _cam.Move(moveDelta);
        
    }
}