using System.Collections;
using UnityEngine;

public class PulsatingEffect : MonoBehaviour {
    // public float pulseSpeed = 5f;
    // public float maxScale = 2f;
    // public float minScale = 1.0f;

    public GameObject target;

    private bool isPulsating = false;
    private Coroutine pulseCoroutine;

    public void StartPulsating(GameObject targetObj, float minScale, float maxScale, float pulseSpeed) {
        target = targetObj;
        if (!isPulsating) {
            isPulsating = true;
            pulseCoroutine = StartCoroutine(Pulse(minScale, maxScale, pulseSpeed));
        }
    }

    public void StopPulsating() {
        if (isPulsating) {
            isPulsating = false;
            if (pulseCoroutine != null) {
                StopCoroutine(pulseCoroutine);
                pulseCoroutine = null;
            }
            if (target) {
                target.transform.localScale = Vector3.one;
                target = null;
            }
        }
    }

    private IEnumerator Pulse(float minScale, float maxScale, float pulseSpeed) {
        while (isPulsating && target) {
            float scale = Mathf.PingPong(Time.time * pulseSpeed, maxScale - minScale) + minScale;
            target.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }
}