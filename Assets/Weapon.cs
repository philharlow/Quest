using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Weapon : MonoBehaviour
{
    public GameObject BulletPrefab;
    public LayerMask RaycastMask;
    public Transform barrelEnd;
    public float Range = 50;

    PlayerController player;

    void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        if (player == null || player.isLocalPlayer == false)
            return;

        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            Vector3 endPoint;
            
            //int mask = ~(1 << LayerMask.NameToLayer("LocalPlayer")); // TODO use LayerMask
            Transform cameraTrans = player.camera.transform;
            if (Physics.Raycast(cameraTrans.position, cameraTrans.forward, out hit, Range, RaycastMask))
                endPoint = hit.point;
            else
                endPoint = cameraTrans.position + Range * cameraTrans.forward;

            player.CmdFire(endPoint);
        }
    }
}
