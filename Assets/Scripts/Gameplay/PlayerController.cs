using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerStats stats;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log($"Player initialized with Attack Power: {GameManager.Instance.TCellAttackPower}");
        }
    }

    private void Update()
    {
        // New Input System Only
        if (Keyboard.current != null)
        {
            float moveX = 0f;
            float moveY = 0f;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                moveY = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                moveY = -1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveX = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveX = 1f;

            moveInput = new Vector2(moveX, moveY).normalized;
        }
    }

    private void FixedUpdate()
    {
        float currentSpeed = stats != null ? stats.GetCurrentMoveSpeed() : moveSpeed;
        rb.linearVelocity = moveInput * currentSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check for GeneticBase (renamed from AminoAcid)
        // Ideally we check the component, or tag if tag was updated.
        // The tag might still be "AminoAcid" on the prefab, so we should keep checking that or check component directly.

        if (other.CompareTag("AminoAcid") || other.GetComponent<GeneticBase>() != null)
        {
            GeneticBase baseObj = other.GetComponent<GeneticBase>();
            if (baseObj != null)
            {
                baseObj.Collect();
            }
        }
    }
}
