using UnityEngine;
using UnityEngine.UI;

public class ButtonClickDetector : MonoBehaviour
{
    public Button targetButton;
    public Image transparentImage;

    void Start()
    {
        // Get the transparent image component
        transparentImage = GetComponent<Image>();
        transparentImage.raycastTarget = true; // Make sure it's initially enabled
    }

    void Update()
    {
        // Enable or disable the raycast target based on the button's interactability
        if (targetButton.interactable)
        {
            transparentImage.raycastTarget = false; // Allow clicks to pass through
        }
        else
        {
            transparentImage.raycastTarget = true; // Block clicks with the transparent overlay
        }
    }

    public void OnClick()
    {
        if (!targetButton.interactable)
        {
            Debug.Log("Button clicked");
        }
    }
}
