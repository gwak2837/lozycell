using TMPro;
using UnityEngine;

public enum BaseType
{
    U,
    C,
    A,
    G,
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
        if (textLabel == null)
            textLabel = GetComponentInChildren<TextMeshPro>();
    }

    private void Update()
    {
        if (manager == null)
            return;

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
            if (manager != null)
                playerTransform = manager.GetPlayerTransform();
            if (playerTransform == null)
                return;
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
        Color color = BaseColorConfig.GetColor(baseType);
        string letter = baseType.ToString();

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
