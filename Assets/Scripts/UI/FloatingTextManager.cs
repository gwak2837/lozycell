using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance { get; private set; }

    [SerializeField]
    private FloatingText textPrefab;

    [SerializeField]
    private int initialPoolSize = 20;

    private List<FloatingText> textPool = new List<FloatingText>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            FloatingText t = Instantiate(textPrefab, transform);
            t.gameObject.SetActive(false);
            textPool.Add(t);
        }
    }

    public void Show(float damage, Vector3 position, bool isCritical)
    {
        if (damage <= 0)
            return;

        FloatingText textToUse = null;

        // 1. Try to find inactive in existing pool
        foreach (var t in textPool)
        {
            if (!t.gameObject.activeSelf)
            {
                textToUse = t;
                break;
            }
        }

        // 2. If none found, create new
        if (textToUse == null)
        {
            textToUse = Instantiate(textPrefab, transform);
            textPool.Add(textToUse);
        }

        textToUse.transform.position = position;
        textToUse.gameObject.SetActive(true);
        textToUse.Setup(damage, isCritical);
    }
}
