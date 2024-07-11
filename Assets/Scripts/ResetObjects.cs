using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ResetObjects : MonoBehaviour {
    public static ResetObjects S;

    public GameManager GM;

    public bool resetting = false;

    // Use this for initialization
    void Awake () {
        S = this;

	    // foreach(GameObject obj in GM.gameObjects)
        // {
        //     GM.originalPositions.Add(obj.transform.position);
        // }
	}

    public void Reset ()
    {
        // GM = GameManager.Instance;
        StartCoroutine(ResetEnum());
    }

    IEnumerator ResetEnum() {

        resetting = true;

        GameObject ball = GameObject.FindGameObjectWithTag("Ball");
        GameObject[] goals = GameObject.FindGameObjectsWithTag("GoalRight").Concat(GameObject.FindGameObjectsWithTag("GoalLeft")).ToArray();

        if (ball) {
            ball.GetComponent<TrailRenderer>().enabled = false;
            Collider2D ballCollider = ball.GetComponent<Collider2D>();
            foreach (GameObject goal in goals)
            {
                Collider2D goalCollider = goal.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(ballCollider, goalCollider, true);
            }

        }

        // for(int i = 0; i < ObjectsToReset.Count; i++)
        // {
        //     ObjectsToReset[i].GetComponent<TrailRenderer>().enabled = false;
        // }
        yield return new WaitForSeconds(1f);
        foreach (GameObject obj in GameManager.gameObjects)
        {
            obj.transform.position = GameManager.originalPositions[GameManager.gameObjects.IndexOf(obj)];
            obj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            obj.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        }
        yield return new WaitForSeconds(.25f);
        // for (int i = 0; i < ObjectsToReset.Count; i++)
        // {
        //     ObjectsToReset[i].GetComponent<TrailRenderer>().enabled = true;
        // }

        if (ball) {
            ball.GetComponent<TrailRenderer>().enabled = true;
            Collider2D ballCollider = ball.GetComponent<Collider2D>();
            foreach (GameObject goal in goals)
            {
                Collider2D goalCollider = goal.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(ballCollider, goalCollider, false);
            }
        }

        resetting = false;
    }

}
