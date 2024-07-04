using System.Collections;
using UnityEngine;

public class PulsatingEffect : MonoBehaviour {
    public float pulseSpeed = 1.5f;
    public float maxScale = 1.2f;
    public float minScale = 1.0f;

    public GameObject fill;

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
            fill.transform.localScale = Vector3.one;
        }
    }

    private IEnumerator Pulse() {
        while (isPulsating) {
            float scale = Mathf.PingPong(Time.time * pulseSpeed, maxScale - minScale) + minScale;
            fill.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }
}