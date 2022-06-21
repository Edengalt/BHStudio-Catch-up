using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Mirror;
using UnityEngine;

public class PlayerColorController : InitedNetworkBehaviour, IInitable
{
    [SerializeField] private SkinnedMeshRenderer mesh;

    private Material mainMaterial;

    [SyncVar(hook = nameof(ColorChange))]private Color color = Color.cyan;

    private void OnEnable()
    {
        mainMaterial = mesh.material;
    }

    public override void Initing()
    {
        mainMaterial.color = Color.cyan;
    }

    public void ColorChange(Color _old, Color _new)
    {
        mainMaterial.color = _new;
    }

    public void Coloring(bool stunned)
    {
        if (isServer)
            Colorize(stunned);
        if (isClient)
            CmdColoring(stunned);
        
    }
    
    [Server]
    public void Colorize(bool stunned)
    {
        mainMaterial.color = stunned ? GlobalGameSettings.Instance.stunColor : Color.cyan;
        color = mainMaterial.color;
    }
    
    [Command(requiresAuthority = false)]
    public void CmdColoring(bool stunned)
    {
        Colorize(stunned);
    }
}
