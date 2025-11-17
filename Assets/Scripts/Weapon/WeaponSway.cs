using System.Collections;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway")]
    public float swayAmount = 0.01f;
    public float swaySmooth = 10f;

    [Header("ADS")]
    public Transform adsPosition;
    public float adsSpeed = 12f;
    public float adsSwayMultiplier = 0.2f;

    [Header("ADS Zoom")]
    public float adsFOV = 45f;
    public float zoomSpeed = 10f;

    private Camera cam;
    private float defaultFOV;

    [Header("Reload")]
    public Vector3 reloadOffset = new Vector3(0f, -0.3f, -0.2f);
    public float reloadMoveSpeed = 10f;

    private Vector3 basePos;
    private Vector3 reloadPos;
    private Vector2 lookInput;
    private bool isADS;
    private bool isReloading;

    void Start()
    {
        basePos = transform.localPosition;
        reloadPos = basePos + reloadOffset;

        cam = Camera.main;
        defaultFOV = cam.fieldOfView;

        InputManager.Instance.OnLook += i => lookInput = i;
        InputManager.Instance.OnAim += a => isADS = a;
    }

    void Update()
    {
        if (isReloading) return;

        ApplySway();
        ApplyADS();
        ApplyADSZoom();
    }

    public IEnumerator PlayReloadAnimation(float duration)
    {
        isReloading = true;

        float half = duration * 0.4f; // 40% bajar, 60% subir

        // BAJAR
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * reloadMoveSpeed;
            transform.localPosition = Vector3.Lerp(basePos, reloadPos, t);
            yield return null;
        }

        // ESPERAR
        yield return new WaitForSeconds(duration - half);

        // SUBIR
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * reloadMoveSpeed;
            transform.localPosition = Vector3.Lerp(reloadPos, basePos, t);
            yield return null;
        }

        isReloading = false;
    }


    void ApplySway()
    {
        float mul = isADS ? adsSwayMultiplier : 1f;

        float swayX = -lookInput.x * swayAmount * mul;
        float swayY = -lookInput.y * swayAmount * mul;

        Vector3 target = basePos + new Vector3(swayX, swayY, 0f);
        transform.localPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * swaySmooth);
    }

    void ApplyADS()
    {
        Vector3 target = isADS ? adsPosition.localPosition : basePos;
        transform.localPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * adsSpeed);
    }

    void ApplyADSZoom()
    {
        float targetFOV = isADS ? adsFOV : defaultFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }
}
