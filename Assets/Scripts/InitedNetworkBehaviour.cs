using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Mirror;
using UnityEngine;

public abstract class InitedNetworkBehaviour :  NetworkBehaviour, IInitable
{
    public void Init()
    {
        Initing();
    }

    public abstract void Initing();
}

public abstract class InitedBehaviour :  MonoBehaviour, IInitable
{

    public void Init()
    {
        Initing();
    }

    public abstract void Initing();
}
