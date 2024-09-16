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

    // instantiate two list of panels
    private List<GameObject> panels1;
    private List<GameObject> panels2;

    public FormTelemetry.FormPart1 formPart1;
    public FormTelemetry.FormPart2 formPart2;
    public FormTelemetry.FormPart3 formPart3;

    private void Start()
    {
        toggleGroups1 = questionList1.GetComponentsInChildren<ToggleGroup>().ToList();
        toggleGroups2 = questionList2.GetComponentsInChildren<ToggleGroup>().ToList();
        
        panels1 = questionList1.GetComponentsInChildren<Transform>(true)
                       .Where(t => t.gameObject.name.StartsWith("PanelQ"))
                       .Select(t => t.gameObject)
                       .ToList();
        panels2 = questionList2.GetComponentsInChildren<Transform>(true)
                       .Where(t => t.gameObject.name.StartsWith("PanelQ"))
                       .Select(t => t.gameObject)
                       .ToList();
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
                    Color color = Color.white;
                    color.a = 0.5f;
                    panels1[i].GetComponent<Image>().color = color;
                    // formTelemetryStruct.GetType().GetField("answer" + _currentQuestion.ToString() + "_" + (i + 1)).SetValue(formTelemetryStruct, selectedToggle.name);
                    switch(i+1)
                    {
                        case 1: formPart1.answer1_01 = selectedToggle.name.Last().ToString(); break;
                        case 2: formPart1.answer1_02 = selectedToggle.name.Last().ToString(); break;
                        case 3: formPart1.answer1_03 = selectedToggle.name.Last().ToString(); break;
                        case 4: formPart1.answer1_04 = selectedToggle.name.Last().ToString(); break;
                        case 5: formPart1.answer1_05 = selectedToggle.name.Last().ToString(); break;
                        case 6: formPart1.answer1_06 = selectedToggle.name.Last().ToString(); break;
                        case 7: formPart1.answer1_07 = selectedToggle.name.Last().ToString(); break;
                        case 8: formPart1.answer1_08 = selectedToggle.name.Last().ToString(); break;
                        case 9: formPart1.answer1_09 = selectedToggle.name.Last().ToString(); break;
                        case 10: formPart1.answer1_10 = selectedToggle.name.Last().ToString(); break;
                        case 11: formPart1.answer1_11 = selectedToggle.name.Last().ToString(); break;
                        case 12: formPart1.answer1_12 = selectedToggle.name.Last().ToString(); break;
                        case 13: formPart1.answer1_13 = selectedToggle.name.Last().ToString(); break;
                        case 14: formPart1.answer1_14 = selectedToggle.name.Last().ToString(); break;
                    }
                
                }
                else
                {
                    ErrorMsg1.text = "Please answer all questions";
                    Color color = Color.red;
                    color.a = 0.5f;
                    panels1[i].GetComponent<Image>().color = color;
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
                    Color color = Color.white;
                    color.a = 0.5f;
                    panels1[i].GetComponent<Image>().color = color;
                    // formTelemetryStruct.GetType().GetField("answer" + _currentQuestion.ToString() + "_" + (i + 1)).SetValue(formTelemetryStruct, selectedToggle.name);
                    switch (i+1)
                    {
                        case 1: formPart2.answer2_01 = selectedToggle.name.Last().ToString(); break;
                        case 2: formPart2.answer2_02 = selectedToggle.name.Last().ToString(); break;
                        case 3: formPart2.answer2_03 = selectedToggle.name.Last().ToString(); break;
                        case 4: formPart2.answer2_04 = selectedToggle.name.Last().ToString(); break;
                        case 5: formPart2.answer2_05 = selectedToggle.name.Last().ToString(); break;
                        case 6: formPart2.answer2_06 = selectedToggle.name.Last().ToString(); break;
                        case 7: formPart2.answer2_07 = selectedToggle.name.Last().ToString(); break;
                        case 8: formPart2.answer2_08 = selectedToggle.name.Last().ToString(); break;
                        case 9: formPart2.answer2_09 = selectedToggle.name.Last().ToString(); break;
                        case 10: formPart2.answer2_10 = selectedToggle.name.Last().ToString(); break;
                        case 11: formPart2.answer2_11 = selectedToggle.name.Last().ToString(); break;
                        case 12: formPart2.answer2_12 = selectedToggle.name.Last().ToString(); break;
                        case 13: formPart2.answer2_13 = selectedToggle.name.Last().ToString(); break;
                        case 14: formPart2.answer2_14 = selectedToggle.name.Last().ToString(); break;
                        case 15: formPart2.answer2_15 = selectedToggle.name.Last().ToString(); break;
                        case 16: formPart2.answer2_16 = selectedToggle.name.Last().ToString(); break;
                        case 17: formPart2.answer2_17 = selectedToggle.name.Last().ToString(); break;
                    }
                }
                else
                {
                    ErrorMsg2.text = "Please answer all questions";
                    Color color = Color.red;
                    color.a = 0.5f;
                    panels2[i].GetComponent<Image>().color = color;
                    return false;
                }
            }

            return true;
        }

        if (_currentQuestion == 3)
        {
            formPart3.answer3 = question3.GetComponent<Slider>().value.ToString();
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
        yield return StartCoroutine(FormTelemetry.DivideAndSubmit(GameIdController.gameId, formPart1, formPart2, formPart3));
        // yield return StartCoroutine(FormTelemetry.DivideAndSubmit("001", formPart1, formPart2, formPart3));
        
        SceneManager.LoadScene("MenuScene");
    }
}
