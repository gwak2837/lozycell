using System.Collections.Generic;
using UnityEngine;

public class CodonRingController : MonoBehaviour
{
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
        float angleStep = 360f / GameConfig.CodonRing.SlotCount;

        for (int i = 0; i < GameConfig.CodonRing.SlotCount; i++)
        {
            var slotObj = Instantiate(slotPrefab, _center);

            float angle = i * angleStep * Mathf.Deg2Rad;
            slotObj.transform.localPosition =
                new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * GameConfig.CodonRing.Radius;

            _slots.Add(new SlotView(slotObj));
        }

        UpdateVisuals(new List<BaseType>());
    }

    private void RotateSlots()
    {
        float step = GameConfig.CodonRing.RotationSpeed * Time.deltaTime;
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

    private Color GetBaseColor(BaseType type) => BaseColorConfig.GetColor(type);
}
