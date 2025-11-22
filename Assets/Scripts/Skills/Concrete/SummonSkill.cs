using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Summon Skill")]
public class SummonSkill : SkillStrategy
{
    [Header("Summon Settings")]
    public GameObject prefabToSummon;
    public float duration = 10f;

    public override void Activate(PlayerSkillController controller)
    {
        GameObject pet = null;

        if (prefabToSummon != null)
        {
            pet = Instantiate(prefabToSummon, controller.transform.position + Vector3.right, Quaternion.identity);
        }
        else
        {
            // Fallback procedural creation
            pet = new GameObject("Pet_Ribosome");
            pet.transform.position = controller.transform.position + Vector3.right;

            var sr = pet.AddComponent<SpriteRenderer>();
            sr.color = Color.green;
            sr.sortingOrder = 10;

            Texture2D tex = new Texture2D(32, 32);
            Color[] colors = new Color[32 * 32];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Color.white;
            tex.SetPixels(colors);
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);

            pet.transform.localScale = Vector3.one * 0.5f;
        }

        // Use ProjectileSystem for the pet? No, Pet has its own controller.
        var petCtrl = pet.GetComponent<PetController>();
        if (petCtrl == null)
            petCtrl = pet.AddComponent<PetController>();

        // We need to pass ProjectileSystem somehow. PetController was modified to take SkillManager.
        // We should update PetController to use Singleton ProjectileSystem too!
        // For now, passing null as manager and updating PetController logic separately.
        // Or better, let's update PetController to not need manager.

        // Passing null for now, we will fix PetController next.
        petCtrl.Initialize(controller.Player, duration);

        controller.SetActivePet(pet);
    }
}
