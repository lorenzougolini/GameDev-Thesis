using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FormController : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI ButtonText;

    [SerializeField] private GameObject question1;
    [SerializeField] private GameObject question2;
    [SerializeField] private GameObject question3;
    [SerializeField] private GameObject question4;
    [SerializeField] private GameObject question5;
    [SerializeField] private GameObject question6;
    [SerializeField] private GameObject question7;
    [SerializeField] private GameObject question8;
    [SerializeField] private GameObject question9;
    [SerializeField] private GameObject question10;
    
    public GameObject waitingRoom;
    public GameObject loadingText;
    public GameObject startingText;
    public GameObject loadingSpinner;

    private TextMeshProUGUI mainText;
    private TextMeshProUGUI dotsText;

    [Range(10, 15)] public float waitingTime;

    public FormTelemetry.FormTelemetryStruct formTelemetryStruct;

    private void Start()
    {
        if (GameIdController.RoundNumber == 3)
            ButtonText.text = "End Game";
        else
            ButtonText.text = "Continue";

        formTelemetryStruct.matchID = GameIdController.gameId;
        formTelemetryStruct.roundNumber = GameIdController.RoundNumber.ToString();

        mainText = loadingText.transform.Find("MainText").GetComponent<TextMeshProUGUI>();
        dotsText = loadingText.transform.Find("DotsText").GetComponent<TextMeshProUGUI>();
    }

    private void FillStructure()
    {
        // question1 gets anwered with a slider 1 to 5
        formTelemetryStruct.answer0 = question1.GetComponentInChildren<Slider>().value.ToString();
        formTelemetryStruct.answer1 = question2.GetComponentInChildren<Slider>().value.ToString();
        formTelemetryStruct.answer2 = question3.GetComponentInChildren<Slider>().value.ToString();
        formTelemetryStruct.answer3 = question4.GetComponentInChildren<Slider>().value.ToString();
        formTelemetryStruct.answer4 = question5.GetComponentInChildren<Slider>().value.ToString();

        // question6 has two checkboxes (yes, no)
        if (question6.transform.GetChild(2).GetComponent<Toggle>().isOn)
            formTelemetryStruct.answer5 = "Yes";
        else
            formTelemetryStruct.answer5 = "No";

        formTelemetryStruct.answer6 = question7.GetComponentInChildren<Slider>().value.ToString();
        formTelemetryStruct.answer7 = question8.GetComponentInChildren<Slider>().value.ToString();
        if (question9.transform.GetChild(2).GetComponent<Toggle>().isOn)
            formTelemetryStruct.answer8 = "Yes";
        else
            formTelemetryStruct.answer8 = "No";
        formTelemetryStruct.answer9 = question10.GetComponentInChildren<Slider>().value.ToString();
    }

    public void ButtonPressed()
    {
        FillStructure();
        StartCoroutine(SubmitAndLoad());
    }

    IEnumerator SubmitAndLoad()
    {
        yield return StartCoroutine(FormTelemetry.SubmitFeedbackForm(formTelemetryStruct));
        
        if (GameIdController.RoundNumber == 3)
        {
            SceneManager.LoadScene("MenuScene");
        }
        else
        {
            GameIdController.SetRoundNumber();
            // SceneManager.LoadScene("PlayScene");
            SceneManager.LoadScene("PlayScene");
        }
    }
}
