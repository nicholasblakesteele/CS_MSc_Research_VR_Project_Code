using UnityEngine;
using UnityEngine.UI;

public class NBS_BackgroundScale : MonoBehaviour
{
    // NOTE: This code came from: https://sharpcoderblog.com/blog/unity-3d-create-main-menu-with-ui-canvas [Accessed on 14 December 2022]
    // Ensure this code is on the same object that also has the image (as a component) you want to display

    Image backgroundImage;
    RectTransform rt;
    float ratio;

    void Start()
    {
        backgroundImage = GetComponent<Image>();
        rt = backgroundImage.rectTransform;
        ratio = backgroundImage.sprite.bounds.size.x / backgroundImage.sprite.bounds.size.y;
    }

    void Update()
    {
        if (!rt)
            return;

        // Scale image proportionally to fit the screen dimensions, while preserving aspect ratio

        if (Screen.height * ratio >= Screen.width)
        {
            rt.sizeDelta = new Vector2(Screen.height * ratio, Screen.height);
        }
        else
        {
            rt.sizeDelta = new Vector2(Screen.width, Screen.width / ratio);
        }
    }
}