using UnityEngine;

public class PickupAmmo : Interactable
{
    public AmmoType type;
    public int amount = 30;

    public override void Interact(GameObject interactor)
    {
        InventoryManager.Instance.AddAmmo(type, amount);

        Destroy(gameObject);
    }
}
