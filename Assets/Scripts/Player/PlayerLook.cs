using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;     // Cuerpo
    public Camera cam;           // Cámara

    [Header("Rotación")]
    public float pitchMin = -80f;
    public float pitchMax = 80f;

    private float pitch;
    private Vector2 lookInput;

    private float recoilX;
    private float recoilY;

    void Start()
    {
        if (cam == null) cam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InputManager.Instance.OnLook += i => lookInput = i;
    }

    void Update()
    {
        ApplyLook();
    }

    void ApplyLook()
    {
        float yaw = lookInput.x * InputManager.Instance.LookSensitivity * Time.deltaTime + recoilX;
        float deltaPitch = lookInput.y * InputManager.Instance.LookSensitivity * Time.deltaTime + recoilY;

        recoilX = 0;
        recoilY = 0;

        // Rotación horizontal del cuerpo
        player.Rotate(Vector3.up * yaw);

        // Rotación vertical de la cámara
        pitch -= deltaPitch;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        cam.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }

    public void AddRecoil(float x, float y)
    {
        recoilX += x;
        recoilY += y;
    }

}
