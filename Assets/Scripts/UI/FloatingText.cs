using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro textMesh;

    private float duration;
    private float moveSpeed;
    private Color startColor;
    private Vector3 moveDirection;

    private void Awake()
    {
        duration = GameConfig.UI.FloatingText.Duration;
        moveSpeed = GameConfig.UI.FloatingText.MoveSpeed;
    }

    public void Setup(float damage, bool isCritical)
    {
        if (textMesh == null)
            return;

        // Text Content
        textMesh.text = Mathf.RoundToInt(damage).ToString();

        // Appearance based on critical
        if (isCritical)
        {
            textMesh.fontSize = GameConfig.UI.FloatingText.CriticalFontSize;
            textMesh.color = new Color(1f, 0.8f, 0f, 1f); // Orange/Gold
            moveDirection = Vector3.up + (Vector3)Random.insideUnitCircle.normalized * 0.2f;
        }
        else
        {
            textMesh.fontSize = GameConfig.UI.FloatingText.NormalFontSize;
            textMesh.color = Color.white;
            moveDirection = Vector3.up;
        }

        startColor = textMesh.color;

        // Offset slightly to avoid overlapping exact same spot
        transform.position += (Vector3)Random.insideUnitCircle * GameConfig.UI.FloatingText.RandomOffset;

        StartCoroutine(AnimateAndHide());
    }

    private IEnumerator AnimateAndHide()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Move up
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // Fade out in the last half
            if (elapsed > duration * 0.5f)
            {
                float fadeProgress = (elapsed - (duration * 0.5f)) / (duration * 0.5f);
                float alpha = Mathf.Lerp(1f, 0f, fadeProgress);
                textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
