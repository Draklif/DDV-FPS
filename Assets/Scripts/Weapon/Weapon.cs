using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponData data;
    public Transform shootPosition;
    public int currentAmmo;

    public void Initialize(WeaponData newData)
    {
        data = newData;

        // Instanciar el modelo
        GameObject model = Instantiate(data.weaponPrefab, transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;

        // Buscar el shootPosition dentro del prefab
        shootPosition = model.transform.Find("ShootPosition");
        if (shootPosition == null) Debug.LogError($"{data.weaponName} no tiene un ShootPosition en el prefab!");

        // Set munición
        currentAmmo = data.magazineSize;
    }
}
