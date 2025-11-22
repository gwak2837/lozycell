using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Area Skill")]
public class AreaSkill : SkillStrategy
{
    public GameObject areaPrefab;
    public float duration = 5f;
    public float effectValue = 5f; // Damage or Slow factor
    public float radius = 1f;
    public bool spawnOnPlayer = true;
    public bool attachToPlayer = false; // New field
    public Color visualColor = Color.white; // New field

    public override void Activate(PlayerSkillController controller, Color skillColor = default)
    {
        if (areaPrefab == null)
            return;

        Vector3 spawnPos = spawnOnPlayer
            ? controller.transform.position
            : controller.transform.position + (Vector3)Random.insideUnitCircle * 3f;

        GameObject area = Instantiate(areaPrefab, spawnPos, Quaternion.identity);

        if (attachToPlayer && spawnOnPlayer)
        {
            area.transform.SetParent(controller.transform);
        }

        // Try to initialize known components
        var toxic = area.GetComponent<ToxicCloud>();
        if (toxic != null)
        {
            toxic.Initialize(effectValue, duration, radius);

            // Use skillColor if valid (alpha > 0), else fallback to inspector setting
            Color c = (skillColor.a > 0) ? skillColor : visualColor;
            toxic.SetColor(c);
        }

        if (toxic == null)
        {
            Destroy(area, duration);
        }
    }
}
