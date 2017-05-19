using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovingTarget : Target
{
    public Transform HiddenTransform;

    public float LerpSeconds = 0.5f;
    public float ResetSeconds = 3;

    [SyncVar(hook = "OnHiddenChanged")]
    bool hidden = false;

    float lerp;

    float resetsAt;
    float lastLerp;

    // Only on the server
    override public void onHit(Bullet bullet, Vector3 hitPosition)
    {
        // Prevent hits while hiding
        if (hidden)
            return;

        base.onHit(bullet, hitPosition);
        hidden = true;
    }

    void OnHiddenChanged(bool newHidden)
    {
        hidden = newHidden;
        if (hidden)
            resetsAt = Time.time + ResetSeconds;
    }

    void Update()
    {
        // Check for reset
        if (isServer && resetsAt > 0 && resetsAt < Time.time)
        {
            hidden = false;
            resetsAt = 0;
        }

        // Update lerp/transform
        lerp = Mathf.Clamp01(lerp + ((hidden ? 1 : -1) * Time.deltaTime / LerpSeconds));
        if (lerp != lastLerp)
        {
            TargetRoot.transform.rotation = Quaternion.Lerp(transform.rotation, HiddenTransform.rotation, lerp);
            TargetRoot.transform.position = Vector3.Lerp(transform.position, HiddenTransform.position, lerp);
            lastLerp = lerp;
        }
    }

    override public bool CanBeHit()
    {
        return hidden == false;
    }

    override public void onRpcReset()
    {
       base.onRpcReset();

        lerp = 0;
        resetsAt = 0;
    }
}
