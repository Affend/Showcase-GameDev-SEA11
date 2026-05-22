using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Rigidbody2D rb;
    public Animator anim;

    [Header("Movement")]
    public float moveSpeed = 2.5f;
    public float detectionRange = 6f;
    public float attackRange = 2.5f;

    [Header("Attack")]
    public float attackCooldown = 1f;
    private float attackTimer;

    private bool isFacingRight = true;

    [Header("Damage")]
    public int damage = 10;
    public float damageRange = 2.5f;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (anim == null) anim = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange && distance > attackRange)
        {
            ChasePlayer();
        }
        else if (distance <= attackRange)
        {
            StopMoving();
            AttackPlayer();
        }
        else
        {
            StopMoving();
        }

        FlipToPlayer();
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;

        rb.linearVelocity = new Vector2(direction.x * moveSpeed, direction.y * moveSpeed);

        anim.SetBool("EnemyWalk", true);
    }

    private void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("EnemyWalk", false);
    }

    private void AttackPlayer()
{
    rb.linearVelocity = Vector2.zero;
    anim.SetBool("EnemyWalk", false);

    attackTimer -= Time.deltaTime;

    if (attackTimer <= 0f)
    {
        Debug.Log("Enemy menyerang!");
        anim.ResetTrigger("EnemyAttack");
        anim.SetTrigger("EnemyAttack");

        if (Vector2.Distance(transform.position, player.position) <= damageRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        attackTimer = attackCooldown;
    }
}
    private void FlipToPlayer()
    {
        if (player.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, damageRange);
    }
}