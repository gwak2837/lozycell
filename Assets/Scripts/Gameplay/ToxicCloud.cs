using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class ToxicCloud : MonoBehaviour
{
    private float damagePerSecond;
    private float duration;
    private float radius;

    public void Initialize(float damage, float durationTime, float areaRadius)
    {
        damagePerSecond = damage;
        duration = durationTime;
        radius = areaRadius;

        transform.localScale = Vector3.one * radius;
        Destroy(gameObject, duration);
    }

    public void SetColor(Color color)
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(color.r, color.g, color.b, 0.5f);
    }

    private void Start()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy)
            enemy.TakeDamage(damagePerSecond * Time.deltaTime);
    }
}
