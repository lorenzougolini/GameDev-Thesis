using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PlayingMode { SINGLE, MULTI, NONE, AI, TEST }

public class MainMenu : MonoBehaviour
{
    public GameObject IdInputPanel;
    public TMP_InputField IdInputField;

    public GameObject waitingRoom;

    public GameObject loadingText;
    public GameObject startingText;
    
    public GameObject loadingSpinner;

    public GameObject ErrorMsg;
    public GameObject warningText;

    public Button startButton;

    private TextMeshProUGUI mainText;
    private TextMeshProUGUI dotsText;

    [SerializeField] public static PlayingMode mode;
    private string sceneFromPlatform = "PlayScene";

    [Range(30, 60)] public float waitingTime;
    public Camera m_Camera;

    void Awake()
    {
        m_Camera = Camera.main;
    }

    private void Start() 
    {
        mainText = loadingText.transform.Find("MainText").GetComponent<TextMeshProUGUI>();
        dotsText = loadingText.transform.Find("DotsText").GetComponent<TextMeshProUGUI>();
        // DetectPlatform();
        if (GameIdController.tutorialCompleted)
        {
            startButton.interactable = true;
        }
        else
        {
            startButton.interactable = false;
            StartCoroutine(ViewMsg(warningText));
        }
    }

    private void DetectPlatform()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            sceneFromPlatform = "PlaySceneMobile";
        else
            sceneFromPlatform = "PlayScene";
    }

    public void PlaySingleGame() 
    {
        mode = PlayingMode.SINGLE;
        IdInputPanel.SetActive(true);
        IdInputField.Select();
    }

    public void PlayTutorial() 
    {
        SceneManager.LoadSceneAsync("TutorialScene");
    }

    public void QuitGame() 
    {
        Application.Quit();
    }

    public void IdSubmit()
    {
        if (checkValidID())
        {
            GameIdController.SetGameId(IdInputField.text);
            GameIdController.IncrementRoundNumber();
            IdInputPanel.SetActive(false);
            StartCoroutine(WaitingRoomCoroutine(sceneFromPlatform));
        }
        else
        {
            IdInputField.text = "";
            StartCoroutine(ViewMsg(ErrorMsg));
        }
    }

    private bool checkValidID()
    {
        return IdInputField.text.Length == 5 && IdInputField.text.Substring(3, 1) == "0";
    }

    public IEnumerator WaitingRoomCoroutine(string scene)
    {
        waitingRoom.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            mainText.text = "Loading Game";
            yield return null;
        }

        //set waiting time to be random within the range
        waitingTime = Random.Range(20, 40);
        Debug.Log("Random waiting time: " + waitingTime);
        float elapsedTime = 0f;

        while (elapsedTime < waitingTime)
        {
            mainText.text = "Connecting to the game";
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dotsText.text = "";
        loadingText.SetActive(false);
        loadingSpinner.SetActive(false);
        startingText.SetActive(true);

        // waitingRoom.SetActive(false);
        asyncLoad.allowSceneActivation = true;
        yield return null;
    }

    public IEnumerator ViewMsg(GameObject msg)
    {
        msg.SetActive(true);
        yield return new WaitForSeconds(5);
        msg.SetActive(false);
    }
}
