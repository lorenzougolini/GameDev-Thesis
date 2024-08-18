using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AiGameManager : MonoBehaviour
{
    public float elapsedTime;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float gameDuration;

    public event Action OnGameEnd;

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        timerText.text = (gameDuration - elapsedTime).ToString("F1");

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
