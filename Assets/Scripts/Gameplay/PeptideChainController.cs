using System.Collections.Generic;
using UnityEngine;

public class PeptideChainController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject ribosomePrefab;
    [SerializeField] private GameObject aminoAcidPrefab;

    // Runtime State
    private GameObject ribosome;
    private List<Transform> chainNodes = new List<Transform>();
    private Transform target; // The player transform to follow

    // Config Cache
    private float followSpeed;
    private float nodeSpacing;
    private float headOffset;

    private void Awake()
    {
        // Initialize logic fields from GameConfig
        followSpeed = GameConfig.PeptideChain.FollowSpeed;
        nodeSpacing = GameConfig.PeptideChain.NodeSpacing;
        headOffset = GameConfig.PeptideChain.HeadOffset;
    }

    public void StartSynthesis(Transform followTarget)
    {
        target = followTarget;
        ClearChain();

        // Create Ribosome (Head)
        if (ribosomePrefab != null)
        {
            ribosome = Instantiate(ribosomePrefab, transform);
            // Initial position behind player
            ribosome.transform.position = target.position - target.up * headOffset; 
            chainNodes.Add(ribosome.transform);
        }
        else
        {
            Debug.LogError("PeptideChainController: Ribosome Prefab is missing!");
        }
    }

    public void AddAminoAcid(AminoAcidData data)
    {
        if (aminoAcidPrefab == null || chainNodes.Count == 0) return;

        Transform lastNode = chainNodes[chainNodes.Count - 1];
        GameObject newNode = Instantiate(aminoAcidPrefab, transform);
        
        // Set Color
        var renderer = newNode.GetComponentInChildren<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = data.Color;
        }

        // Initial Position (start at last node)
        newNode.transform.position = lastNode.position;
        chainNodes.Add(newNode.transform);
    }

    public void FinishSynthesis()
    {
        // TODO: Add fancy folding animation here
        // For now, just clear immediately
        ClearChain();
    }

    private void ClearChain()
    {
        foreach (var node in chainNodes)
        {
            if (node != null) Destroy(node.gameObject);
        }
        chainNodes.Clear();
        ribosome = null;
    }

    private void LateUpdate()
    {
        if (chainNodes.Count == 0 || target == null) return;

        // 1. Move Head (Ribosome) to follow Player
        Transform head = chainNodes[0];
        
        // Calculate desired position behind the player (or just trailing)
        // Simple logic: SmoothDamp towards target position
        // To make it feel like a "tail", we can just follow the player's position directly but with a delay/distance
        // However, standard snake logic is usually: Node N moves to where Node N-1 was.
        // Let's use a distance constraint approach for smooth trailing.

        Vector3 targetPos = target.position;
        
        // If we want it to always be behind, we need the player's velocity or facing direction.
        // For simplicity in this 2D top-down view, just moving towards the player is fine, 
        // gravity/physics isn't the main focus.
        
        // Move Head
        float distance = Vector3.Distance(head.position, targetPos);
        if (distance > headOffset)
        {
            Vector3 direction = (targetPos - head.position).normalized;
            head.position += direction * followSpeed * Time.deltaTime;
            // Optional: Look at target
            head.up = direction;
        }

        // 2. Move Body Segments
        for (int i = 1; i < chainNodes.Count; i++)
        {
            Transform current = chainNodes[i];
            Transform prev = chainNodes[i - 1];

            float dist = Vector3.Distance(current.position, prev.position);
            if (dist > nodeSpacing)
            {
                Vector3 dir = (prev.position - current.position).normalized;
                // Move towards previous node until distance is nodeSpacing
                // Lerp or MoveTowards
                current.position = Vector3.MoveTowards(current.position, prev.position - dir * nodeSpacing, followSpeed * Time.deltaTime);
            }
        }
    }
}

