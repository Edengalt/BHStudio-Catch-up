using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Its better to use scriptable objects & serialization to save data like this, but for speeding up development its left as it
/// </summary>
public class GlobalGameSettings : NetworkBehaviour
{
    private static GlobalGameSettings instance;
    public static GlobalGameSettings Instance
    {
        get
        {
            return instance;
        }
    }

    public float PlayerStunTime;
    public float MaxScore = 3;
    [Tooltip("Its not necessary to have this value here, its added for easier testing (not runtime - edited)")]
    [Range(1,10)] public float dashRange = 2f;
    [Tooltip("Its not necessary to have this value here, its added for easier testing (not runtime - edited)")]
    [Range(1,10)] public float mouseSensitivity = 3f;
    public Color stunColor;
    

    private void OnEnable()
    {
        instance = this;
    }
}
