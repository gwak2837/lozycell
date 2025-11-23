using UnityEngine;

public static class NucleobaseColorConfig
{
    public static readonly Color ColorU = new Color(1f, 0.2f, 0.2f);
    public static readonly Color ColorC = new Color(0f, 0.5f, 1f);
    public static readonly Color ColorA = new Color(0.2f, 0.8f, 0.2f);
    public static readonly Color ColorG = new Color(1f, 0.92f, 0.016f);

    public static Color GetColor(NucleobaseType type)
    {
        return type switch
        {
            NucleobaseType.U => ColorU,
            NucleobaseType.C => ColorC,
            NucleobaseType.A => ColorA,
            NucleobaseType.G => ColorG,
            _ => Color.white,
        };
    }
}
