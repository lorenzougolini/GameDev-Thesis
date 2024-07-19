using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControls : MonoBehaviour
{
    public static bool leftPressed;
    public static bool rightPressed;
    public static bool jumpPressed;
    public static bool kickPressed = false;
    public static bool powerPressed = false;

    public float lastLeftPressTime = 0f;
    public float lastRightPressTime = 0f;
    public float doubleClickThreshold = 0.25f;

    public void OnLeftButtonDown()
    {
        float currentTime = Time.time;
        if (currentTime - lastLeftPressTime <= doubleClickThreshold)
        {
            PlayerMovement.Instance.TriggerDash(-1);
        }
        lastLeftPressTime = currentTime;
        leftPressed = true;
        LogButtons();
    }
    public void OnLeftButtonUp()
    {
        leftPressed = false;
        LogButtons();
    }
    
    public void OnRightButtonDown()
    {
        float currentTime = Time.time;
        if (currentTime - lastRightPressTime <= doubleClickThreshold)
        {
            PlayerMovement.Instance.TriggerDash(1);
        }
        lastRightPressTime = currentTime;
        rightPressed = true;
        LogButtons();
    }
    public void OnRightButtonUp()
    {
        rightPressed = false;
        LogButtons();
    }
    
    public void OnJumpButtonDown()
    {
        jumpPressed = true;
        LogButtons();
    }
    public void OnJumpButtonUp()
    {
        jumpPressed = false;
        LogButtons();
    }
    
    public void OnKickButtonDown()
    {
        kickPressed = true;
        LogButtons();
    }
    public void OnKickButtonUp()
    {
        kickPressed = false;
        LogButtons();
    }
    
    public void OnPowerButton()
    {
        powerPressed = true;
        LogButtons();
    }

    private void LogButtons() {
        Debug.Log($"Variables: left pressed: {leftPressed}, right pressed: {rightPressed}, jump pressed: {jumpPressed}, kick pressed: {kickPressed}, power pressed: {powerPressed}");
    }
}
