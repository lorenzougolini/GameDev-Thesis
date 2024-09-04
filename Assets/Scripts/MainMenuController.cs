using System.Collections;
using System.Collections.Generic;
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

    private TextMeshProUGUI mainText;
    private TextMeshProUGUI dotsText;

    [SerializeField] public static PlayingMode mode;
    private string sceneFromPlatform = "PlayScene";

    [Range(30, 60)] public float waitingTime;

    private void Start() 
    {
        mainText = loadingText.transform.Find("MainText").GetComponent<TextMeshProUGUI>();
        dotsText = loadingText.transform.Find("DotsText").GetComponent<TextMeshProUGUI>();
        // DetectPlatform();

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
        // StartCoroutine(WaitingRoomCoroutine(sceneFromPlatform));
        // SceneManager.LoadSceneAsync(sceneFromPlatform);
    }

    public void PlayMultiGame() 
    {
        mode = PlayingMode.MULTI;
        IdInputPanel.SetActive(true);
        // StartCoroutine(WaitingRoomCoroutine(sceneFromPlatform));

        // SceneManager.LoadSceneAsync(sceneFromPlatform);
    }

    public void QuitGame() 
    {
        //! TO CHANGE
        // Application.Quit();
        mode = PlayingMode.NONE;
        IdInputPanel.SetActive(true);
        // StartCoroutine(WaitingRoomCoroutine(sceneFromPlatform));

        // SceneManager.LoadSceneAsync(sceneFromPlatform);
    }

    public void IdSubmit()
    {
        GameIdController.SetGameId(IdInputField.text);
        GameIdController.SetRoundNumber();
        IdInputPanel.SetActive(false);
        StartCoroutine(WaitingRoomCoroutine(sceneFromPlatform));
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
        mainText.text = "Game is starting!!";

        // waitingRoom.SetActive(false);
        asyncLoad.allowSceneActivation = true;
        yield return null;
    }
}
