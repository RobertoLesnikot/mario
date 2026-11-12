using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpHeight;
    private Rigidbody2D rb;
    public Animator animator;
    private SpriteRenderer spriteRenderer;

    private float inputHorizontal;
    public Transform groundCheck;
    public LayerMask groundLayer;
    
    private bool jumpPressed;
    private bool isGrounded;
    private bool hasLeftGround;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update() 
    {
        inputHorizontal = Input.GetAxis("Horizontal");

        // Run animation logic
        if (inputHorizontal != 0)
        {
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);  
        }

        // Capture jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
        }

        // Sprite flipping logic
        if (inputHorizontal > 0.01f)
        {
            spriteRenderer.flipX = false; // Face right
        }
        else if (inputHorizontal < -0.01f)
        {
            spriteRenderer.flipX = true;  // Face left
        }
    }

    private void FixedUpdate()
    {
        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // Move horizontally
        rb.linearVelocity = new Vector2(inputHorizontal * moveSpeed, rb.linearVelocity.y);

        // Handle Jumping
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false); // Make sure fall is off during jump
            jumpPressed = false;
            hasLeftGround = false;
        }
        else
        {
            jumpPressed = false;
        }

        // Once we are in the air
        if (!isGrounded)
        {
            hasLeftGround = true;

            // If we are falling DOWNWARD, but we DID NOT jump (meaning we walked off a ledge)
            if (!animator.GetBool("IsJumping") && rb.linearVelocity.y < -1.0f)
            {
                animator.SetBool("IsFalling", true);
            }
        }

        // Landing check
        if (isGrounded && hasLeftGround && rb.linearVelocity.y <= 0.1f)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false); // Reset fall state upon landing
            hasLeftGround = false;
        }
    }
}