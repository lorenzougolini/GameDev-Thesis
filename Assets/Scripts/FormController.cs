using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FormController : MonoBehaviour
{
    public int _currentQuestion = 1;

    [SerializeField] public TextMeshProUGUI ButtonText;

    [SerializeField] private GameObject questionView1;
    [SerializeField] private GameObject questionView2;
    [SerializeField] private GameObject questionView3;

    [SerializeField] private GameObject questionList1;
    [SerializeField] private GameObject questionList2;
    [SerializeField] private GameObject question3;
    [SerializeField] private TextMeshProUGUI ErrorMsg1;
    [SerializeField] private TextMeshProUGUI ErrorMsg2;
    [SerializeField] private TextMeshProUGUI ErrorMsg3;

    private List<ToggleGroup> toggleGroups1;
    private List<ToggleGroup> toggleGroups2;

    // [SerializeField] private ToggleGroup question1_1;
    // [SerializeField] private ToggleGroup question1_2;
    // [SerializeField] private ToggleGroup question1_3;
    // [SerializeField] private ToggleGroup question1_4;
    // [SerializeField] private ToggleGroup question1_5;
    // [SerializeField] private ToggleGroup question1_6;
    // [SerializeField] private ToggleGroup question1_7;
    // [SerializeField] private ToggleGroup question1_8;
    // [SerializeField] private ToggleGroup question1_9;
    // [SerializeField] private ToggleGroup question1_10;
    // [SerializeField] private ToggleGroup question1_11;
    // [SerializeField] private ToggleGroup question1_12;
    // [SerializeField] private ToggleGroup question1_13;
    // [SerializeField] private ToggleGroup question1_14;

    public FormTelemetry.FormStruct formTelemetryStruct;

    private void Start()
    {
        toggleGroups1 = questionList1.GetComponentsInChildren<ToggleGroup>().ToList();
        toggleGroups2 = questionList2.GetComponentsInChildren<ToggleGroup>().ToList();
        
        // if (GameIdController.RoundNumber == 3)
        //     ButtonText.text = "End Game";
        // else
        //     ButtonText.text = "Continue";

        formTelemetryStruct.matchID = GameIdController.gameId;
    }

    private bool FillStructure()
    {
        if (_currentQuestion == 1)
        {
            for (int i = 0; i < toggleGroups1.Count; i++)
            {
                ToggleGroup toggleGroup = toggleGroups1[i];
                Toggle selectedToggle = toggleGroup.ActiveToggles().FirstOrDefault();
                if (selectedToggle != null)
                {
                    formTelemetryStruct.GetType().GetField("answer" + _currentQuestion.ToString() + "_" + (i + 1)).SetValue(formTelemetryStruct, selectedToggle.name);
                }
                else
                {
                    ErrorMsg1.text = "Please answer all questions";
                    return false;
                }
            }
            return true;
        }

        if (_currentQuestion == 2)
        {
            for (int i = 0; i < toggleGroups2.Count; i++)
            {
                ToggleGroup toggleGroup = toggleGroups2[i];
                Toggle selectedToggle = toggleGroup.ActiveToggles().FirstOrDefault();
                if (selectedToggle != null)
                {
                    formTelemetryStruct.GetType().GetField("answer" + _currentQuestion.ToString() + "_" + (i + 1)).SetValue(formTelemetryStruct, selectedToggle.name);
                }
                else
                {
                    ErrorMsg2.text = "Please answer all questions";
                    return false;
                }
            }
            return true;
        }

        if (_currentQuestion == 3)
        {
            formTelemetryStruct.GetType().GetField("answer" + _currentQuestion.ToString()).SetValue(formTelemetryStruct, question3.GetComponent<Slider>().value.ToString());
            return true;
        }

        return false;

    }

    public void ButtonPressed()
    {
        if (_currentQuestion == 1 && FillStructure())
        {
            questionView1.SetActive(false);
            questionView2.SetActive(true);
            _currentQuestion = 2;
            return;
        }

        if (_currentQuestion == 2 && FillStructure())
        {
            questionView2.SetActive(false);
            questionView3.SetActive(true);
            _currentQuestion = 3;
            return;
        }

        if (FillStructure())
            StartCoroutine(SubmitAndLoad());
    }

    IEnumerator SubmitAndLoad()
    {
        yield return StartCoroutine(FormTelemetry.SubmitFeedbackForm(formTelemetryStruct));
        
        // if (GameIdController.RoundNumber == 3)
        // {
        //     SceneManager.LoadScene("MenuScene");
        // }
        // else
        // {
        //     GameIdController.IncrementRoundNumber();
        //     SceneManager.LoadScene("PlayScene");
        // }
        SceneManager.LoadScene("MenuScene");
    }
}
