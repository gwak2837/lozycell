using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerStats stats;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
        UpdateColliderSize();
    }

    private void Start()
    {
        // Ensure size is applied on start
        UpdateColliderSize();

        if (GameManager.Instance != null)
        {
            Debug.Log($"Player initialized with Attack Power: {GameManager.Instance.TCellAttackPower}");
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // In Editor mode, we can't rely on UpdateColliderSize here because hitBoxSize is NonSerialized
        // and might not be set correctly during editing.
        // But since we want "Code Only" control, we rely on Awake/Start at runtime.
        // If we want to visualize in Editor Edit Mode, we would need [ExecuteAlways] or similar,
        // but keeping it simple for now: changes apply on Play.
    }
#endif

    private void UpdateColliderSize()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            // Apply square size based on hitBoxSize
            if (box.size.x != GameConfig.Player.HitBoxSize || box.size.y != GameConfig.Player.HitBoxSize)
            {
                box.size = new Vector2(GameConfig.Player.HitBoxSize, GameConfig.Player.HitBoxSize);
            }
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
        float currentSpeed = stats ? stats.GetCurrentMoveSpeed() : GameConfig.Player.MoveSpeed;
        rb.linearVelocity = moveInput * currentSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("AminoAcid") || other.GetComponent<GeneticBase>())
        {
            GeneticBase baseObj = other.GetComponent<GeneticBase>();
            if (baseObj)
            {
                baseObj.Collect();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = Color.blue;
            if (col is CircleCollider2D circle)
            {
                Gizmos.DrawWireSphere(
                    transform.position,
                    circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y)
                );
            }
            else if (col is BoxCollider2D box)
            {
                Gizmos.DrawWireCube(
                    transform.position,
                    new Vector3(box.size.x * transform.lossyScale.x, box.size.y * transform.lossyScale.y, 1)
                );
            }
        }
    }
}
