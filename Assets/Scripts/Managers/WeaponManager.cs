using System;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Header("Settings")]
    public Transform weaponHolder;
    public WeaponData startingWeapon;

    private Weapon currentWeapon;

    public event Action<Weapon> OnWeaponChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        EquipWeapon(startingWeapon);

        InventoryManager.Instance.OnInventoryWeaponChanged += EquipWeapon;

        // Equipar la primera
        EquipWeapon(InventoryManager.Instance.GetCurrentWeapon());
    }

    public void EquipWeapon(WeaponData data)
    {
        if (currentWeapon != null)
        {
            // Guardar munición del arma que dejamos
            InventoryManager.Instance.SetClipAmmo(currentWeapon.data, currentWeapon.currentAmmo);
            Destroy(currentWeapon.gameObject);
        }

        GameObject go = new GameObject("Weapon");
        go.transform.SetParent(weaponHolder);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        currentWeapon = go.AddComponent<Weapon>();
        currentWeapon.Initialize(data);

        currentWeapon.currentAmmo = InventoryManager.Instance.GetClipAmmo(data);

        OnWeaponChanged?.Invoke(currentWeapon);
    }

    bool TryReload(Weapon weapon)
    {
        int missing = weapon.data.magazineSize - weapon.currentAmmo;

        int available = InventoryManager.Instance.GetAmmo(weapon.data.ammoType);
        if (available <= 0) return false;

        int amountToLoad = Mathf.Min(missing, available);

        weapon.currentAmmo += amountToLoad;
        InventoryManager.Instance.TryConsumeAmmo(weapon.data.ammoType, amountToLoad);
        return true;
    }


    public Weapon CurrentWeapon => currentWeapon;
}
