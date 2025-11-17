using UnityEngine;

public class PickupWeapon : Interactable
{
    public WeaponData weaponData;

    public override void Interact(GameObject interactor)
    {
        InventoryManager.Instance.AddWeapon(weaponData);
        Destroy(gameObject);
    }
}
