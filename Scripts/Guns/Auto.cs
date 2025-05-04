using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auto : Gun
{
    public override void Start()
    {
        maxClipSize = 30;
        damage = 15;
        fireRate = 0.07f;
        reloadSpeed = 2.5f;
        spread = 1.5f;
        isSemiAuto = false;
        base.Start();
    }

    public override void buttonFire()
    {
        if (currentClipSize > 0 && canShoot && fireTimer <= 0)
        {
            base.buttonFire();
            Audio.instance.PlaySound(sfxenum.shotauto);
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
                if (transform.parent.GetComponent<Player>().inventory.autoAmmo.ammoOnHand <= 0) //if ammo runs out then break
                {
                    break;
                }
                currentClipSize++;
                transform.parent.GetComponent<Player>().inventory.autoAmmo.ammoOnHand--;
            }
            Audio.instance.PlaySound(sfxenum.getshotgun);
        }
    }

    /*
    public override IEnumerator reload()
    {
        canShoot = false;
        yield return new WaitForSeconds(reloadSpeed);
        while (currentClipSize < maxClipSize) //while clip has space
        {
            if (transform.parent.GetComponent<Player>().inventory.autoAmmo <= 0) //if ammo runs out then break
            {
                break;
            }
            currentClipSize++;
            transform.parent.GetComponent<Player>().inventory.autoAmmo--;
        }
        canShoot = true;
        StopCoroutine(reload());
    }
    */
}
