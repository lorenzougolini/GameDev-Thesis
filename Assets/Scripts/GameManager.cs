using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject ballPrefab;
    public GameObject playerPrefab;
    public GameObject botPrefab;

    private Rigidbody2D ballRb;

    // Start is called before the first frame update
    void Start() {
        if (MainMenu.mode == PlayingMode.SINGLE)
        // if (mode == PlayingMode.SINGLE)
            StartCoroutine(SetUpSingleGame());
        else if (MainMenu.mode == PlayingMode.MULTI)
            StartCoroutine(SetUpMultiGame());
        else if (MainMenu.mode == PlayingMode.NONE)
            StartCoroutine(SetUpTestGame());

        Gui.S.player1Goals = 0;
        Gui.S.player2Goals = 0;
        Gui.S.gameStarted = false;
    }

    IEnumerator SetUpMultiGame() {
        GameObject ball = Instantiate(ballPrefab, new Vector3(0, 2, 0), Quaternion.identity);
        ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true;
        ResetObjects.S.AddObjectToReset(ball);

        GameObject player1 = Instantiate(playerPrefab, new Vector3(-7, 0, 0), Quaternion.identity);
        PlayerMovement p1Move = player1.GetComponent<PlayerMovement>();
        p1Move.playerNumber = 1;
        p1Move.speed = 8f;
        p1Move.jumpForce = 16f;
        ResetObjects.S.AddObjectToReset(player1);

        GameObject player2 = Instantiate(playerPrefab, new Vector3(7, 0, 0), Quaternion.identity);
        PlayerMovement p2Move = player2.GetComponent<PlayerMovement>();
        p2Move.playerNumber = 2;
        p2Move.speed = 8f;
        p2Move.jumpForce = 16f;
        Transform bodyP2 = player2.transform.Find("Body");
        if (bodyP2) 
            bodyP2.GetComponent<SpriteRenderer>().flipX = false;
        Transform footP2 = player2.transform.Find("Foot");
        if (footP2) 
            footP2.GetComponent<SpriteRenderer>().flipX = true;
        ResetObjects.S.AddObjectToReset(player2);

        StartCountdown();
        yield return null;
    }
    
    IEnumerator SetUpSingleGame() {

        GameObject ball = Instantiate(ballPrefab, new Vector3(0, 2, 0), Quaternion.identity);
        ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true;
        ResetObjects.S.AddObjectToReset(ball);

        GameObject player1 = Instantiate(playerPrefab, new Vector3(-7, 0, 0), Quaternion.identity);
        PlayerMovement p1Move = player1.GetComponent<PlayerMovement>();
        p1Move.playerNumber = 1;
        p1Move.speed = 8f;
        p1Move.jumpForce = 16f;
        ResetObjects.S.AddObjectToReset(player1);

        GameObject bot = Instantiate(botPrefab, new Vector3(7, 0, 0), Quaternion.identity);
        Bot botMove = bot.GetComponent<Bot>();
        botMove.speed = 8f;
        botMove.jumpForce = 16f;
        ResetObjects.S.AddObjectToReset(bot);

        StartCountdown();
        yield return null;
    }

    IEnumerator SetUpTestGame() {

        GameObject ball = Instantiate(ballPrefab, new Vector3(0, 2, 0), Quaternion.identity);
        ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.isKinematic = true;
        ResetObjects.S.AddObjectToReset(ball);

        StartCountdown();
        yield return null;
    }

    void StartCountdown() {
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown() {
        int countdown = 3;
        while (countdown > 0) {
            Gui.S.goalText.SetActive(true);
            Gui.S.goalText.GetComponent<Text>().text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }
        Gui.S.goalText.GetComponent<Text>().text = "GO!";
        Gui.S.gameStarted = true;
        ballRb.isKinematic = false;
        
        yield return new WaitForSeconds(1f);
        Gui.S.goalText.SetActive(false);
    }
}
