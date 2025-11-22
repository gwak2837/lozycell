using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Area Skill")]
public class AreaSkill : SkillStrategy
{
    public enum AreaType { InstantDamage, Slow, Push, SpawnObject }
    
    [Header("Area Settings")]
    public AreaType type;
    public float radius = 5f;
    public float value = 30f;
    public float duration = 0f; 
    public Color visualColor = Color.red;
    
    [Header("Spawn Settings")]
    public GameObject prefabToSpawn;

    public override void Activate(PlayerSkillController controller)
    {
        if (type == AreaType.SpawnObject)
        {
            // Logic for spawning toxic cloud etc.
            // If prefabToSpawn is null, we might need fallback or just return
            if (prefabToSpawn != null)
            {
                GameObject obj = Instantiate(prefabToSpawn, controller.transform.position, Quaternion.identity);
                var cloud = obj.GetComponent<ToxicCloud>();
                if (cloud != null) cloud.Initialize(value, duration, radius);
            }
            else
            {
                 // Fallback for procedural generation if needed (omitted for brevity as we moved to assets)
            }
            return; 
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(controller.transform.position, radius);
        SkillEffects.Instance.CreateVisualRing(controller.transform.position, radius, visualColor);

        foreach (var hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if (enemy != null)
            {
                ApplyEffect(enemy, controller.Player);
            }
        }
    }

    private void ApplyEffect(EnemyController enemy, PlayerController player)
    {
        switch (type)
        {
            case AreaType.InstantDamage:
                enemy.TakeDamage(value);
                break;
            case AreaType.Slow:
                enemy.ApplySlow(value, duration);
                break;
            case AreaType.Push:
                Vector3 pushDir = (enemy.transform.position - player.transform.position).normalized;
                enemy.transform.position += pushDir * value; 
                if (value > 2f) enemy.TakeDamage(20f); 
                break;
        }
    }
}
