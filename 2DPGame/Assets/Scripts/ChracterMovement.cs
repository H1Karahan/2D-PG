using UnityEngine;

public class ChracterMovement : MonoBehaviour
{
    private float movementInputDirection;
    
    private float dashTimeLeft;
    private float lastDash;

    private int amountOfJumpsLeft;


    private bool isFacingRight = true;

    private bool isGrounded;
    private bool isWalking;
    private bool isJumping;
    private bool isFalling;
    private bool isIdle;
    private bool isTouchingWall;
    private bool isDashing;

    private bool canJump;
    private bool canMove = true;
    private bool canFlip;
    private bool checkJumpMultiplier;


    private Rigidbody2D rb;
    private Animator anim;

    public int amountOfJumps = 1;

    public float movementSpeed;
    public float jumpForce;
    public float groundCheckRadius;
    public float airDragMultiplier;
    public float variableJumpHeightMultiplier;
    public float wallCheckDistance;

    public float dashTime;
    public float dashSpeed;
    public float dashCoolDown;

    public Transform groundCheck;
    public Transform wallCheck;

    public LayerMask whatIsGround;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        amountOfJumpsLeft = amountOfJumps;
    }

    private void Update()
    {
        CheckMovementDirectionAndAnimations();
        CheckInput();
        CheckIfCanJump();
        UpdateAnimations();
        CheckDash();

    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }



    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);

    }

    private void CheckIfCanJump()
    {
        if(isGrounded && rb.velocity.y <= 0.01)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if(amountOfJumpsLeft <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }

        if(isTouchingWall)
        {
            amountOfJumpsLeft = 0;
        }

    }

    private void CheckMovementDirectionAndAnimations()
    {
        if(isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if(!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if(Mathf.Abs(rb.velocity.x) >= 0.01f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        if(isJumping)
        {
            if(rb.velocity.y < Mathf.Epsilon)
            {
                isJumping = false;
                isFalling = true;
            }
        }
        if(isFalling)
        {
            if(isGrounded)
            {
                isFalling = false;
                isIdle = true;
            }
            else
            {
                isIdle = false;
            }
        }

    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isFalling", isFalling);
        anim.SetBool("isDashing", isDashing);
        anim.SetBool("isIdle", isIdle);
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if(checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }

        if(Input.GetButtonDown("Dash"))
        {
            if (Time.time >= lastDash + dashCoolDown && Mathf.Abs(rb.velocity.x) > 0)
            {
                AttemptToDash();
            }
            
        }
    }

    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
    }

    private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;

                rb.velocity = new Vector2(dashSpeed * movementInputDirection, 0);
                dashTimeLeft -= Time.deltaTime;
            }

            if (dashTimeLeft <= 0 || isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }

        }
    }

    private void ApplyMovement()
    {
        if (canMove)
        {
            if (!isGrounded && movementInputDirection == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
            }
        }
    }

    private void Jump()
    {
        if(canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
            checkJumpMultiplier = true;

            isFalling = false;
            isJumping = true;
        }
    }

    public void DisableFlip()
    {
        canFlip = false;
    }

    public void EnableFlip()
    {
        canFlip = true;
    }

    private void Flip()
    {
        canFlip = true;
        isFacingRight = !isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
