using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(TextMesh))]
[RequireComponent(typeof(Floater))]
public class ScorePopcorn : NetworkBehaviour
{
    public Vector2 ScoreRange = new Vector2(0, 50);
    public Color lowColor = Color.yellow;
    public Color highColor = Color.red;

    [HideInInspector]
    [SyncVar]
    public int Score;
    [HideInInspector]
    [SyncVar]
    public GameObject Player;

    public override void OnStartClient()
    {
        base.OnStartClient();

        PlayerController pc = Player.GetComponent<PlayerController>();

        TextMesh text = GetComponent<TextMesh>();
        text.text = "+" + Score;
        if (pc.isLocalPlayer)
        {
            float lerp = (Score - ScoreRange.x) / (ScoreRange.y - ScoreRange.x);
            text.color = Color.Lerp(lowColor, highColor, lerp);
        }
        else
            text.color = new Color(pc.hatColor.r, pc.hatColor.g, pc.hatColor.b, 0.3f);

        float initialWander = 2;
        Floater floater = GetComponent<Floater>();
        floater.Velocity = new Vector3(Random.Range(-initialWander, initialWander), Random.Range(1f, 5f), Random.Range(-initialWander, initialWander));
    }
}
