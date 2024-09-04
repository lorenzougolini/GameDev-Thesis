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
    
    public FormTelemetry.FormTelemetryStruct formTelemetryStruct;

    private void Start()
    {
        if (GameIdController.RoundNumber == 3)
            ButtonText.text = "End Game";
        else
            ButtonText.text = "Continue";

        formTelemetryStruct.matchID = GameIdController.gameId;
        formTelemetryStruct.roundNumber = GameIdController.RoundNumber.ToString();
    }

    private void FillStructure()
    {
        // question1 gets anwered with a slider 1 to 5
        formTelemetryStruct.answer1 = question1.GetComponentInChildren<Slider>().value.ToString();
        formTelemetryStruct.answer2 = question2.GetComponentInChildren<Slider>().value.ToString();
        formTelemetryStruct.answer3 = question3.GetComponentInChildren<Slider>().value.ToString();
        formTelemetryStruct.answer4 = question4.GetComponentInChildren<Slider>().value.ToString();
        formTelemetryStruct.answer5 = question5.GetComponentInChildren<Slider>().value.ToString();

        // question6 has two checkboxes (yes, no)
        if (question6.transform.GetChild(2).GetComponent<Toggle>().isOn)
            formTelemetryStruct.answer6 = "Yes";
        else
            formTelemetryStruct.answer6 = "No";

        formTelemetryStruct.answer7 = question7.GetComponentInChildren<Slider>().value.ToString();
        formTelemetryStruct.answer8 = question8.GetComponentInChildren<Slider>().value.ToString();
        if (question9.transform.GetChild(2).GetComponent<Toggle>().isOn)
            formTelemetryStruct.answer9 = "Yes";
        else
            formTelemetryStruct.answer9 = "No";
        formTelemetryStruct.answer10 = question10.GetComponentInChildren<Slider>().value.ToString();
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
            SceneManager.LoadScene("PlayScene");
        }
    }
}
