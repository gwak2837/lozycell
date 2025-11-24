using System;
using TMPro;
using UnityEngine;

public enum NucleobaseType
{
    U,
    C,
    A,
    G,
}

public class Nucleobase : MonoBehaviour
{
    public NucleobaseType baseType;
    private SpriteRenderer spriteRenderer;

    // Events
    public event Action<NucleobaseType> OnCollected;
    public event Action<Nucleobase> OnDespawn;

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

        if (textLabel == null)
            textLabel = GetComponentInChildren<TextMeshPro>();
    }

    private void Update()
    {
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
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null)
                playerTransform = player.transform;

            if (playerTransform == null)
                return;
        }

        float distSq = (transform.position - playerTransform.position).sqrMagnitude;
        if (distSq > despawnDistanceSq)
        {
            OnDespawn?.Invoke(this);
        }
    }

    public void Initialize(NucleobaseType type)
    {
        baseType = type;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        spriteRenderer.color = NucleobaseColorConfig.GetColor(baseType);
        textLabel.text = baseType.ToString();
        textLabel.color = NucleobaseColorConfig.GetTextColor(baseType);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>())
        {
            Collect();
        }
    }

    public void Collect()
    {
        OnCollected?.Invoke(baseType);
    }
}
