using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AiGameManager : MonoBehaviour
{
    public float elapsedTime;
    public float timeRemaining;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float gameDuration;

    public int roundNumber = 1;
    public event Action OnRoundEnd;
    public event Action OnGameEnd;

    // generate instance
    public static AiGameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        timeRemaining = gameDuration - elapsedTime;

        timerText.text = timeRemaining.ToString("F1");

        if (elapsedTime >= gameDuration)
        {
            EndRound();
        }
    }

    void EndRound()
    {   
        OnRoundEnd?.Invoke();

        // if it's the third round, end the game
        if (roundNumber == 3)
        {
            EndGame();
        }
        else
        {
            roundNumber += 1;
            elapsedTime = 0f;
        }
    }

    void EndGame()
    {
        roundNumber = 1;
        elapsedTime = 0f;
        OnGameEnd?.Invoke();
    }
}
