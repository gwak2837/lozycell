using UnityEngine;

public class SummonSkill : SkillStrategy
{
    public GameObject petPrefab;
    public float duration = 10f;

    public override void Activate(PlayerSkillController controller, Color skillColor = default)
    {
        if (petPrefab == null)
            return;

        GameObject pet = Object.Instantiate(petPrefab, controller.transform.position, Quaternion.identity);
        PetController pc = pet.GetComponent<PetController>();

        if (pc != null)
        {
            pc.Initialize(controller.transform, duration);
            if (skillColor.a > 0)
            {
                pc.SetColor(skillColor);
            }
        }
        else
        {
            Object.Destroy(pet, duration);
        }

        controller.SetActivePet(pet);
    }
}
