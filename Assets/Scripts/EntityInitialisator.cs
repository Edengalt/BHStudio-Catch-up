using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Mirror;
using UnityEngine;

public class EntityInitialisator : NetworkBehaviour
{
    public override void OnStartAuthority()
    {
        List<IInitable> toInit = new List<IInitable>();
        IInitable[] components = GetComponents<IInitable>();
        IInitable[] ChildComponents = GetComponentsInChildren<IInitable>();
        toInit.AddRange(components);
        for (int i = 0; i < ChildComponents.Length; i++)
        {
            if (!toInit.Contains(ChildComponents[i]))
            {
                toInit.Add(ChildComponents[i]);
            }
        }
        foreach (var initable in toInit)
        {
            initable.Init();  
        }
        
    }
}
