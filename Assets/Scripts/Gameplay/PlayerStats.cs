using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Movement")]
    public float baseMoveSpeed = 5f;
    private float speedMultiplier = 1f;

    [Header("Status")]
    public bool isShielded = false;
    public bool isInvulnerable = false;

    public event Action OnDeath;
    public event Action<float> OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public float GetCurrentMoveSpeed()
    {
        return baseMoveSpeed * speedMultiplier;
    }

    public void SetSpeedMultiplier(float multiplier, float duration = 0f)
    {
        speedMultiplier = multiplier;
        if (duration > 0)
        {
            Invoke(nameof(ResetSpeed), duration);
        }
    }

    private void ResetSpeed()
    {
        speedMultiplier = 1f;
    }

    public void EnableInvulnerability(float duration)
    {
        isInvulnerable = true;
        CancelInvoke(nameof(DisableInvulnerability));
        Invoke(nameof(DisableInvulnerability), duration);
    }

    private void DisableInvulnerability()
    {
        isInvulnerable = false;
    }

    public void EnableShield(float duration)
    {
        isShielded = true;
        CancelInvoke(nameof(DisableShield));
        Invoke(nameof(DisableShield), duration);
    }

    private void DisableShield()
    {
        isShielded = false;
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        if (isShielded)
        {
            // Shield absorbs damage (maybe one hit or all damage for duration? 
            // PRD says "ignores next damage" for Glycine usually, but "Speed + Shield (Gly - GGG)" 
            // says "ignores next damage" in plan. Let's stick to that or duration.)
            // The plan says "ignores next damage". The code above does duration. 
            // Let's make it duration based for "Shield" usually implies duration or hit count. 
            // Plan: "Speed + Shield ... ignores next damage". 
            // I will implement "consume shield on hit" logic.
            
            isShielded = false; // Consume shield
            return;
        }

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player Died");
        OnDeath?.Invoke();
        gameObject.SetActive(false);
    }
}

