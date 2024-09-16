using TMPro;  // Make sure you have TMPro imported
using UnityEngine;
using UnityEngine.UI;

public class SliderPercentageUpdater : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Text humanPercText;
    [SerializeField] private Text aiPercText;

    private void Start()
    {
        UpdatePercentageText(slider.value);
        
        slider.onValueChanged.AddListener(UpdatePercentageText);
    }

    private void UpdatePercentageText(float sliderValue)
    {
        float humanPercentage = (sliderValue + 100) / 2;
        float aiPercentage = 100 - humanPercentage;

        humanPercText.text = $"{humanPercentage}% Human";
        aiPercText.text = $"{aiPercentage}% AI";
    }
}

