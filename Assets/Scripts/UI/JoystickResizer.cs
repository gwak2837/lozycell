using UnityEngine;
using UnityEngine.InputSystem.OnScreen;

public class JoystickResizer : MonoBehaviour
{
    [SerializeField]
    private RectTransform handleRect;

    private void Start()
    {
        ApplyPhysicalSize();
    }

    public void ApplyPhysicalSize()
    {
        Canvas canvas = GetComponentInParent<Canvas>();

        float dpi = Screen.dpi;
        if (dpi <= 0)
            dpi = 160f;

        float backgroundPixelSize = GameConfig.UI.Joystick.SizeCm / 2.54f * dpi;
        float handlePixelSize = GameConfig.UI.Joystick.HandleSizeCm / 2.54f * dpi;
        float scaleFactor = canvas.scaleFactor;
        float backgroundSizeDelta = backgroundPixelSize / scaleFactor;
        float handleSizeDelta = handlePixelSize / scaleFactor;

        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(backgroundSizeDelta, backgroundSizeDelta);
        handleRect.sizeDelta = new Vector2(handleSizeDelta, handleSizeDelta);

        OnScreenStick stick = handleRect.GetComponent<OnScreenStick>();
        stick.movementRange = (backgroundSizeDelta - handleSizeDelta) * 0.5f * 0.6f;
    }
}
