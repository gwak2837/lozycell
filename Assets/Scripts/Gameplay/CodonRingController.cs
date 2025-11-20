using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CodonRingController : MonoBehaviour
{
    [Header("Settings")]
    public float radius = 1.5f;
    public float rotationSpeed = 50f;

    [Header("Prefabs")]
    public GameObject slotPrefab; // Prefab for the slot visual (e.g., a circle sprite)

    private List<GameObject> slots = new List<GameObject>();
    private Transform center;

    private void Start()
    {
        center = transform;
        CreateSlots();
    }

    private void Update()
    {
        RotateSlots();
    }

    private void CreateSlots()
    {
        // Create 3 slots
        for (int i = 0; i < 3; i++)
        {
            GameObject slot = null;
            if (slotPrefab != null)
            {
                slot = Instantiate(slotPrefab, center);
            }
            else
            {
                // Fallback if no prefab
                slot = new GameObject($"Slot_{i}");
                slot.transform.SetParent(center);
                var sr = slot.AddComponent<SpriteRenderer>();
                // Default sprite? We'll assume prefab has one or use a basic circle if we had assets.
                // For now, just a white square (default sprite is usually null, so need a sprite)
                // But we'll stick to setting Color.
            }
            
            slots.Add(slot);
        }
        
        UpdateSlotPositions();
        UpdateVisuals(new List<BaseType>()); // Clear visuals
    }

    private void UpdateSlotPositions()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            float angle = i * (360f / 3f);
            Vector3 pos = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * radius;
            slots[i].transform.localPosition = pos;
        }
    }

    private void RotateSlots()
    {
        // Rotate the whole container or individual positions?
        // Simpler to rotate the object itself or calculate positions.
        // Since this script is on the Player, we shouldn't rotate the Player.
        // Actually, this script should probably be on a child object "CodonRing" of the Player.
        // But if it is on the Player, we should rotate the slots around.
        
        // Let's assume this script is on a "RingPivot" child object, OR we handle rotation manually here.
        // If on Player, we just rotate the positions.
        
        float angleStep = Time.deltaTime * rotationSpeed;
        foreach (var slot in slots)
        {
            slot.transform.RotateAround(center.position, Vector3.forward, angleStep);
            // Keep slot upright?
            slot.transform.rotation = Quaternion.identity;
        }
    }

    public void UpdateVisuals(List<BaseType> currentCodon)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            SpriteRenderer sr = slots[i].GetComponent<SpriteRenderer>();
            TextMeshPro text = slots[i].GetComponentInChildren<TextMeshPro>();
            
            if (i < currentCodon.Count)
            {
                // Filled slot
                BaseType type = currentCodon[i];
                Color c = GetColorForBase(type);
                
                if (sr != null) sr.color = c;
                if (text != null) text.text = type.ToString();
                
                slots[i].SetActive(true);
            }
            else
            {
                // Empty slot
                if (sr != null) sr.color = new Color(1f, 1f, 1f, 0.3f); // Gray/Transparent
                if (text != null) text.text = "";
                
                // Keep active to show empty slots? Yes.
                slots[i].SetActive(true);
            }
        }
    }

    private Color GetColorForBase(BaseType type)
    {
        switch (type)
        {
            case BaseType.U: return new Color(1f, 0.2f, 0.2f); // Red
            case BaseType.C: return new Color(0f, 0.5f, 1f); // Blue
            case BaseType.A: return new Color(0.2f, 0.8f, 0.2f); // Green
            case BaseType.G: return new Color(1f, 0.92f, 0.016f); // Yellow
            default: return Color.white;
        }
    }
}

