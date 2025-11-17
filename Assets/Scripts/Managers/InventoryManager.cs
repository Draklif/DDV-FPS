using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Armas iniciales")]
    public List<WeaponData> startingWeapons;

    [Header("Munición inicial")]
    public List<AmmoEntry> startingAmmo;

    private List<WeaponData> weapons = new List<WeaponData>();
    private Dictionary<AmmoType, int> ammoDict = new Dictionary<AmmoType, int>();
    private Dictionary<WeaponData, int> clipAmmo = new Dictionary<WeaponData, int>();

    public int CurrentIndex { get; private set; } = 0;

    public event Action<WeaponData> OnInventoryWeaponChanged;


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
        // Cargar armas iniciales
        weapons.AddRange(startingWeapons);

        // Cargar ammo inicial
        foreach (var entry in startingAmmo) ammoDict[entry.type] = entry.amount;

        NotifyWeaponChanged();

        InputManager.Instance.OnNext += NextWeapon;
        InputManager.Instance.OnPrevious += PreviousWeapon;
    }

    public WeaponData GetCurrentWeapon()
    {
        if (weapons.Count == 0) return null;
        return weapons[CurrentIndex];
    }

    public void NextWeapon()
    {
        if (weapons.Count <= 1) return;

        CurrentIndex++;
        if (CurrentIndex >= weapons.Count) CurrentIndex = 0;

        NotifyWeaponChanged();
    }

    public void PreviousWeapon()
    {
        if (weapons.Count <= 1) return;

        CurrentIndex--;
        if (CurrentIndex < 0) CurrentIndex = weapons.Count - 1;

        NotifyWeaponChanged();
    }

    public int GetClipAmmo(WeaponData weapon)
    {
        if (clipAmmo.TryGetValue(weapon, out int ammo)) return ammo;
        return weapon.magazineSize;
    }

    public void SetClipAmmo(WeaponData weapon, int amount)
    {
        clipAmmo[weapon] = Mathf.Max(0, amount);
    }

    private void NotifyWeaponChanged()
    {
        OnInventoryWeaponChanged?.Invoke(GetCurrentWeapon());
    }

    public bool TryConsumeAmmo(AmmoType type, int amount)
    {
        if (!ammoDict.ContainsKey(type)) return false;
        if (ammoDict[type] < amount) return false;

        ammoDict[type] -= amount;
        return true;
    }

    public int GetAmmo(AmmoType type)
    {
        return ammoDict.ContainsKey(type) ? ammoDict[type] : 0;
    }

    public void AddAmmo(AmmoType type, int amount)
    {
        if (!ammoDict.ContainsKey(type)) ammoDict[type] = 0;
        ammoDict[type] += amount;
    }

    public void AddWeapon(WeaponData weaponData)
    {
        if (weaponData == null) return;

        // Ya tengo esta arma
        if (weapons.Contains(weaponData)) return;

        // Añadir arma
        weapons.Add(weaponData);

        // Inicializar su munición en el cargador
        if (!clipAmmo.ContainsKey(weaponData)) clipAmmo[weaponData] = weaponData.magazineSize;

        // Seleccionar automáticamente la nueva arma
        CurrentIndex = weapons.Count - 1;

        NotifyWeaponChanged();
    }
}
