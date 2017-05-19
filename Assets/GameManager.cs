using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    public GameObject BalloonPrefab;
    public BoxCollider BalloonSpawnArea;

    public float GameLength = 30;
    public int VisibleTargets = 10;

    public AudioSource StartingAudioSource;
    public AudioSource TickingAudioSouce;
    public AudioSource FinishedAudioSource;

    [HideInInspector]
    [SyncVar(hook = "OnSecondsRemainingChanged")]
    public int SecondsRemaining = -1;

    float gameStartedAt = -1;
    Target startGameTarget;

    public override void OnStartClient()
    {
        base.OnStartClient();

        startGameTarget = GameObject.FindObjectOfType<StartGameTarget>();

        if (isServer)
            updateTargets();
    }

    // Tick
    void Update()
    {
        // Check for ganme end
        if (isServer && gameStartedAt > 0)
        {
            float endsAt = gameStartedAt + GameLength;
            SecondsRemaining = (int)(endsAt - Time.time);
            if (Time.time >= endsAt)
            {
                EndGame();
            }
        }

        // Update ticking clients
        bool shouldBeTicking = SecondsRemaining <= 5 && gameStartedAt > 0;
        if (TickingAudioSouce.isPlaying != shouldBeTicking)
        {
            if (shouldBeTicking)
                TickingAudioSouce.Play();
            else
                TickingAudioSouce.Stop();
        }
	}

    // Play Start/stop sounds
    void OnSecondsRemainingChanged(int seconds)
    {
        // Starting
        if (SecondsRemaining <= 0 && seconds > 0)
            StartingAudioSource.Play();
        // Finished
        else if (SecondsRemaining > -1 && seconds <= -1)
            FinishedAudioSource.Play();

        SecondsRemaining = seconds;
    }

    // Start the game
    [Server]
    public void StartGame()
    {
        gameStartedAt = Time.time;
        ResetScores();
        ResetTargets();
        updateTargets();
    }

    // Update target visibilty
    [Server]
    void updateTargets()
    {
        bool gameRunning = gameStartedAt > 0;

        Target[] allTargets = GameObject.FindObjectsOfType<Target>();

        Target[] targets = (Target[])allTargets.Clone();
        randomizeArray(targets);
        
        int visibleTargets = 0;
        for (int i = 0; i < targets.Length; i++)
        {
            Target target = targets[i];
            if (target == startGameTarget)
                target.Visible = gameRunning == false;
            else
                target.Visible = gameRunning && visibleTargets++ < VisibleTargets;
        }

    }

    // Reset Scores
    [Server]
    void ResetScores()
    {
        PlayerController[] players = GameObject.FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in players)
        {
            player.Score = 0;
        }
    }

    // Reset Targets
    [Server]
    void ResetTargets()
    {
        Target[] targets = GameObject.FindObjectsOfType<Target>();
        foreach (Target target in targets)
        {
            target.Reset();
        }
    }

    // End the game
    void EndGame()
    {
        SecondsRemaining = -1;
        gameStartedAt = 0;
        updateTargets();
        RpcSpawnBalloons();
    }

    // Spawn balloons for their score
    [ClientRpc]
    void RpcSpawnBalloons()
    {
        int score = 0;
        PlayerController[] players = GameObject.FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in players)
        {
            if (player.isLocalPlayer)
            score = player.Score;
        }
        for (int i=0; i< score/10; i++)
        {
            // TODO move to util
            Bounds bounds = BalloonSpawnArea.GetComponent<Collider>().bounds;
            Vector3 position = bounds.center;
            position.x += Random.Range(-1f, 1f) * bounds.extents.x;
            position.y += Random.Range(-1f, 1f) * bounds.extents.y;
            position.z += Random.Range(-1f, 1f) * bounds.extents.z;
            GameObject balloonGO = Instantiate<GameObject>(BalloonPrefab, position, Quaternion.identity);
        }
    }

    // move to utils
    void randomizeArray<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int r = Random.Range(0, i);
            T temp = array[i];
            array[i] = array[r];
            array[r] = temp;
        }
    }
    bool arrayContains<T>(T[] array, T obj)
    {
        for (int i = 0; i < array.Length; i++)
            if (array[i].Equals(obj))
                return true;
        return false;
    }
}
