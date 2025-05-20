using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviour
{
    private float damage = 0;

    private void Start()
    {
        Destroy(gameObject, 10f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            other.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, damage);
        }
        if(other.tag != "Bullet")
        {
            Destroy(gameObject);
        }
    }

    public void setDamage(float dmg)
    {
        this.damage = dmg;
    }
}
