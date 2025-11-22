using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Area Skill")]
public class AreaSkill : SkillStrategy
{
    public GameObject areaPrefab;
    public float duration = 5f;
    public float effectValue = 5f; // Damage or Slow factor
    public float radius = 1f;
    public bool spawnOnPlayer = true;

    public override void Activate(PlayerSkillController controller)
    {
        if (areaPrefab == null)
            return;

        Vector3 spawnPos = spawnOnPlayer
            ? controller.transform.position
            : controller.transform.position + (Vector3)Random.insideUnitCircle * 3f;

        GameObject area = Instantiate(areaPrefab, spawnPos, Quaternion.identity);

        // Try to initialize known components
        var toxic = area.GetComponent<ToxicCloud>();
        if (toxic != null)
        {
            toxic.Initialize(effectValue, duration, radius);
        }

        // If it's a gravity or slow zone, we might need other components.
        // For now, assume the prefab is self-contained or we extend this later.
        // Simple destruction if no specific init logic handles it
        if (toxic == null)
        {
            Destroy(area, duration);
        }
    }
}
