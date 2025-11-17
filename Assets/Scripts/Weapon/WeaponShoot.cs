using System.Collections;
using UnityEngine;

public class WeaponShoot : MonoBehaviour
{
    public PlayerLook playerLook;

    private Camera cam;

    private float fireCooldown;
    private bool isShooting;
    private bool canSemiAutoShoot;
    private bool isReloading = false;
    private WeaponSway weaponSway;

    void Start()
    {
        cam = Camera.main;

        weaponSway = GetComponent<WeaponSway>();

        InputManager.Instance.OnAttackStarted += HandleShootStarted;
        InputManager.Instance.OnAttackEnded += HandleShootEnded;
        InputManager.Instance.OnReload += HandleReload;
    }

    void Update()
    {
        if (fireCooldown > 0) fireCooldown -= Time.deltaTime;

        if (!isShooting) return;

        Weapon weapon = WeaponManager.Instance.CurrentWeapon;
        if (weapon == null) return;

        if (weapon.data.fireMode == FireMode.FullAuto && isShooting) TryFire(weapon);
    }
    void HandleReload()
    {
        Weapon weapon = WeaponManager.Instance.CurrentWeapon;

        if (weapon == null) return;
        if (isReloading) return;
        if (weapon.currentAmmo >= weapon.data.magazineSize) return;

        int invAmmo = InventoryManager.Instance.GetAmmo(weapon.data.ammoType);
        if (invAmmo <= 0) return;

        StartCoroutine(ReloadRoutine(weapon));
    }

    IEnumerator ReloadRoutine(Weapon weapon)
    {
        isReloading = true;
        isShooting = false;
        canSemiAutoShoot = true;

        StartCoroutine(weaponSway.PlayReloadAnimation(weapon.data.reloadTime));

        yield return new WaitForSeconds(weapon.data.reloadTime);

        int magSize = weapon.data.magazineSize;
        int current = weapon.currentAmmo;
        int needed = magSize - current;

        if (needed <= 0)
        {
            isReloading = false;
            yield break;
        }

        // Cuánta munición tengo realmente
        int available = InventoryManager.Instance.GetAmmo(weapon.data.ammoType);

        if (available <= 0)
        {
            isReloading = false;
            yield break;
        }

        // Cuánta munición puedo tomar del inventario
        int toLoad = Mathf.Min(needed, available);

        // Consumir del inventario
        InventoryManager.Instance.TryConsumeAmmo(weapon.data.ammoType, toLoad);

        // Cargar arma
        weapon.currentAmmo += toLoad;
        InventoryManager.Instance.SetClipAmmo(weapon.data, weapon.currentAmmo);

        isReloading = false;
    }


    void HandleShootStarted()
    {
        Weapon weapon = WeaponManager.Instance.CurrentWeapon;

        if (isReloading) return;

        if (weapon.currentAmmo <= 0)
        {
            HandleReload();
            return;
        }

        if (weapon == null) return;

        isShooting = true;

        switch (weapon.data.fireMode)
        {
            case FireMode.FullAuto:
                TryFire(weapon);
                break;

            case FireMode.SemiAuto:
                if (canSemiAutoShoot)
                {
                    TryFire(weapon);
                    canSemiAutoShoot = false;
                }
                break;
        }
    }

    void HandleShootEnded()
    {
        isShooting = false;
        canSemiAutoShoot = true;
    }

    void TryFire(Weapon weapon)
    {
        if (fireCooldown > 0) return;

        if (weapon.currentAmmo <= 0)
        {
            isShooting = false;
            HandleReload();
            return;
        }

        Shoot(weapon);
        fireCooldown = 1f / weapon.data.fireRate;
    }

    void Shoot(Weapon weapon)
    {
        weapon.currentAmmo--;
        if (weapon.currentAmmo < 0) weapon.currentAmmo = 0;
        InventoryManager.Instance.SetClipAmmo(weapon.data, weapon.currentAmmo);

        Transform shootPosition = weapon.shootPosition;

        Vector3 origin = cam.transform.position;
        Vector3 dir = cam.transform.forward;

        // Recoil
        playerLook.AddRecoil(
            Random.Range(-weapon.data.recoilX, weapon.data.recoilX), weapon.data.recoilY
        );

        Debug.DrawRay(origin, dir * weapon.data.range, Color.red, 0.1f);

        if (Physics.Raycast(origin, dir, out RaycastHit hit, weapon.data.range))
        {
            Debug.Log($"Hit: {hit.collider.name}");

            // Check for Health component
            Health targetHealth = hit.collider.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(weapon.data.damage);
                Debug.Log($"Applied {weapon.data.damage} damage to {hit.collider.name}");
            }
        }
    }
}
