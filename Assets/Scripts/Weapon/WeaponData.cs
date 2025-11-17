using UnityEngine;

public enum FireMode
{
    SemiAuto,
    FullAuto
}

[CreateAssetMenu(menuName = "FPS/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("General")]
    public string weaponName;
    public GameObject weaponPrefab;
    public Sprite weaponIcon;

    [Header("Shooting")]
    public float damage = 20f;
    public float fireRate = 10f;
    public float range = 100f;
    public float bulletSpread = 0.02f;
    public FireMode fireMode = FireMode.FullAuto;

    [Header("Reload")]
    public AmmoType ammoType = AmmoType.Light;
    public int magazineSize = 30;
    public float reloadTime = 1.5f;

    [Header("Camera Recoil")]
    public float recoilX = 1f;
    public float recoilY = 1f;
}
