using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("References")]
    public Animator anim;
    public EnemyAI enemyAI;
    public Rigidbody2D rb;
    public Collider2D enemyCollider;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (anim == null)
            anim = GetComponent<Animator>();

        if (enemyAI == null)
            enemyAI = GetComponent<EnemyAI>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (enemyCollider == null)
            enemyCollider = GetComponent<Collider2D>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        anim.SetTrigger("EnemyHurt");

        Debug.Log("Enemy HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        anim.SetTrigger("EnemyDie");

        if (enemyAI != null)
            enemyAI.enabled = false;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (enemyCollider != null)
            enemyCollider.enabled = false;

        Destroy(gameObject, 2f);
    }
}