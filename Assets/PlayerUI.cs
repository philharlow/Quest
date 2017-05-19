using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Text Score;
    public Text Time;

    [HideInInspector]
    public PlayerController player;
    
    GameManager gameManager;

    void Start ()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        SetTime(gameManager.SecondsRemaining);
        SetScore(0);
    }
	
	// Update is called once per frame
	void Update ()
    {
        // TODO: setup events
        if (player)
            SetScore(player.Score);

        if (gameManager)
            SetTime(gameManager.SecondsRemaining);
    }

    void SetScore(int score)
    {
        Score.text = "Score: " + score;
    }

    void SetTime(int seconds)
    {
        bool stopped = seconds == -1;
        seconds = Mathf.Max(0, seconds);
        int min = seconds / 60;
        int sec = seconds % 60;

        Time.text = "Time: " + min + ":" + (sec < 10 ? "0" : "") + sec;
        Time.color = seconds > 10 ? Color.white : Color.red;
        Time.enabled = stopped == false;
    }
}
