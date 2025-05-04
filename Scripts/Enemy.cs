using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //weapons
    [SerializeField] GameObject[] weaponPrefabs = new GameObject[3];
    [SerializeField] GameObject selectedGun;
    [SerializeField] Gun gunScript;
    //movement
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Rigidbody2D rb;
    public string direction = "None";
    Vector2 movement;
    bool canMove = true;
    //health 
    int maxHealth = 100;
    public int currentHealth;
    public bool isDead = false;
    //anyone in their sights
    public GameObject target;
    public float targetDistance;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        selectWeapon();
        gunScript = selectedGun.GetComponent<Gun>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            isDead = true;
        }

        if (isDead) //remove from world if dead
        {
            Audio.instance.PlaySound(sfxenum.enemydeath);
            Destroy(gameObject);
            GameEvents.current.EnemyDeath();
        }
    }

    void selectWeapon()
    {
        switch(Random.Range(0, 3))
        {
            case 0: //pistol
                selectedGun = Instantiate(weaponPrefabs[0], transform);
                break;
            case 1: //shotgun
                selectedGun = Instantiate(weaponPrefabs[1], transform);
                break;
            case 2: //auto
                selectedGun = Instantiate(weaponPrefabs[2], transform);
                break;
        }
    }

    public void takeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void healHealth(int healingValue)
    {
        currentHealth += healingValue;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void wander() //for state
    {
        //Debug.Log("CONSCRIPT EXCITED TO FIGHT");
        if(canMove)
        {
            StartCoroutine(randomWalk());
            canMove = false;
        }
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public IEnumerator randomWalk()
    {
        //choose direction
        switch(Random.Range(0, 10))
        {
            case 0: //ul
                movement.y = 1;
                movement.x = -1;
                rb.rotation = 45;
                direction = "Up Left";
                break;
            case 1: //u
                movement.y = 1;
                rb.rotation = 0;
                direction = "Up";
                break;
            case 2: //ur
                movement.y = 1;
                movement.x = 1;
                rb.rotation = 315;
                direction = "Up Right";
                break;
            case 3: //l
                movement.x = -1;
                rb.rotation = 90;
                direction = "Left";
                break;
            case 4: //r
                movement.x = 1;
                rb.rotation = 270;
                direction = "Right";
                break;
            case 5: //dl
                movement.y = -1;
                movement.x = -1;
                rb.rotation = 135;
                direction = "Down Left";
                break;
            case 6: //d
                movement.y = -1;
                rb.rotation = 180;
                direction = "Down";
                break;
            case 7: //dr
                movement.y = -1;
                movement.x = 1;
                rb.rotation = 225;
                direction = "Down Right";
                break;
            default:
                //idle
                movement.x = 0;
                movement.y = 0;
                break;
        }
        yield return new WaitForSeconds(Random.Range(0, 5));
        movement.x = 0;
        movement.y = 0;
        canMove = true;
        StopCoroutine(randomWalk());
    }

    public void chase() //for state
    {
        if(target != null)
        {
            //Debug.Log("Chasing Object");
            //lock on the target
            Vector2 direction = target.transform.position - transform.position;
            //calculate distance
            targetDistance = direction.magnitude;
            //follow target ingame
            rb.MovePosition(rb.position + direction.normalized * moveSpeed * Time.fixedDeltaTime);
            //look at target
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
    }

    public void attack() //for state
    {
        Debug.Log("AUUUUUUGGGGHHHH");

        chase();

        if (gunScript.currentClipSize == 0 && gunScript.canReload) //if no ammo, activate timer
        {
            gunScript.canReload = false;
            gunScript.canShoot = false;
            //StartCoroutine(enemyReload());
        }
        else if (gunScript.reloadTimer <= 0) //reload 
        {
            gunScript.canReload = true;
            gunScript.canShoot = true;
            gunScript.reloadTimer = gunScript.reloadSpeed * 2f;
            gunScript.currentClipSize = gunScript.maxClipSize;
        }
        else if(gunScript.currentClipSize > 0) //fire only when clip has bullets
        {
            gunScript.buttonFire();
        }
    }
    /* enemy coroutine reload
    IEnumerator enemyReload()
    {
        float enemyReloadSpeed = gunScript.reloadSpeed * 2;
        Debug.Log("enemy reloading");
        yield return new WaitForSeconds(enemyReloadSpeed);
        gunScript.currentClipSize = gunScript.maxClipSize;
        canReload = true;
        StopCoroutine(enemyReload());
    }
    */
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player" || collision.tag == "Enemy")
        {
            target = collision.gameObject;
        }
    }
}
