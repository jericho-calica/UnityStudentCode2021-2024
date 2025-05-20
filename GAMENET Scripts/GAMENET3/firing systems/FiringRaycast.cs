using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FiringRaycast : FiringScript
{
    public Camera raycamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isControlEnabled)
        {
            if (Input.GetMouseButton(0))
            {
                Fire();
            }
            if (raycamera != null)
            {
                Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
                Debug.DrawRay(raycamera.gameObject.transform.position, forward, Color.red);
            }
        }
    }

    public void Fire() //copy paste from module 2
    {
        RaycastHit hit;
        Ray ray = raycamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 1f);
            }
        }
    }
}
