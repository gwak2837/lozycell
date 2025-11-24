using UnityEngine;

public class AreaSkill : SkillStrategy
{
    public GameObject areaPrefab;
    public float duration = 5f;
    public float effectValue = 5f; // Damage or Slow factor
    public float radius = 1f;
    public bool spawnOnPlayer = true;
    public bool attachToPlayer = false;
    public Color visualColor = Color.white;

    public override void Activate(PlayerSkillController controller, Color skillColor = default)
    {
        if (areaPrefab == null)
            return;

        Vector3 spawnPos = spawnOnPlayer
            ? controller.transform.position
            : controller.transform.position + (Vector3)Random.insideUnitCircle * 3f;

        GameObject area = Object.Instantiate(areaPrefab, spawnPos, Quaternion.identity);

        if (attachToPlayer && spawnOnPlayer)
        {
            area.transform.SetParent(controller.transform);
            area.transform.localPosition = Vector3.zero;
        }

        var toxic = area.GetComponent<ToxicCloud>();
        if (toxic)
        {
            toxic.Initialize(effectValue, duration, radius);
            Color c = (skillColor.a > 0) ? skillColor : visualColor;
            toxic.SetColor(c);
        }
        else
        {
            Object.Destroy(area, duration);
        }
    }
}
