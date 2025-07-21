using UnityEngine;

public class WeaponINIT : MonoBehaviour
{
    [Header("Weapon Initialization")]
    public Weapon weapon;

    private Vector3 initialPosition;
    private float bobAmplitude = 0.15f;
    private float bobFrequency = 2f;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            // Transfer weapon data to the player
            collider.gameObject.GetComponent<PlayerController>().bulletSpeed = weapon.bulletSpeed;
            collider.gameObject.GetComponent<PlayerController>().bulletLife = weapon.range;
            collider.gameObject.GetComponent<PlayerController>().damage = weapon.damage;
            collider.gameObject.GetComponent<PlayerController>().currentAmmo = weapon.ammoCapacity;
            collider.gameObject.GetComponent<PlayerController>().ammoCapacity = weapon.ammoCapacity;
            collider.gameObject.GetComponent<PlayerController>().reloadTime = weapon.reloadTime;
            collider.gameObject.GetComponent<PlayerController>().barrel.GetComponent<SpriteRenderer>().sprite = weapon.weaponSprite;
            collider.gameObject.GetComponent<PlayerController>().audioName = weapon.audioName;
            collider.gameObject.GetComponent<PlayerController>().reloadAudio = weapon.reloadAudio;
            collider.gameObject.GetComponent<PlayerController>().IsFullAuto = weapon.isFullAuto;
            collider.gameObject.GetComponent<PlayerController>().bulletSprite = weapon.bulletSprite;

            // Set the weapon display UI with the current weapon
            var gameplayUI = GameObject.Find("GameplayUI");
            if (gameplayUI != null)
            {
                var weaponDisplay = gameplayUI.transform.Find("PNL_WeaponDisplay");
                if (weaponDisplay != null)
                {
                    var imgWeapon = weaponDisplay.Find("IMG_WeaponImage");
                    if (imgWeapon != null)
                    {
                        var img = imgWeapon.GetComponent<UnityEngine.UI.Image>();
                        if (img != null && weapon.weaponSprite != null)
                        {
                            img.sprite = weapon.weaponSprite;
                        }
                    }
                    var lblWeaponName = weaponDisplay.Find("LBL_WeaponName");
                    if (lblWeaponName != null)
                    {
                        var txt = lblWeaponName.GetComponent<TMPro.TextMeshProUGUI>();
                        if (txt != null)
                        {
                            txt.text = weapon.weaponName;
                        }
                    }
                }
            }

            Destroy(this.gameObject); // Destroy the weapon object after pickup
        }
    }

    void Start()
    {
        // Ensure the weapon object has a SpriteRenderer and assign the weapon sprite
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = gameObject.AddComponent<SpriteRenderer>();
        }
        if (weapon != null && weapon.weaponSprite != null)
        {
            sr.sprite = weapon.weaponSprite;
        }

        // Ensure the weapon object has a Collider2D
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
        }

        // Store initial position for bobbing
        initialPosition = transform.position;
    }

    void Update()
    {
        // Bob up and down if weapon is not picked up
        transform.position = initialPosition + Vector3.up * Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
    }
}
