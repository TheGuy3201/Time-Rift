using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Weapon Details")]
    public string weaponName;

    public enum WeaponType
    {
        RiftCrystal,
        RappidRailgun,
        BlastPistol,
        TheScarab
    }

    public float damage;
    public float range;
    public int ammoCapacity;
    public float reloadTime;
    public float bulletSpeed;
    public Sprite weaponSprite;
    public string audioName;

    public string reloadAudio;
    public bool isFullAuto;

    public Sprite bulletSprite;
}
