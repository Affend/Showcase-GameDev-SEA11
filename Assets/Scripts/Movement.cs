using UnityEngine;

public class Movement : MonoBehaviour
{
    // Static reference to hold our single player instance globally
    public static Movement Instance { get; private set; }

    public Rigidbody2D rb;
    private Vector2 moveInput;
    public float speed = 5f;
    private bool isFacingRight = true;
    private Animator anim;

    [Header("Hold / Heavy Attack")]
    public float heavyAttackThreshold = 1.0f;
    private bool isChargingAttack = false;
    private float attackHoldTime = 0f;

    [Header("Bow Attack")]
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public float arrowSpeed = 10f;
    public float maxBowHoldTime = 1.5f;
    public int arrowDamage = 15;

    private bool isBowCharging = false;
    private float bowHoldTime = 0f;

    [Header("Attack Damage")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public int attackDamage = 20;
    public int heavyAttackDamage = 40;
    public LayerMask enemyLayer;

    private void Awake()
    {
        // PERSISTENCE LOGIC START
        // If an instance already exists and it's not this one, destroy the new duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Establish this object as the persistent instance and protect it across scenes
        Instance = this;
        DontDestroyOnLoad(gameObject);
        // PERSISTENCE LOGIC END

        anim = GetComponent<Animator>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.magnitude > 1)
            moveInput.Normalize();

        if (!isFacingRight && moveInput.x > 0f)
            Flip();
        else if (isFacingRight && moveInput.x < 0f)
            Flip();

        anim.SetBool("Walk", moveInput.magnitude > 0);

        // HOLD / HEAVY ATTACK - Klik kiri
        if (Input.GetMouseButtonDown(0) && !isBowCharging)
        {
            isChargingAttack = true;
            attackHoldTime = 0f;

            anim.ResetTrigger("Attack");
            anim.ResetTrigger("HeavyAttack");
            anim.SetBool("IsCharging", true);
        }

        if (Input.GetMouseButton(0) && isChargingAttack)
        {
            attackHoldTime += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0) && isChargingAttack)
        {
            isChargingAttack = false;
            anim.SetBool("IsCharging", false);

            if (attackHoldTime >= heavyAttackThreshold)
                HeavyAttack();
            else
                NormalAttack();

            attackHoldTime = 0f;
        }

        // BOW ATTACK - Klik kanan
        if (Input.GetMouseButtonDown(1) && !isChargingAttack)
        {
            isBowCharging = true;
            bowHoldTime = 0f;

            anim.ResetTrigger("ShootArrow");
            anim.SetBool("IsBowCharging", true);
        }

        if (Input.GetMouseButton(1) && isBowCharging)
        {
            bowHoldTime += Time.deltaTime;
            bowHoldTime = Mathf.Clamp(bowHoldTime, 0f, maxBowHoldTime);
        }

        if (Input.GetMouseButtonUp(1) && isBowCharging)
        {
            isBowCharging = false;
            anim.SetBool("IsBowCharging", false);

            anim.ResetTrigger("ShootArrow");
            anim.SetTrigger("ShootArrow");

            ShootArrow();

            bowHoldTime = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (isChargingAttack || isBowCharging)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = new Vector2(moveInput.x * speed, moveInput.y * speed);
    }

    private void NormalAttack()
    {
        anim.ResetTrigger("HeavyAttack");
        anim.SetTrigger("Attack");

        DealDamage(attackDamage);
    }

    private void HeavyAttack()
    {
        anim.ResetTrigger("Attack");
        anim.SetTrigger("HeavyAttack");

        DealDamage(heavyAttackDamage);
    }

    public void DealDamage(int damage)
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("AttackPoint belum dipasang di Inspector!");
            return;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
                enemyHealth.TakeDamage(damage);
        }
    }

    private void ShootArrow()
    {
        if (arrowPrefab == null || arrowSpawnPoint == null)
        {
            Debug.LogWarning("Arrow Prefab atau Arrow Spawn Point belum dipasang!");
            return;
        }

        GameObject arrow = Instantiate(
            arrowPrefab,
            arrowSpawnPoint.position,
            Quaternion.identity
        );

        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        Arrow arrowScript = arrow.GetComponent<Arrow>();

        if (arrowRb == null)
        {
            Debug.LogWarning("Arrow belum punya Rigidbody2D!");
            Destroy(arrow);
            return;
        }

        if (arrowScript != null)
            arrowScript.damage = arrowDamage;

        float direction = isFacingRight ? 1f : -1f;

        float holdPercent = bowHoldTime / maxBowHoldTime;
        float finalSpeed = arrowSpeed + (arrowSpeed * holdPercent);

        arrowRb.linearVelocity = new Vector2(direction * finalSpeed, 0f);

        Vector3 arrowScale = arrow.transform.localScale;
        arrowScale.x = Mathf.Abs(arrowScale.x) * direction;
        arrow.transform.localScale = arrowScale;

        Destroy(arrow, 3f);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}