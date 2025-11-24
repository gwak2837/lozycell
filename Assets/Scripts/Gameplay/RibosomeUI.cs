using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RibosomeUI : MonoBehaviour
{
    [Header("Slot UI Components")]
    [SerializeField]
    private Image[] slotImages;

    [SerializeField]
    private TextMeshProUGUI[] slotTexts;

    [Header("Colors")]
    [SerializeField]
    private Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

    [Header("Dopamine Settings")]
    private float punchScaleNormal;

    private void Start()
    {
        punchScaleNormal = AppConfig.UI.Ribosome.PunchScaleNormal;

        if (ArcadeManager.Instance != null)
        {
            ArcadeManager.Instance.OnCodonUpdated += UpdateVisuals;
            // Force update immediately to sync with current state
            UpdateVisuals(ArcadeManager.Instance.GetCurrentCodon());
        }
        else
        {
            UpdateVisuals(new List<NucleobaseType>());
        }
    }

    private void OnDestroy()
    {
        if (ArcadeManager.Instance != null)
        {
            ArcadeManager.Instance.OnCodonUpdated -= UpdateVisuals;
        }
    }

    // Track running jackpot to stop it if state changes
    private Coroutine jackpotRoutine;

    private void UpdateVisuals(List<NucleobaseType> currentCodon)
    {
        // Stop any existing jackpot effect to prevent color overrides
        if (jackpotRoutine != null)
        {
            StopCoroutine(jackpotRoutine);
            jackpotRoutine = null;
            // Force reset colors to ensure clean slate
            for (int i = 0; i < slotImages.Length; i++)
            {
                // If cleared, set to empty, otherwise restore base color
                if (i >= currentCodon.Count)
                    slotImages[i].color = emptyColor;

                // Reset border
                var outline = slotImages[i].GetComponent<Outline>();
                outline.effectColor = new Color(1f, 1f, 1f, 0f);
                outline.enabled = false;
            }
        }

        if (currentCodon == null)
            currentCodon = new List<NucleobaseType>();

        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i < currentCodon.Count)
            {
                NucleobaseType type = currentCodon[i];
                Color c = GetColorForBase(type);

                slotImages[i].color = c;
                slotTexts[i].text = type.ToString();

                // Fix contrast: If Yellow(G), use Black text. Else White.
                // Assuming G is the bright yellow one.
                if (type == NucleobaseType.G)
                    slotTexts[i].color = Color.black;
                else
                    slotTexts[i].color = Color.white;

                if (i == currentCodon.Count - 1)
                {
                    // 3rd slot gets a jackpot effect
                    if (i == 2)
                    {
                        // Calculate amino acid to get skill color
                        string aminoAcid = CodonTable.GetAminoAcid(currentCodon[0], currentCodon[1], currentCodon[2]);

                        // Fetch color from AminoAcidDefinitions (Code-based)
                        Color skillColor = AminoAcidDefinitions.GetData(aminoAcid).Color;

                        jackpotRoutine = StartCoroutine(JackpotEffect(skillColor));
                    }
                    else
                    {
                        StartCoroutine(PunchAnimation(slotImages[i].transform, punchScaleNormal));
                    }
                }
            }
            else
            {
                slotImages[i].color = emptyColor;
                slotTexts[i].text = "";
            }
        }
    }

    private IEnumerator PunchAnimation(Transform target, float maxScale)
    {
        float duration = 0.15f;
        float time = 0;
        Vector3 originalScale = Vector3.one;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * (maxScale - 1f);
            target.localScale = originalScale * scale;
            yield return null;
        }
        target.localScale = originalScale;
    }

    private IEnumerator JackpotEffect(Color skillColor)
    {
        int flashes = 5;
        float interval = 0.1f;

        for (int i = 0; i < flashes; i++)
        {
            // Borders ON (Skill Color)
            foreach (var img in slotImages)
            {
                var outline = img.GetComponent<Outline>();
                outline.enabled = true;
                outline.effectColor = new Color(skillColor.r, skillColor.g, skillColor.b, 1f);
            }
            yield return new WaitForSeconds(interval);

            // Borders OFF (Transparent)
            foreach (var img in slotImages)
            {
                var outline = img.GetComponent<Outline>();
                outline.effectColor = new Color(1f, 1f, 1f, 0f);
                outline.enabled = false;
            }
            yield return new WaitForSeconds(interval);
        }
    }

    private Color GetColorForBase(NucleobaseType type) => NucleobaseColorConfig.GetColor(type);
}
