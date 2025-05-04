using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [SerializeField] public Transform firePoint;
    [SerializeField] public GameObject bulletPrefab;

    [SerializeField] public GameObject itself;

    public int currentClipSize = 0;
    public int maxClipSize;
    public int damage;
    public float fireRate;
    public float reloadSpeed;
    public float spread;
    public bool isSemiAuto = true;
    public bool canShoot = true;
    public float fireTimer;
    public bool canReload = true;
    public float reloadTimer;

    public virtual void Start()
    {
        fireTimer = fireRate;
        reloadTimer = reloadSpeed;
    }

    private void Update()
    {
        //timer for fire rate
        if (fireTimer > 0)
        {
            fireTimer -= 1 * Time.deltaTime;
            canShoot = true;
        }

        if (reloadTimer > 0 && !canReload)
        {
            reloadTimer -= 1 * Time.deltaTime;
        }
        else if(reloadTimer <= 0 && transform.parent.tag != "Enemy")
        {
            buttonReload(); //reload again to reapply magazine
        }

        //editorControls();
    }

    public virtual void buttonFire()
    {
        //StartCoroutine(fire());

        canShoot = false;
        fireTimer = fireRate;
        //makes bullets
        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation);
        //apply damage to bullet
        bullet.GetComponent<Bullet>().setDamage(this.damage);
        //find bullet rigidbody and make shoot them where the gun is pointing at
        Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
        bulletRB.AddForce(
            firePoint.up * 20f,
            ForceMode2D.Impulse);
        //adds spread to bullets
        bulletRB.AddForce(
            new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0),
            ForceMode2D.Impulse);
        currentClipSize--;
    }

    public virtual void buttonReload()
    {
        //StartCoroutine(reload());
        if (canReload) //countdown starts when can reload is false
        {
            canReload = false;
            canShoot = false;
        }
        else if (reloadTimer <= 0)
        {
            canReload = true;
            canShoot = true;
            reloadTimer = reloadSpeed;
            currentClipSize = maxClipSize;
        }
    }
    /* coroutine fire
    public virtual IEnumerator fire()
    {
        if (currentClipSize > 0 && canShoot)
        {
            canShoot = false;
            //makes bullets and make them children of the gun that shot them
            GameObject bullet = Instantiate(
                bulletPrefab,
                firePoint.position,
                firePoint.rotation,
                transform);
            //find bullet rigidbody and make shoot them where the gun is pointing at
            Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
            bulletRB.AddForce(
                firePoint.up * 20f,
                ForceMode2D.Impulse);
            //adds spread to bullets
            bulletRB.AddForce(
                new Vector3(0, Random.Range(-spread, spread), 0),
                ForceMode2D.Impulse);
            currentClipSize--;
            yield return new WaitForSeconds(fireRate);
            canShoot = true;
        }
        StopCoroutine(fire());
    }
    */
    /* coroutine reload
    public virtual IEnumerator reload()
    {
        //does not use player's ammo inventory
        currentClipSize = maxClipSize;
        yield return new WaitForSeconds(0f);
        StopCoroutine(reload());
    }
    */
    public void editorControls()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("bang");
            buttonFire();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            buttonReload();
        }
    }
}
