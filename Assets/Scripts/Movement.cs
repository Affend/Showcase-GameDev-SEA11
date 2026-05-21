using UnityEngine;

public class Movement : MonoBehaviour
{
    public Rigidbody2D rb;
    private Vector2 moveInput;
    public float speed = 1f;
    private bool isFacingRight = true;

    void Update()
    {
        // Reads horizontal (A/D, Left/Right) and vertical (W/S, Up/Down) axes directly
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Flip the sprite scale only when changing horizontal direction
        if (!isFacingRight && moveInput.x > 0f)
        {
            Flip();
        }
        else if (isFacingRight && moveInput.x < 0f)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        // Smoothly move the physics body along both X and Y axes
        rb.linearVelocity = new Vector2(moveInput.x * speed, moveInput.y * speed);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localscale = transform.localScale;
        localscale.x *= -1f;
        transform.localScale = localscale;
    }
}
