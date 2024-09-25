using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    public GameObject tutorialPlayer;
    public GameObject tutorialBall;
    public ProgressBar tutorialProgBar;
    public GameObject tutorialGraphic;

    private Vector3 tutorialPlayerOriginalPos = new Vector3(-4f, 1, 0);
    private Vector3 tutorialBallOriginalPos = new Vector3(-2.5f, 3, 0);

    public GameObject endTutorialPanel;

    public GameObject backButton;

    // Start is called before the first frame update
    void Start()
    {
        GameIdController.isTutorial = true;
        StartCoroutine(StartTutorialCountdown());
    }

    // Update is called once per frame
    void Update()
    {
        tutorialProgBar.UpdateCurrent(0.1f);
    }

    public void resetObjs()
    {
        tutorialPlayer.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        tutorialPlayer.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        tutorialBall.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        tutorialBall.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        tutorialPlayer.transform.position = tutorialPlayerOriginalPos;
        tutorialBall.transform.position = tutorialBallOriginalPos;
    }

    public void EndTutorial()
    {
        GameIdController.tutorialCompleted = true;
        tutorialGraphic.SetActive(false);
        endTutorialPanel.SetActive(true);
    }

    public void LoadMainMenu()
    {
        GameIdController.isTutorial = false;
        SceneManager.LoadScene("MenuScene");
    }

    public void ContinueTutorial()
    {
        GameIdController.isTutorial = true;
        endTutorialPanel.SetActive(false);
        resetObjs();
    }

    IEnumerator StartTutorialCountdown()
    {
        var elapsedTime = 0f;

        while (elapsedTime < 30f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        EndTutorial();
        backButton.SetActive(true);
    }
}
