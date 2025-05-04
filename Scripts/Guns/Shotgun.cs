using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    public override void Start()
    {
        maxClipSize = 2;
        damage = 20;
        fireRate = 1.0f;
        reloadSpeed = 2.0f;
        spread = 7.0f;
        isSemiAuto = true;
        base.Start();
    }
    public override void buttonFire()
    {
        if (currentClipSize > 0 && canShoot && fireTimer <= 0)
        {
            fireTimer = fireRate;
            canShoot = false;
            //one shell, 8 pellets
            for (int i = 0; i < 8; i++)
            {
                GameObject bullet = Instantiate(
                bulletPrefab,
                firePoint.position,
                firePoint.rotation);
                bullet.GetComponent<Bullet>().setDamage(this.damage);
                Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
                bulletRB.AddForce(
                    firePoint.up * 20f,
                    ForceMode2D.Impulse);
                //adds spread to bullets
                bulletRB.AddForce(
                    new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0),
                    ForceMode2D.Impulse);
            }
            currentClipSize--;
            Audio.instance.PlaySound(sfxenum.shotshotgun);
        }
    }

    public override void buttonReload()
    {
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
            while (currentClipSize < maxClipSize) //while clip has space
            {
                if (transform.parent.GetComponent<Player>().inventory.shotgunAmmo.ammoOnHand <= 0) //if ammo runs out then break
                {
                    break;
                }
                currentClipSize++;
                transform.parent.GetComponent<Player>().inventory.shotgunAmmo.ammoOnHand--;
            }
            Audio.instance.PlaySound(sfxenum.getshotgun);
        }
    }

    /*
    public override IEnumerator fire()
    {
        if (currentClipSize > 0 && canShoot)
        {
            canShoot = false;
            //one shell, 8 pellets
            for(int i = 0; i < 8; i++)
            {
                GameObject bullet = Instantiate(
                bulletPrefab,
                firePoint.position,
                firePoint.rotation,
                transform);
                Rigidbody2D bulletRB = bullet.GetComponent<Rigidbody2D>();
                bulletRB.AddForce(
                    firePoint.up * 20f,
                    ForceMode2D.Impulse);
                //adds spread to bullets
                bulletRB.AddForce(
                    new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0),
                    ForceMode2D.Impulse);
            }
            currentClipSize--;
            yield return new WaitForSeconds(fireRate);
            canShoot = true;
        }
        StopCoroutine(fire());
    }
    */
    /*
    public override IEnumerator reload()
    {
        canShoot = false;
        yield return new WaitForSeconds(reloadSpeed);
        while (currentClipSize < maxClipSize) //while clip has space
        {
            if (transform.parent.GetComponent<Player>().inventory.shotgunAmmo <= 0) //if ammo runs out then break
            {
                break;
            }
            currentClipSize++;
            transform.parent.GetComponent<Player>().inventory.shotgunAmmo--;
        }
        canShoot = true;
        StopCoroutine(reload());
    }
    */
}
