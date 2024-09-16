using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using TMPro;

public class AiEnvManager : MonoBehaviour
{
    [SerializeField] public AgentController player1;
    [SerializeField] public AgentController player2;
    public AiGameManager gameManager;
    [SerializeField] private Transform ball;
    // [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    // [SerializeField] private float gameDuration = 60f;

    private Rigidbody2D ballRb;
    private AIBall aIBall;

    public int scorePlayer1;
    public int scorePlayer2;


    private void Start()
    {
        ballRb = ball.GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<AiGameManager>();
        if (gameManager != null)
        {
            gameManager.OnRoundEnd += EndRound;
            gameManager.OnGameEnd += EndGame;
        }
        
        aIBall = FindObjectOfType<AIBall>();

        player1.spawnPosition = new Vector3(-7f, 0f);
        player2.spawnPosition = new Vector3(3.4f, 0f);
        
        ResetGame();
    }

    private void Update()
    {
        scoreText.text = $"{player1.Score} - {player2.Score}";

        player1.progressBar.UpdateCurrent(2f);
        player2.progressBar.UpdateCurrent(2f);
        // player1.progressBar.UpdateCurrent(0.1f);
        // player2.progressBar.UpdateCurrent(0.1f);

        if (!checkBallInside())
            ResetBall();
    }

    private void ResetGame()
    {
        player1.ResetPlayer();
        player2.ResetPlayer();
        ball.localPosition = new Vector3(-1.5f, 2f);
        ballRb.velocity = Vector2.zero;
        ballRb.angularVelocity = 0f;

        player1.PreviousRoundScore = player1.Score;
        player2.PreviousRoundScore = player2.Score;
        player1.Score = 0;
        player2.Score = 0;

        player1.progressBar.SetCurrent(0);
        player2.progressBar.SetCurrent(0);
    }

    public void GoalScored(int player)
    {

        if (player == 1)
        {
            player1.Score++;
            player2.progressBar.UpdateCurrent(15f);
            Debug.Log($"Player 1 score is: {player1.Score}");
        } 
        else
        {
            player2.Score++;
            player1.progressBar.UpdateCurrent(15f);
            Debug.Log($"Player 2 score is: {player2.Score}");
        }

        player1.ResetPlayer();
        player2.ResetPlayer();

        ball.localPosition = new Vector3(-1.5f, 2f);
        ballRb.velocity = Vector2.zero;
        ballRb.angularVelocity = 0f;
        aIBall.StopPower();

    }

    private void EndRound()
    {
        float rewardDifference = Mathf.Abs(player1.Score - player2.Score);
        // Calculate rewards based on the game result
        if (player1.Score > player2.Score)
        {
            player1.AddReward(rewardDifference * 2f);
            player2.AddReward(-rewardDifference * 2f);
        }
        else if (player2.Score > player1.Score)
        {
            player1.AddReward(-rewardDifference * 2f);
            player2.AddReward(rewardDifference * 2f);
        }

        if ((player1.Score == player2.Score) || (player1.Score == 0 && player2.Score == 0))
        {
            player1.AddReward(-2f);
            player2.AddReward(-2f);
        }

        // compare with previous round score
        if (player1.Score > player1.PreviousRoundScore)
            player1.AddReward(2f);
        else if (player1.Score < player1.PreviousRoundScore)
            player1.AddReward(-2f);

        if (player2.Score > player2.PreviousRoundScore)
            player2.AddReward(2f);
        else if (player2.Score < player2.PreviousRoundScore)
            player2.AddReward(-2f);

        player1.AddReward(-(player1.jumpCount*0.6f));
        player2.AddReward(-(player2.jumpCount*0.6f));

        // player1.EndEpisode();
        // player2.EndEpisode();
        ResetGame();
    }

    private void EndGame()
    {
        player1.EndEpisode();
        player2.EndEpisode();
        ResetGame();
    }

    public void KickOpponent(string kickedTag) {
        if (kickedTag == "Player")
            
            player1.TakeDamage(-1);

        else if (kickedTag == "Enemy")

            player2.TakeDamage(1);
    }

    private bool checkBallInside()
    {
        return ball.localPosition.x >= -14 && ball.localPosition.x <= 11 && ball.localPosition.y >= -1 && ball.localPosition.y <= 10;
    }

    private void ResetBall()
    {
        ball.localPosition = new Vector3(-1.5f, 2f);
    }
}
