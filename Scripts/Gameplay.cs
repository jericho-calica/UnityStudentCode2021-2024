using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gameplay : MonoBehaviour
{
    //player and ui
    [SerializeField] Player player;
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI weaponText;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] Button fireLeftButton;
    [SerializeField] Button fireRightButton;

    //guns
    [SerializeField] GameObject[] weaponPrefabs = new GameObject[3];
    [SerializeField] GameObject weaponOrganizer;
    private bool pistolActivated = false;
    private bool shotgunActivated = false;
    private bool autoActivated = false;

    //ammo
    [SerializeField] GameObject[] ammoPrefabs = new GameObject[3];
    [SerializeField] GameObject ammoOrganizer;

    //switch buttons
    [SerializeField] Button[] switches = new Button[3];

    //map
    [SerializeField] GameObject map;

    //enemy
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject enemyOrganizer;
    [SerializeField] int numOfEnemies;
    [SerializeField] List<GameObject> enemyList = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < 7; i++) //spawn guns
        {
            for(int j = 0; j < 3; j++)
            {
                spawn(weaponPrefabs[j], weaponOrganizer, 0);
            }
        }

        for (int i = 0; i < 10; i++) //spawn ammo
        {
            for (int j = 0; j < 3; j++)
            {
                spawn(ammoPrefabs[j], ammoOrganizer, 0);
            }
        }

        for (int i = 0; i < numOfEnemies; i++) //spawn enemies
        {
            spawn(enemyPrefab, enemyOrganizer, i);
        }

        GameEvents.current.onEnemyDeath += deadEnemyFinder;
    }

    void Update()
    {
        //make sure health bar is updated if player is dead
        if (player == null)
        {
            healthBar.value = 0;
            GameEvents.current.PlayerDies();
        }
        //win
        if (enemyList.Count <= 0)
        {
            GameEvents.current.PlayerWins();
        }
    }

    void FixedUpdate()
    {
        if(!player.isDead) //do all this while player is not dead
        {
            updateHealthBar();
            if(player.selectedGun != null)
            {
                updateWeaponText();
                updateAmmoText();
            }

            if (
                //left
                player.selectedGun.name != "None" && //player has a gun
                fireLeftButton.GetComponent<ButtonPressed>().isBeingPressed && //button being pressed
                !player.selectedGun.GetComponent<Gun>().isSemiAuto //player gun is not a semi auto
                ||
                //right
                player.selectedGun.name != "None" && 
                fireRightButton.GetComponent<ButtonPressed>().isBeingPressed && 
                !player.selectedGun.GetComponent<Gun>().isSemiAuto 
                
                ) 
            {
                //if gun is not a semi auto, shoot as an auto
                shootGun();
            }

            checkWeaponSwitches();
        }
    }

    public void switchGun(int gunNumber) //for button
    {
        //deactivate current gun
        player.selectedGun.SetActive(false);

        switch (gunNumber)
        {
            case 0: //pistol
                //switch
                player.selectedGun = player.playerSecondary;
                break;
            case 1: //shotgun
            case 2: //auto
                player.selectedGun = player.playerPrimary;
                break;
        }

        //activate switched gun
        player.selectedGun.SetActive(true);
    }

    public void shootGun() //for button
    {
        if (!player.isDead && player.selectedGun.name != "None")
        {
            player?.selectedGun.GetComponent<Gun>().buttonFire();
        }
    }

    public void reloadGun() //for button
    {
        if (!player.isDead && player.selectedGun.name != "None")
        {
            player?.selectedGun.GetComponent<Gun>().buttonReload();
        }
    }

    void checkWeaponSwitches()
    {

        //primary
        if (player.playerPrimary != null && player.playerPrimary.GetComponent<Shotgun>() != null) //show button if gun is available
        {
            if(!shotgunActivated)
            {
                shotgunActivated = true;
                autoActivated = false;
                Audio.instance.PlaySound(sfxenum.getshotgun);
            }
            switches[1].gameObject.SetActive(true);
        }
        else
        {
            switches[1].gameObject.SetActive(false);
        }
        if (player.playerPrimary != null && player.playerPrimary.GetComponent<Auto>() != null)
        {
            if (!autoActivated)
            {
                autoActivated = true;
                shotgunActivated = false;
                Audio.instance.PlaySound(sfxenum.getauto);
            }
            switches[2].gameObject.SetActive(true);
        }
        else
        {
            switches[2].gameObject.SetActive(false);
        }
        //secondary
        if (player.playerSecondary != null && player.playerSecondary.GetComponent<Pistol>() != null)
        {
            if (!pistolActivated)
            {
                pistolActivated = true;
                Audio.instance.PlaySound(sfxenum.getpistol);
            }
            switches[0].gameObject.SetActive(true);
        }
        else
        {
            switches[0].gameObject.SetActive(false);
        }
    }

    public void spawn(GameObject thing, GameObject organizer, int enemyNumbering)
    {
        GameObject temp;

        //spawn within map
        temp = Instantiate(thing, new Vector2(
            Random.Range((-map.transform.localScale.x / 2) + 3, (map.transform.localScale.x / 2) - 3),
            Random.Range((-map.transform.localScale.y / 2) + 3, (map.transform.localScale.y / 2) - 3)), 
            Quaternion.identity,
            organizer.transform);

        if(thing.name.StartsWith("Enemy"))
        {
            temp.name = enemyNumbering.ToString();
            enemyList.Add(temp);
        }
    }

    private void deadEnemyFinder()
    {
        StartCoroutine(findAndEraseEnemy());
    }

    private IEnumerator findAndEraseEnemy()
    {
        //remove dead enemies on the list
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i] == null)
            {
                enemyList.RemoveAt(i);
                break;
            }
        }
    }

    void updateHealthBar()
    {
        healthBar.value = player.currentHealth;
    }

    void updateWeaponText()
    {
        string weaponName = "None";

        if (player.selectedGun.name.StartsWith("Pistol"))
        {
            weaponName = "Pistol";
        }
        else if (player.selectedGun.name.StartsWith("Shotgun"))
        {
            weaponName = "Shotgun";
        }
        else if (player.selectedGun.name.StartsWith("Auto"))
        {
            weaponName = "Auto";
        }
        else
        {
            weaponName = "None";
        }

        weaponText.text = "Weapon: " + weaponName;
    }

    void updateAmmoText()
    {
        string current = "";
        string inventory = "";

        if (player.selectedGun.name.StartsWith("Pistol"))
        {
            current = player.selectedGun.GetComponent<Pistol>().currentClipSize.ToString();
            inventory = player.inventory.pistolAmmo.ammoOnHand.ToString();
        }
        else if (player.selectedGun.name.StartsWith("Shotgun"))
        {
            current = player.selectedGun.GetComponent<Shotgun>().currentClipSize.ToString();
            inventory = player.inventory.shotgunAmmo.ammoOnHand.ToString();
        }
        else if (player.selectedGun.name.StartsWith("Auto"))
        {
            current = player.selectedGun.GetComponent<Auto>().currentClipSize.ToString();
            inventory = player.inventory.autoAmmo.ammoOnHand.ToString();
        }

        ammoText.text = string.Format("Ammo: {0} / {1}", current, inventory);
    }
}
