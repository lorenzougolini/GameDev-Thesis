using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSpinner : MonoBehaviour
{
    [SerializeField] Image CircleImage;
    [SerializeField] Transform CircleTr;
    [SerializeField] public TextMeshProUGUI dotsText;

    [Range(0, 1)] float fillAmount = 0.25f;

    private float rotationSpeed = 200f;

    private float rotationTime = 0f;

    private float dotAppearTime = 0.5f;

    private void Start() 
    {
        StartCoroutine(UpdateDotText());  
    }

    // Update is called once per frame
    void Update()
    {
        CircleTr.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        rotationTime += rotationSpeed * Time.deltaTime;

        fillAmount = 0.25f + 0.25f * Mathf.Sin(rotationTime * Mathf.Deg2Rad); 

        CircleImage.fillAmount = fillAmount;

        // txtProgress.text = Mathf.Floor(fillAmount * 100).ToString() + "%";
    }

    IEnumerator UpdateDotText() 
    {
        while (true) 
        {
            dotsText.text = "";
            yield return new WaitForSeconds(dotAppearTime);
            
            dotsText.text = ".";
            yield return new WaitForSeconds(dotAppearTime);
            
            dotsText.text = "..";
            yield return new WaitForSeconds(dotAppearTime);
            
            dotsText.text = "...";
            yield return new WaitForSeconds(dotAppearTime);
        }
    }
}
