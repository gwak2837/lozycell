using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerStats stats;

    private InputAction moveAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();

        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
        }
        else
        {
            Debug.LogError("PlayerInput component is missing on Player!");
        }

        // Jitter 방지: 물리 업데이트와 렌더링 프레임 간의 불일치 해소
        if (rb != null)
        {
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

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
        if (moveAction != null)
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }
    }

    private void FixedUpdate()
    {
        float currentSpeed = stats ? stats.GetCurrentMoveSpeed() : GameConfig.Player.MoveSpeed;
        rb.linearVelocity = moveInput * currentSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Nucleobase") || other.GetComponent<Nucleobase>())
        {
            Nucleobase baseObj = other.GetComponent<Nucleobase>();
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
