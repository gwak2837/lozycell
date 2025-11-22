using System.Collections;
using UnityEngine;

public class PetController : MonoBehaviour
{
    private PlayerController player;
    private float duration;
    private float shootInterval = 1f;
    private float damage = 10f;

    // Manager removed, using Singleton
    public void Initialize(PlayerController owner, float dur)
    {
        player = owner;
        duration = dur;
        StartCoroutine(BehaviorRoutine());
    }

    private IEnumerator BehaviorRoutine()
    {
        float elapsed = 0;
        float lastShot = 0;

        while (elapsed < duration && player != null)
        {
            elapsed += Time.deltaTime;

            // Follow player
            Vector3 targetPos = player.transform.position + new Vector3(1.5f, 0, 0);
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);

            // Shoot
            if (elapsed - lastShot > shootInterval)
            {
                lastShot = elapsed;
                Vector3 enemyDir = SkillUtility.GetClosestEnemyDir(transform.position, Vector3.right);
                ProjectileSystem.Instance.Spawn(transform.position, enemyDir, damage, 12f, 2f, Color.green, 0.3f);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
