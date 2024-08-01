using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour {

    public int max;
    public float current;

    public GameObject associatedPlayer;

    public Image mask;
    public GameObject powerText;
    public PulsatingEffect pulsatingEffect;


    // Start is called before the first frame update
    void Start() {
        current = 0;
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
        CheckPulsatingEffect();
    }

    public void UpdateCurrent(float i) {
        if (current < max)
            current += i;
        else
            current = max;
    }

    public void SetCurrent(float i)
    {
        current = i;
    }

    void GetCurrentFill() {
        float fillAmount = (float) current / (float) max;
        mask.fillAmount = fillAmount;
    }

    void CheckPulsatingEffect() {
        bool powerReady = current >= max;
        if (associatedPlayer.TryGetComponent(out PlayerMovement playerMovement)) {
            playerMovement.powerReady = powerReady;
        } else if (associatedPlayer.TryGetComponent(out Bot bot)) {
            bot.powerReady = powerReady;
        } else if (associatedPlayer.TryGetComponent(out AgentController agentController)) {
            agentController.powerReady = powerReady;
        }
        
        if (powerReady) {
            pulsatingEffect.StartPulsating(powerText.gameObject, 1f, 2f, 5f);
        } else {
            pulsatingEffect.StopPulsating();
        }
    }
}
