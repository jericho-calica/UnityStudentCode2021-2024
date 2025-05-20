using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FiringProjectile : FiringScript
{
    public GameObject firepoint;
    public GameObject bulletPrefab;
    public Camera mainCamera;
    public Camera zoomCamera;
    private float bulletSpeed = 100f;
    private int spread = 50;
    private int firerate = 20;
    private int firerateTimer;

    // Start is called before the first frame update
    void Start()
    {
        firerateTimer = firerate;
    }

    // Update is called once per frame
    void Update()
    {
        if (isControlEnabled)
        {
            if (Input.GetMouseButton(0) && firerateTimer == 0)
            {
                Fire();
            }
            if (firerateTimer > 0) //custom timer
            {
                firerateTimer--;
            }
            //camera views
            if (Input.GetMouseButton(1))
            {
                zoomCamera.enabled = true;
                mainCamera.enabled = false;
            }
            else
            {
                mainCamera.enabled = true;
                zoomCamera.enabled = false;
            }
        }
    }

    private void Fire()
    {
        //create
        GameObject tb = PhotonNetwork.Instantiate(bulletPrefab.name, firepoint.transform.position, firepoint.transform.rotation);
        //set damage
        tb.GetComponent<Bullet>().setDamage(20f);
        //get rb
        Rigidbody tbrb = tb.GetComponent<Rigidbody>();
        //add force
        tbrb.AddForce(firepoint.transform.forward * bulletSpeed, ForceMode.Impulse);
        //spread
        tbrb.AddForce(new Vector3(
            UnityEngine.Random.Range(-spread, spread), 
            UnityEngine.Random.Range(-spread, spread/2), 
            UnityEngine.Random.Range(-spread, spread)));
        //reset timer
        firerateTimer = firerate;
    }
}
