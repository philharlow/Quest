using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameTarget : Target
{
    override public void onHit(Bullet bullet, Vector3 hitPosition)
    {
        base.onHit(bullet, hitPosition);

        if (isServer == false)
            return;

        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        gm.StartGame();
    }
}
