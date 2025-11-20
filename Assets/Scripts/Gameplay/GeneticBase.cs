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
    private ArcadeManager manager;

    // Optional: Text label to show the letter
    public TextMeshPro textLabel;

    // Optimization: Disable if too far
    private Transform playerTransform;
    private float checkInterval = 1f;
    private float checkTimer;
    private float despawnDistanceSq = 400f; // 20 units squared

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Try to find a text component in children if not assigned
        if (textLabel == null) textLabel = GetComponentInChildren<TextMeshPro>();
    }

    private void Update()
    {
        if (manager == null) return;

        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0;
            CheckOutOfBounds();
        }
    }

    private void CheckOutOfBounds()
    {
        if (playerTransform == null)
        {
            if (manager != null) playerTransform = manager.GetPlayerTransform();
            if (playerTransform == null) return;
        }

        float distSq = (transform.position - playerTransform.position).sqrMagnitude;
        if (distSq > despawnDistanceSq)
        {
            manager.ReturnToPool(this);
        }
    }

    public void Initialize(BaseType type, ArcadeManager managerRef)
    {
        baseType = type;
        manager = managerRef;
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
        if (manager != null)
        {
            manager.CollectBase(baseType);
            manager.ReturnToPool(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
