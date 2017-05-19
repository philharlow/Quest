using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkAttach : NetworkBehaviour
{
    [SyncVar]
    public GameObject AttachParent;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (AttachParent)
        {
            MovingTarget moving = AttachParent.GetComponent<MovingTarget>();
            if (moving)
                transform.parent = moving.TargetRoot.transform;
            else
                transform.parent = AttachParent.transform;
        }
    }
}
