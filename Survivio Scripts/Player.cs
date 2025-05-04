using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public struct AmmoType
    {
        public int ammoOnHand;
        public int ammoMax;
    }

    public struct AmmoInventory
    {
        public AmmoType pistolAmmo;
        public AmmoType shotgunAmmo;
        public AmmoType autoAmmo;

        public void setMax(int pistol, int shotgun, int auto)
        {
            this.pistolAmmo.ammoMax = pistol;
            this.shotgunAmmo.ammoMax = shotgun;
            this.autoAmmo.ammoMax = auto;
        }
        public void addAmmo(int ammoType, int amount)
        {
            switch(ammoType)
            {
                case 0: //pistol
                    pistolAmmo.ammoOnHand += amount;
                    break;
                case 1: //shotgun
                    shotgunAmmo.ammoOnHand += amount;
                    break;
                case 2: //auto
                    autoAmmo.ammoOnHand += amount;
                    break;
                default:
                    break;
            }
        }
        public void checkAmmo()
        {
            if (pistolAmmo.ammoOnHand > pistolAmmo.ammoMax)
            {
                pistolAmmo.ammoOnHand = pistolAmmo.ammoMax;
            }
            else if (shotgunAmmo.ammoOnHand > shotgunAmmo.ammoMax)
            {
                shotgunAmmo.ammoOnHand = shotgunAmmo.ammoMax;
            }
            else if (autoAmmo.ammoOnHand > autoAmmo.ammoMax)
            {
                autoAmmo.ammoOnHand = autoAmmo.ammoMax;
            }
        }
    }

    public AmmoInventory inventory;
    public GameObject selectedGun;
    public GameObject playerPrimary;
    public GameObject playerSecondary;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Rigidbody2D rb;

    [SerializeField] Camera cam;

    [SerializeField] Joystick moveStick;
    [SerializeField] Joystick lookStick;

    int maxHealth = 100;
    public int currentHealth;
    public bool isDead = false;
    [SerializeField] bool isInvulnerable = false;

    Vector2 movement;
    Vector2 mousePos;

    private void Start()
    {
        currentHealth = maxHealth;
        inventory.setMax(90, 60, 120);
        selectedGun = new GameObject();
        selectedGun.name = "None";
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_2017_1_OR_NEWER
        editorControls();
#elif UNITY_ANDROID || UNITY_IOS
        mobileControls();
#endif

        //camera follow 
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        if (currentHealth <= 0 && !isInvulnerable) //dead
        {
            isDead = true;
        }

        if(isDead)
        {
            Destroy(gameObject);
        }
    }

    void editorControls()
    {
        //input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        //get position of mouse and convert to game units
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        //movement
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        //player look at mouse
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;

        //gun
        if (Input.GetMouseButton(0) && selectedGun)
        {
            selectedGun.GetComponent<Gun>()?.buttonFire();
        }
    }

    void mobileControls()
    {
        //movement
        movement.x = moveStick.Horizontal * moveSpeed;
        movement.y = moveStick.Vertical * moveSpeed;
        rb.MovePosition(rb.position + (movement * Time.fixedDeltaTime));

        //looking
        float angle = Mathf.Atan2(lookStick.Vertical, lookStick.Horizontal) * Mathf.Rad2Deg - 90f;
        if(lookStick.Horizontal != 0f) //dont update looking if stick is resting
        {
            rb.rotation = angle;
        }
    }

    public void takeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void healHealth(int healingValue)
    {
        currentHealth += healingValue;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    void OnTriggerEnter2D(Collider2D other) //looting
    {
        if (other.tag == "Ammo")
        {
            //destroy ammo in world
            Destroy(other.gameObject);

            if (other.gameObject.name.StartsWith("9mm"))
            {
                inventory.addAmmo(0, 15);
            }
            else if (other.gameObject.name.StartsWith("12Gauge"))
            {
                inventory.addAmmo(1, 2);
            }
            else if (other.gameObject.name.StartsWith("5.56mm"))
            {
                inventory.addAmmo(2, 30);
            }

            inventory.checkAmmo();
        }
        else if(other.tag == "Gun")
        {
            if(selectedGun.name == "None")
            {
                Destroy(selectedGun);
            }

            if (other.GetComponent<WorldGun>().isPrimaryWeapon && playerPrimary == null) //if primary
            {
                playerPrimary = Instantiate(other.GetComponent<WorldGun>().itselfModel, transform);
                Destroy(other.gameObject);
                selectedGun?.SetActive(false);
                selectedGun = playerPrimary;
            }
            else if (!other.GetComponent<WorldGun>().isPrimaryWeapon && playerSecondary == null) //if secondary
            {
                playerSecondary = Instantiate(other.GetComponent<WorldGun>().itselfModel, transform);
                Destroy(other.gameObject);
                selectedGun?.SetActive(false);
                selectedGun = playerSecondary;
            }
            else if ( //switch weapon
                other.GetComponent<WorldGun>().isPrimaryWeapon && //if primary weapon
                playerPrimary != null && //if player has a primary weapon already
                !other.name.StartsWith(playerPrimary.GetComponent<Gun>().itself.name)) //if not the same weapon
            {
                Vector2 dropBehind = cam.ScreenToWorldPoint(Input.mousePosition);
                dropBehind -= rb.position;
                dropBehind = -2 * dropBehind.normalized;

                Instantiate( //make gun in world
                    playerPrimary.GetComponent<Gun>().itself, 
                    new Vector2(transform.position.x + dropBehind.x, transform.position.y + dropBehind.y), 
                    Quaternion.identity);
                Destroy(playerPrimary);
                selectedGun?.SetActive(false);
                playerPrimary = Instantiate(other.GetComponent<WorldGun>().itselfModel, transform); //make player gun model
                selectedGun = playerPrimary;
                Destroy(other.gameObject);
            }

            /*
            //switch gun in hand if there is any
            if(selectedGun.name != "None")
            {
                Instantiate(selectedGun.GetComponent<Gun>().itself, new Vector2(transform.position.x + 1, transform.position.y + 1), Quaternion.identity);
                Destroy(selectedGun);
            }

            if (other.gameObject.name.StartsWith("Pistol") && playerPistol == null)
            {
                //destroy gun in the world, aslo makes sure that player cannot pick the same gun in the world
                Destroy(other.gameObject);
                //make player gun
                playerPistol = Instantiate(weaponPrefabs[0], transform);
                //if there is a selected gun, set it off
                selectedGun?.SetActive(false);
                //have it selected
                selectedGun = playerPistol;
            }
            else if (other.gameObject.name.StartsWith("Shotgun") && playerShotgun == null)
            {
                Destroy(other.gameObject);
                playerShotgun = Instantiate(weaponPrefabs[1], transform);
                selectedGun?.SetActive(false);
                selectedGun = playerShotgun;
            }
            else if (other.gameObject.name.StartsWith("Auto") && playerAuto == null)
            {
                Destroy(other.gameObject);
                playerAuto = Instantiate(weaponPrefabs[2], transform);
                selectedGun?.SetActive(false);
                selectedGun = playerAuto;
            }
            */
        }
    }
}
