using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Summon Skill")]
public class SummonSkill : SkillStrategy
{
    public GameObject petPrefab;
    public float duration = 10f;

    public override void Activate(PlayerSkillController controller)
    {
        if (petPrefab == null)
            return;

        GameObject pet = Instantiate(petPrefab, controller.transform.position, Quaternion.identity);
        PetController pc = pet.GetComponent<PetController>();

        if (pc != null)
        {
            pc.Initialize(controller.transform, duration);
        }
        else
        {
            Destroy(pet, duration);
        }

        controller.SetActivePet(pet);
    }
}
