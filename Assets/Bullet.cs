using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
    public GameObject SplatPrefab;

    [HideInInspector]
    public PlayerController player;
 
    private void OnCollisionEnter(Collision collision)
    {
        Target target = collision.gameObject.GetComponentInParent<Target>();
        if (target)
            target.onHit(this, collision.contacts[0].point);

        // Destroy self
        Destroy(gameObject);

        // Run the rest only on the server
        if (isServer == false)
            return;

        //Debug.Log("OnCollisionEnter: " + collision.gameObject);

        // Spawn splat
        if (collision.gameObject.tag != "nosplat")
        {
            GameObject splat = Instantiate<GameObject>(SplatPrefab);
            splat.transform.position = collision.contacts[0].point + 0.01f * collision.contacts[0].normal;
            SetGlobalScale(splat.transform, new Vector3(0.05f, 1, 0.05f));
            splat.transform.rotation = Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal);
            //splat.transform.Rotate(splat.transform.forward, Random.Range(0, 360));

            // Attach to target because they can move
            MovingTarget moving = collision.gameObject.GetComponentInParent<MovingTarget>();
            NetworkAttach attach = splat.GetComponent<NetworkAttach>();
            if (moving)
                attach.AttachParent = moving.gameObject;
            else if (collision.gameObject.GetComponent<PlayerController>())
                attach.AttachParent = collision.gameObject;

            NetworkColor color = splat.GetComponent<NetworkColor>();
            color.SetColor(player.hatColor);

            NetworkServer.Spawn(splat);

            Destroy(splat, 20);
        }
    }
    
    public static void SetGlobalScale(Transform transform, Vector3 globalScale)
    {
        transform.localScale = Vector3.one;
        transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
    }
}
