using UnityEngine;
using UnityEngine.InputSystem.OnScreen;

[RequireComponent(typeof(OnScreenStick))]
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

        // Calculate target pixels: (CM / 2.54) * DPI
        float backgroundPixelSize = GameConfig.UI.Joystick.SizeCm / 2.54f * dpi;
        float handlePixelSize = GameConfig.UI.Joystick.HandleSizeCm / 2.54f * dpi;
        float scaleFactor = canvas.scaleFactor;
        float backgroundSizeDelta = backgroundPixelSize / scaleFactor;
        float handleSizeDelta = handlePixelSize / scaleFactor;

        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(backgroundSizeDelta, backgroundSizeDelta);
        handleRect.sizeDelta = new Vector2(handleSizeDelta, handleSizeDelta);

        OnScreenStick stick = GetComponent<OnScreenStick>();
        stick.movementRange = (backgroundSizeDelta - handleSizeDelta) * 0.5f;
    }
}
