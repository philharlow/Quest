using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkColor : NetworkBehaviour
{
    public MeshRenderer[] Meshes;

    [SyncVar(hook = "OnColorChanged")]
    Color color;

    [Server]
    public void SetColor(Color newColor)
    {
        color = newColor;
    }

    void OnColorChanged(Color newColor)
    {
        color = newColor;
        foreach (MeshRenderer mesh in Meshes)
            mesh.material.color = color;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        OnColorChanged(color);
    }

}
