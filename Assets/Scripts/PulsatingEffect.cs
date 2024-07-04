using System.Collections;
using UnityEngine;

public class PulsatingEffect : MonoBehaviour {
    public float pulseSpeed = 5f;
    public float maxScale = 2f;
    public float minScale = 1.0f;

    public GameObject text;

    private bool isPulsating = false;
    private Coroutine pulseCoroutine;

    public void StartPulsating() {
        if (!isPulsating) {
            isPulsating = true;
            pulseCoroutine = StartCoroutine(Pulse());
        }
    }

    public void StopPulsating() {
        if (isPulsating) {
            isPulsating = false;
            if (pulseCoroutine != null) {
                StopCoroutine(pulseCoroutine);
                pulseCoroutine = null;
            }
            text.transform.localScale = Vector3.one;
        }
    }

    private IEnumerator Pulse() {
        while (isPulsating) {
            float scale = Mathf.PingPong(Time.time * pulseSpeed, maxScale - minScale) + minScale;
            text.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }
}