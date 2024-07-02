using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class gui : MonoBehaviour {
    public static gui S;

    public Text scoreText;
    public GameObject goalText;

    public int player1Goals;
    public int player2Goals;

	// Use this for initialization
	void Start () {
        S = this;
        player1Goals = 0;
        player2Goals = 0;
		scoreText = GameObject.Find("ScoreText").GetComponent<Text> ();
        goalText = GameObject.Find("GoalText");
        goalText.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		scoreText.text = player1Goals.ToString() + " - " + player2Goals.ToString();
    }

    public void ScoreGoalText(int i)
    {
        StartCoroutine(ScoreGoalTextEnum(i));
    }

    IEnumerator ScoreGoalTextEnum(int i)
    {
        goalText.SetActive(true);
        goalText.GetComponent<Text>().text = "Player " + i + " goal!";
        yield return new WaitForSeconds(1f);
        goalText.SetActive(false);
    }
}
