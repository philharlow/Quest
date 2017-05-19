using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Target : NetworkBehaviour
{
    public GameObject PopcornPrefab;
    public Transform TargetRoot;
    public GameObject SoundPrefab;
    public AudioClip Clip;
    public bool DestroyOnHit = true;
    public int Points = 10;
    public float BullseyeRadius = 0.1f;

    [SyncVar(hook="OnVisibleChanged")]
    public bool Visible;

    private void Start()
    {
        if (isServer)
            resetColor();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        OnVisibleChanged(Visible);
    }
    
    public virtual void onHit(Bullet bullet, Vector3 hitPosition)
    {
        // Destroy self
        if (DestroyOnHit)
            Destroy(gameObject);

        // Only run the rest on the server
        if (isServer == false)
            return;

        // Random color
        if (GetComponent<NetworkColor>())
            GetComponent<NetworkColor>().SetColor(Random.ColorHSV());
        
        // Give points
        int points = GetPoints(hitPosition);
        if (points > 0)
        {
            bullet.player.Score += points;

            // Spawn popcorn
            GameObject popcornGO = Instantiate<GameObject>(PopcornPrefab);
            popcornGO.transform.position = hitPosition;

            ScorePopcorn popcorn = popcornGO.GetComponent<ScorePopcorn>();
            popcorn.Score = points;
            popcorn.Player = bullet.player.gameObject;

            NetworkServer.Spawn(popcornGO);
        }

        // Play sound
        if (SoundPrefab && Clip)
        {
            GameObject soundGO = Instantiate<GameObject>(SoundPrefab);
            AudioSource audio = soundGO.GetComponent<AudioSource>();
            audio.clip = Clip;
            audio.pitch = Points == 0 ? 1 : 1 + points / (float)Points;
            audio.Play();
            Destroy(soundGO, 3);
        }
    }
    

    virtual public bool CanBeHit()
    {
        return true;
    }

    virtual public int GetPoints(Vector3 hitPosition)
    {
        if (Points == 0)
            return 0;

        Vector3 localCoord = transform.InverseTransformPoint(hitPosition);
        Vector2 local2d = new Vector2(localCoord.x, localCoord.y);
        float distanceToCenter = local2d.magnitude;
        // Bullseye
        if (distanceToCenter < BullseyeRadius)
            return Points;

        return (int)(Mathf.Clamp((1 - distanceToCenter) * Points, 1, Points));
    }

    virtual public void Reset()
    {
        resetColor();
        RpcReset();
    }

    [ClientRpc]
    void RpcReset()
    {
        // Unity doesnt seem to like virtual rpcs?
        onRpcReset();
    }

    virtual public void onRpcReset()
    {

    }

    void resetColor()
    {
        // Set back to brown
        if (GetComponent<NetworkColor>())
            GetComponent<NetworkColor>().SetColor(new Color(0.816f, 0.69f, 0.529f));
    }


    void OnVisibleChanged(bool visible)
    {
        Visible = visible;

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
            collider.enabled = visible;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
            renderer.enabled = visible;
    }
}
