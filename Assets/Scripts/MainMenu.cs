using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayingMode { SINGLE, MULTI, NONE }

public class MainMenu : MonoBehaviour
{

    public static PlayingMode mode;
    private string sceneFromPlatform = "PlayScene";

    private void Start() 
    {
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
        SceneManager.LoadSceneAsync(sceneFromPlatform);
    }

    public void PlayMultiGame() 
    {
        mode = PlayingMode.MULTI;
        SceneManager.LoadSceneAsync(sceneFromPlatform);
    }

    public void QuitGame() 
    {
        //! TO CHANGE
        // Application.Quit();
        mode = PlayingMode.NONE;
        SceneManager.LoadSceneAsync(sceneFromPlatform);
    }
}
