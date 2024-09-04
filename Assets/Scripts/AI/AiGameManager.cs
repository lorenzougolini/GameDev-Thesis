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
            EndGame();
        }
    }

    void EndGame()
    {
        OnGameEnd?.Invoke();
        elapsedTime = 0f;
    }
}
