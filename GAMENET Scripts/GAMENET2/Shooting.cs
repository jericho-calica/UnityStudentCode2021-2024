using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using TMPro;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera camera;
    public GameObject hitEffectPrefab;

    [Header("HP Related Variables")]
    public float startHealth = 100;
    [SerializeField] private float health;
    public Image healthBar;

    public bool isDead = false;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire() //for button
    {
        if(!isDead) //can shoot if not dead
        {
            RaycastHit hit;
            Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            if (Physics.Raycast(ray, out hit, 200))
            {
                Debug.Log(hit.collider.gameObject.name);

                photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);

                if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);
                }
            }
        }
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        this.health -= damage;
        this.healthBar.fillAmount = health / startHealth;

        if(health <= 0 && !isDead)
        {
            isDead = true; //immediately locks this function
            Die();
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
            GameManager.instance.addScore(info.Sender.NickName, info.photonView.Owner.NickName);
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.2f);
    }

    public void Die()
    {
       if(photonView.IsMine)
        {
            animator.SetBool("isDead", true);
            StartCoroutine(RespawnCountdown());
        }
    }

    IEnumerator RespawnCountdown()
    {
        GameObject respawnText = GameObject.Find("Respawn Text");
        TextMeshProUGUI respawnTMP = respawnText.GetComponent<TextMeshProUGUI>();

        float respawnTime = 5.0f;
        while(respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawnTMP.text = "You are Killed. Respawning in " + respawnTime.ToString(".00");
        }

        animator.SetBool("isDead", false);
        respawnTMP.text = "";

        //int randomPointX = UnityEngine.Random.Range(-20, 20);
        //int randomPointZ = UnityEngine.Random.Range(-20, 20);
        //this.transform.position = new Vector3(randomPointX, 0, randomPointZ);

        Transform pointToRespawn = GameManager.instance.getSpawnPoint();
        transform.position = new Vector3(pointToRespawn.position.x, pointToRespawn.position.y, pointToRespawn.position.z);
        transform.GetComponent<PlayerMovementController>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth()
    {
        isDead = false;
        health = 100;
        healthBar.fillAmount = health / startHealth;
    }
}
