using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayingMode { SINGLE, MULTI, NONE }

public class MainMenu : MonoBehaviour
{

    public static PlayingMode mode;

    public void PlaySingleGame() {
        mode = PlayingMode.SINGLE;
        SceneManager.LoadSceneAsync("PlayScene");
    }

    public void PlayMultiGame() {
        mode = PlayingMode.MULTI;
        SceneManager.LoadSceneAsync("PlayScene");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
