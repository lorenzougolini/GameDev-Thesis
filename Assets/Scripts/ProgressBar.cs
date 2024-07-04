using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour {

    public int max;
    public float current;
    public Image mask;
    public PulsatingEffect pulsatingEffect;

    // Start is called before the first frame update
    void Start()
    {
        current = 0;
        // if (pulsatingEffect = null) 
        //     pulsatingEffect = GetComponent<PulsatingEffect>();
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
        if (current >= max) 
            pulsatingEffect.StartPulsating();
        else
            pulsatingEffect.StopPulsating();
    }
}
