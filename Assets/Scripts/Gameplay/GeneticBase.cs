using UnityEngine;
using TMPro;

public enum BaseType
{
    U,
    C,
    A,
    G
}

public class GeneticBase : MonoBehaviour
{
    public BaseType baseType;
    private SpriteRenderer spriteRenderer;

    // Optional: Text label to show the letter
    public TextMeshPro textLabel;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Try to find a text component in children if not assigned
        if (textLabel == null) textLabel = GetComponentInChildren<TextMeshPro>();
    }

    public void Initialize(BaseType type)
    {
        baseType = type;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        Color color = Color.white;
        string letter = "";

        switch (baseType)
        {
            case BaseType.U:
                color = new Color(1f, 0.2f, 0.2f); // Red
                letter = "U";
                break;
            case BaseType.C:
                color = new Color(0f, 0.5f, 1f); // Blue
                letter = "C";
                break;
            case BaseType.A:
                color = new Color(0.2f, 0.8f, 0.2f); // Green
                letter = "A";
                break;
            case BaseType.G:
                color = new Color(1f, 0.92f, 0.016f); // Yellow
                letter = "G";
                break;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }

        if (textLabel != null)
        {
            textLabel.text = letter;
        }
    }

    public void Collect()
    {
        ArcadeManager manager = FindObjectOfType<ArcadeManager>();
        if (manager != null)
        {
            manager.CollectBase(baseType);
        }

        Destroy(gameObject);
    }
}
