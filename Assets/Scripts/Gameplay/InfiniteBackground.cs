using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private Vector2 parallaxMultiplier = new Vector2(0.1f, 0.1f);
    
    private MeshRenderer meshRenderer;
    private Material material;
    private Vector2 startOffset;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            material = meshRenderer.material;
        }
    }

    private void Start()
    {
        if (targetToFollow == null && Camera.main != null)
        {
            targetToFollow = Camera.main.transform;
        }

        if (material != null)
        {
            startOffset = material.mainTextureOffset;
        }
    }

    private void Update()
    {
        if (targetToFollow == null || material == null) return;

        // Move the texture offset based on the target's position
        // The divisor determines how fast the texture scrolls relative to movement units
        // A value of 0.1 means 1 unit of movement = 0.1 units of texture offset
        Vector2 offset = new Vector2(
            targetToFollow.position.x * parallaxMultiplier.x, 
            targetToFollow.position.y * parallaxMultiplier.y
        );

        material.mainTextureOffset = startOffset + offset;

        // Also move the background object to follow the camera so it never goes off screen
        // We truncate the position to avoid jitter if needed, or just lock it exactly
        transform.position = new Vector3(targetToFollow.position.x, targetToFollow.position.y, transform.position.z);
    }
}

