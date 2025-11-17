using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI currentAmmoText;
    public TextMeshProUGUI totalAmmoText;
    public Image weaponIcon;

    private Weapon currentWeapon;

    void Start()
    {
        // Suscribirse al cambio de arma
        WeaponManager.Instance.OnWeaponChanged += UpdateWeaponUI;

        // Inicializar si hay arma activa
        currentWeapon = WeaponManager.Instance.CurrentWeapon;
        UpdateWeaponUI(currentWeapon);
    }

    void Update()
    {
        if (currentWeapon == null) return;

        // Actualizar munición cada frame
        currentAmmoText.text = currentWeapon.currentAmmo.ToString();

        // Ammo total en inventario para este tipo
        int totalAmmo = InventoryManager.Instance.GetAmmo(currentWeapon.data.ammoType);
        totalAmmoText.text = totalAmmo.ToString();
    }

    void UpdateWeaponUI(Weapon newWeapon)
    {
        currentWeapon = newWeapon;

        if (currentWeapon != null)
        {
            weaponIcon.sprite = currentWeapon.data.weaponIcon;

            currentAmmoText.text = currentWeapon.currentAmmo.ToString();

            int totalAmmo = InventoryManager.Instance.GetAmmo(currentWeapon.data.ammoType);
            totalAmmoText.text = totalAmmo.ToString();
        }
        else
        {
            weaponIcon.sprite = null;
            currentAmmoText.text = "-";
            totalAmmoText.text = "-";
        }
    }
}
