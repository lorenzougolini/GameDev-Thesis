using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "GoalRight") {
			Gui.S.player1Goals++;
            Gui.S.ScoreGoalText(1);
            ResetObjects.S.Reset();
			Gui.S.progressBar2.UpdateCurrent(15f);
		}
        if (coll.gameObject.tag == "GoalLeft")
        {
            Gui.S.player2Goals++;
            Gui.S.ScoreGoalText(2);
            ResetObjects.S.Reset();
			Gui.S.progressBar1.UpdateCurrent(15f);
        }
    }
}
