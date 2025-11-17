using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 12f;
    public float airControl = 0.5f;
    public float sprintMultiplier = 1.6f;

    [Header("Crouch")]
    public float crouchSpeedMultiplier = 0.5f;
    public float crouchHeight = 1f;
    private float originalHeight;
    private CapsuleCollider col;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundMask;

    public bool IsGrounded { get; private set; }
    public bool IsDashing { get; set; }
    public Rigidbody RB { get; private set; }
    public Vector2 MoveInput { get; private set; }

    private bool isSprinting;
    private bool isCrouching;

    private Transform cam;

    void Start()
    {
        RB = GetComponent<Rigidbody>();
        RB.freezeRotation = true;
        RB.interpolation = RigidbodyInterpolation.Interpolate;

        col = GetComponent<CapsuleCollider>();
        originalHeight = col.height;

        cam = Camera.main.transform;

        InputManager.Instance.OnMove += input => MoveInput = input;
        InputManager.Instance.OnSprintPressed += () => isSprinting = true;
        InputManager.Instance.OnSprintReleased += () => isSprinting = false;
        InputManager.Instance.OnCrouchPressed += StartCrouch;
        InputManager.Instance.OnCrouchReleased += StopCrouch;
    }

    void StartCrouch()
    {
        isCrouching = true;
        col.height = crouchHeight;
    }

    void StopCrouch()
    {
        isCrouching = false;
        col.height = originalHeight;
    }

    void FixedUpdate()
    {
        CheckGround();
        MovePlayer();
    }

    void CheckGround()
    {
        IsGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundMask);
    }

    void MovePlayer()
    {
        if (IsDashing) return;

        Vector3 camForward = cam.forward;
        camForward.y = 0; camForward.Normalize();

        Vector3 camRight = cam.right;
        camRight.y = 0; camRight.Normalize();

        Vector3 targetDir = camForward * MoveInput.y + camRight * MoveInput.x;

        float control = IsGrounded ? 1f : airControl;

        float speed = moveSpeed;

        if (isSprinting && !isCrouching && IsGrounded)
            speed *= sprintMultiplier;

        if (isCrouching)
            speed *= crouchSpeedMultiplier;

        Vector3 targetVelocity = targetDir * speed * control;

        Vector3 velocity = Vector3.Lerp(
            new Vector3(RB.linearVelocity.x, 0, RB.linearVelocity.z),
            targetVelocity,
            Time.fixedDeltaTime * acceleration
        );

        RB.linearVelocity = new Vector3(velocity.x, RB.linearVelocity.y, velocity.z);
    }

    public void SetVerticalVelocity(float v)
    {
        RB.linearVelocity = new Vector3(RB.linearVelocity.x, v, RB.linearVelocity.z);
    }

    public void AddExternalVelocity(Vector3 vel)
    {
        RB.linearVelocity = vel;
    }
}
