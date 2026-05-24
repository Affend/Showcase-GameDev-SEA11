using UnityEngine;

public class Arrow : MonoBehaviour
{
    [HideInInspector] public int damage;

    [Header("Enemy Layer")]
    public LayerMask enemyLayer;

    private bool hasHit = false;

    private void Start()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        // Ignore Player
        if (other.GetComponent<Movement>() != null)
            return;

        // Cek apakah object ada di layer Enemy
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            hasHit = true;

            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log("Arrow hit enemy: " + other.gameObject.name + " | Damage: " + damage);
            }
            else
            {
                Debug.LogWarning("Object di layer Enemy tapi tidak punya EnemyHealth!");
            }

            Destroy(gameObject);
            return;
        }

        // Kalau kena selain enemy
        hasHit = true;
        Debug.Log("Arrow hit: " + other.gameObject.name);
        Destroy(gameObject);
    }
}