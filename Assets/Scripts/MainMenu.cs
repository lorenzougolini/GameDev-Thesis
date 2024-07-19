using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayingMode { SINGLE, MULTI, NONE }

public class MainMenu : MonoBehaviour
{

    public static PlayingMode mode;
    private string platform;

    private void Start() 
    {
        DetectPlatform();
    }

    private void DetectPlatform()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            platform = "PlaySceneMobile";
        else
            platform = "PlayScene";
    }

    public void PlaySingleGame() 
    {
        mode = PlayingMode.SINGLE;
        SceneManager.LoadSceneAsync(platform);
    }

    public void PlayMultiGame() 
    {
        mode = PlayingMode.MULTI;
        SceneManager.LoadSceneAsync(platform);
    }

    public void QuitGame() 
    {
        //! TO CHANGE
        // Application.Quit();
        mode = PlayingMode.NONE;
        SceneManager.LoadSceneAsync(platform);
    }
}
