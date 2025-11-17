using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public float dashForce = 20f;
    public float dashCooldown = 1f;
    public int maxDashes = 1;
    public float dashDuration = 0.15f;

    private int dashesRemaining;
    private float cooldownTimer;
    private bool isDashing;

    private PlayerMovement movement;
    private Transform cam;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        dashesRemaining = maxDashes;
        cam = Camera.main.transform;

        InputManager.Instance.OnDash += HandleDash;
    }

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0) RestoreDashes();
        }
    }


    void HandleDash()
    {
        if (cooldownTimer > 0) return;
        if (dashesRemaining <= 0) return;
        if (isDashing) return;

        StartCoroutine(DashRoutine());
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        dashesRemaining--;

        // Obtener input
        Vector2 input = movement.MoveInput;

        Vector3 dir;

        if (input.sqrMagnitude > 0.1f)
        {
            // Dash según input relativo a la cámara
            Vector3 camForward = cam.forward; camForward.y = 0; camForward.Normalize();
            Vector3 camRight = cam.right; camRight.y = 0; camRight.Normalize();

            dir = camForward * input.y + camRight * input.x;
        }
        else
        {
            // Dash hacia adelante si no hay input
            dir = cam.forward;
        }

        dir.y = 0;
        dir.Normalize();

        movement.IsDashing = true;
        movement.AddExternalVelocity(dir * dashForce);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        movement.IsDashing = false;

        cooldownTimer = dashCooldown;
    }


    public void RestoreDashes()
    {
        dashesRemaining = maxDashes;
    }
}
