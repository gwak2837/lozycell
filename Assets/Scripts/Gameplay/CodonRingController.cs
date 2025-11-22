using System.Collections.Generic;
using UnityEngine;

public class CodonRingController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float radius = 1.5f;

    [SerializeField]
    private float rotationSpeed = 50f;

    [SerializeField]
    private int slotCount = 3;

    [Header("References")]
    [SerializeField]
    private GameObject slotPrefab;

    private class SlotView
    {
        public readonly Transform transform;
        public readonly SpriteRenderer renderer;
        public readonly GameObject gameObject;

        public SlotView(GameObject obj)
        {
            gameObject = obj;
            transform = obj.transform;
            renderer = obj.GetComponent<SpriteRenderer>();
        }
    }

    private readonly List<SlotView> _slots = new();
    private Transform _center;

    private void Start()
    {
        _center = transform;
        InitializeSlots();
    }

    private void Update()
    {
        RotateSlots();
    }

    private void InitializeSlots()
    {
        float angleStep = 360f / slotCount;

        for (int i = 0; i < slotCount; i++)
        {
            var slotObj = Instantiate(slotPrefab, _center);

            float angle = i * angleStep * Mathf.Deg2Rad;
            slotObj.transform.localPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;

            _slots.Add(new SlotView(slotObj));
        }

        UpdateVisuals(new List<BaseType>());
    }

    private void RotateSlots()
    {
        float step = rotationSpeed * Time.deltaTime;
        foreach (var slot in _slots)
        {
            slot.transform.RotateAround(_center.position, Vector3.forward, step);
            slot.transform.rotation = Quaternion.identity; // Keep upright
        }
    }

    public void UpdateVisuals(List<BaseType> collectedBases)
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            var slot = _slots[i];
            bool isFilled = i < collectedBases.Count;

            if (isFilled)
            {
                BaseType type = collectedBases[i];
                slot.renderer.color = GetBaseColor(type);
            }
            else
            {
                slot.renderer.color = new Color(1f, 1f, 1f, 0.3f);
            }
        }
    }

    private Color GetBaseColor(BaseType type) =>
        type switch
        {
            BaseType.U => new Color(1f, 0.2f, 0.2f),
            BaseType.C => new Color(0f, 0.5f, 1f),
            BaseType.A => new Color(0.2f, 0.8f, 0.2f),
            BaseType.G => new Color(1f, 0.92f, 0.016f),
            _ => Color.white,
        };
}
