using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NBS_HUDManager : MonoBehaviour
{
    // Editor objects
    public TMPro.TMP_Text centreText;
    public TMPro.TMP_Text leftText;
    public TMPro.TMP_Text rightText;
    public TMPro.TMP_Text explainerText;
    public TMPro.TMP_Text locomotionText;
    public TMPro.TMP_Text sceneText;
    public Image image; 

    // ********************************
    // Class default methods
    // ********************************

    private void Awake()
    {
        // Not used
    }

    private void Start()
    {
        // Not used
    }

    private void Update()
    {
        // Not used
    }

    // ********************************
    // Accessor methods
    // ********************************

    public void ShowBackgroundImage()
    {
        image.CrossFadeAlpha(1, 0, true);
    }

    public void HideBackgroundImage()
    {
        image.CrossFadeAlpha(0, 0, true);
    }

    public void UpdateCentreText(string textValue)
    {
        centreText.text = textValue;
    }

    public void UpdateLeftText(string textValue)
    {
        leftText.text = textValue;
    }

    public void UpdateRightText(string textValue)
    {
        rightText.text = textValue;
    }

    public void UpdateExplainerText(string textValue)
    {
        explainerText.text = textValue;
    }

    public void UpdateLocomotionText(string textValue)
    {
        locomotionText.text = textValue;
    }

    public void UpdateSceneText(string textValue)
    {
        sceneText.text = textValue;
    }
}