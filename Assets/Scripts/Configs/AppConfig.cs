using UnityEngine;

public static class AppConfig
{
    public static class Camera
    {
        public const float SmoothTime = 0.125f;
    }

    public static class UI
    {
        public static class Ribosome
        {
            public const float PunchScaleNormal = 1.2f;
            public const float PunchScaleCombo = 1.5f; // 3번째 완성 시 더 커짐
            public const float ShakeIntensity = 10f; // 흔들림 강도
        }

        public static class Popup
        {
            public const float AnimateDuration = 0.3f;
            public const float DisplayDuration = 0.8f;
            public const float FadeOutDuration = 0.5f;
            public const float FloatDistance = 100f;
        }
    }
}
