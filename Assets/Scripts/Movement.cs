using UnityEngine;

public class Movement : MonoBehaviour
{
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

    private bool isBowCharging = false;
    private float bowHoldTime = 0f;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.magnitude > 1)
        {
            moveInput.Normalize();
        }

        if (!isFacingRight && moveInput.x > 0f)
        {
            Flip();
        }
        else if (isFacingRight && moveInput.x < 0f)
        {
            Flip();
        }

        anim.SetBool("walk", moveInput.magnitude > 0);

        // ======================
        // HOLD + HEAVY ATTACK
        // Klik kiri
        // ======================
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
            {
                HeavyAttack();
            }
            else
            {
                NormalAttack();
            }

            attackHoldTime = 0f;
        }

        // ======================
        // BOW ATTACK
        // Klik kanan
        // ======================
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
        Debug.Log("Normal Attack!");
    }

    private void HeavyAttack()
    {
        anim.ResetTrigger("Attack");
        anim.SetTrigger("HeavyAttack");
        Debug.Log("HEAVY ATTACK!");
    }

    private void ShootArrow()
    {
        if (arrowPrefab == null || arrowSpawnPoint == null)
        {
            Debug.LogWarning("Arrow Prefab atau Arrow Spawn Point belum dipasang di Inspector!");
            return;
        }

        GameObject arrow = Instantiate(
            arrowPrefab,
            arrowSpawnPoint.position,
            arrowSpawnPoint.rotation
        );

        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();

        if (arrowRb == null)
        {
            Debug.LogWarning("Arrow prefab belum punya Rigidbody2D!");
            return;
        }

        float direction = isFacingRight ? 1f : -1f;

        float holdPercent = bowHoldTime / maxBowHoldTime;
        float finalSpeed = arrowSpeed + (arrowSpeed * holdPercent);

        arrowRb.linearVelocity = new Vector2(direction * finalSpeed, 0f);

        Vector3 arrowScale = arrow.transform.localScale;
        arrowScale.x = Mathf.Abs(arrowScale.x) * direction;
        arrow.transform.localScale = arrowScale;

        Debug.Log("Shoot Arrow!");
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 localscale = transform.localScale;
        localscale.x *= -1f;
        transform.localScale = localscale;
    }
}