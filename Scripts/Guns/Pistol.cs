using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun
{
    public override void Start()
    {
        maxClipSize = 15;
        damage = 10;
        fireRate = 0.5f;
        reloadSpeed = 1.0f;
        spread = 0.0f;
        isSemiAuto = true;
        base.Start();
    }

    public override void buttonFire()
    {
        if (currentClipSize > 0 && canShoot && fireTimer <= 0)
        {
            base.buttonFire();
            Audio.instance.PlaySound(sfxenum.shotpistol);
        }
    }

    public override void buttonReload()
    {
        if (canReload) //countdown starts when can reload is false
        {
            canReload = false;
            canShoot = false;
        }
        else if(reloadTimer <= 0)
        {
            canReload = true;
            canShoot = true;
            reloadTimer = reloadSpeed;
            while (currentClipSize < maxClipSize) //while clip has space
            {
                if (transform.parent.GetComponent<Player>().inventory.pistolAmmo.ammoOnHand <= 0) //if ammo runs out then break
                {
                    break;
                }
                currentClipSize++;
                transform.parent.GetComponent<Player>().inventory.pistolAmmo.ammoOnHand--;
            }
            Audio.instance.PlaySound(sfxenum.getpistol);
        }
    }

    /*
    public override IEnumerator reload()
    {
        canShoot = false;
        yield return new WaitForSeconds(reloadSpeed);
        while (currentClipSize < maxClipSize) //while clip has space
        {
            if (transform.parent.GetComponent<Player>().inventory.pistolAmmo <= 0) //if ammo runs out then break
            {
                break;
            }
            currentClipSize++;
            transform.parent.GetComponent<Player>().inventory.pistolAmmo--;
        }
        canShoot = true;
        StopCoroutine(reload());
    }
    */
}
