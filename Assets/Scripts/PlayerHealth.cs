using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Animator anim;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (anim == null)
            anim = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        anim.SetTrigger("Hurt");

        Debug.Log("Player terkena damage! HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        anim.SetTrigger("Die");

        Debug.Log("Player mati!");

        // Optional: matikan gerakan player
        Movement movement = GetComponent<Movement>();
        if (movement != null)
            movement.enabled = false;
    }
}
