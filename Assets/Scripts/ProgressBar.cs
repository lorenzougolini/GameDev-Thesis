using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    void GetCurrentFill() {
        float fillAmount = (float) current / (float) max;
        mask.fillAmount = fillAmount;
    }

    void CheckPulsatingEffect() {
        if (current >= max) {
            associatedPlayer.GetComponent<PlayerMovement>().powerReady = true;
            pulsatingEffect.StartPulsating(powerText.gameObject, 1f, 2f, 5f);
        } else {
            associatedPlayer.GetComponent<PlayerMovement>().powerReady = false;
            pulsatingEffect.StopPulsating();
        }
    }
}
