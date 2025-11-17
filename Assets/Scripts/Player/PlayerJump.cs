using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Jump")]
    public float jumpForce = 6f;
    public int extraJumps = 2;

    [Header("Buffer")]
    public float jumpBufferTime = 0.12f;
    private float bufferTimer;

    [Header("Variable Height")]
    public float fallMultiplier = 2f;
    public float lowJumpMultiplier = 3f;

    private PlayerMovement movement;
    private Rigidbody rb;

    private bool jumpHeld;
    private int jumpsRemaining;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();

        InputManager.Instance.OnJumpPressed += OnJumpPressed;
        InputManager.Instance.OnJumpReleased += OnJumpReleased;

        jumpsRemaining = extraJumps;
    }

    void OnJumpPressed()
    {
        bufferTimer = jumpBufferTime;
        jumpHeld = true;
    }

    void OnJumpReleased()
    {
        jumpHeld = false;
    }

    void FixedUpdate()
    {
        if (bufferTimer > 0) bufferTimer -= Time.fixedDeltaTime;

        HandleGroundReset();
        ProcessJump();
        ApplyVariableHeight();
    }

    void HandleGroundReset()
    {
        if (movement.IsGrounded) jumpsRemaining = extraJumps;
    }

    void ProcessJump()
    {
        if (bufferTimer <= 0f) return;

        // Jump from ground
        if (movement.IsGrounded)
        {
            PerformJump();
            return;
        }

        // Jump in mid air
        if (jumpsRemaining > 0) PerformJump();
    }

    void PerformJump()
    {
        movement.SetVerticalVelocity(jumpForce);
        jumpsRemaining--;
        bufferTimer = 0f;
    }

    void ApplyVariableHeight()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !jumpHeld)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
}
