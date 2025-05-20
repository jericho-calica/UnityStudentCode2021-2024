using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera camera;
    public Camera firingCamera;
    public GameObject weapon;
    public float startHealth;
    public float health;
    public GameObject canvas;
    public Image healthBar;
    [SerializeField] private TextMeshProUGUI playerNameUi;
    public FiringScript fs;
    public bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        startHealth = 1000;
        health = startHealth;
        healthBar.fillAmount = health / startHealth;

        //get firing script
        if (GetComponent<FiringRaycast>())
        {
            fs = GetComponent<FiringRaycast>();
        }
        else if(GetComponent<FiringProjectile>())
        {
            fs = GetComponent<FiringProjectile>();
        }

        this.camera = transform.Find("Camera").GetComponent<Camera>();
        this.firingCamera = transform.Find("firingcamera").GetComponent<Camera>();
        this.canvas = transform.Find("Canvas").gameObject;
        this.playerNameUi = canvas.transform.Find("nameText").GetComponent<TextMeshProUGUI>();
        playerNameUi.text = photonView.Owner.NickName;
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponent<LapController>().enabled = photonView.IsMine;
            GetComponent<PhotonEventManager>().enabled = photonView.IsMine;
            fs.enabled = false;
            camera.enabled = photonView.IsMine;
            firingCamera.enabled = false;
            weapon.SetActive(false);

            if (firingCamera != null)
            {
                firingCamera.gameObject.SetActive(false);
            }
        }
        else if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponent<LapController>().enabled = photonView.IsMine;
            GetComponent<PhotonEventManager>().enabled = photonView.IsMine;
            fs.enabled = photonView.IsMine;
            camera.enabled = photonView.IsMine;

            if (firingCamera != null)
            {
                firingCamera.enabled = false;
            }

            this.transform.SetParent(RacingGameManager.instance.pOrganizer.transform);
            gameObject.name = photonView.Owner.NickName;
        }
    }

    private void Update()
    {
        Debug.Log(photonView.Owner.NickName + " is dead: " + isDead);
        //Debug.Log(photonView.Owner.NickName + " is checking the child count: " +  RacingGameManager.instance.pOrganizer.transform.childCount);

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            if (RacingGameManager.instance.pOrganizer.transform.childCount == 1)
            {
                //who is last one event data
                object[] data = new object[] { RacingGameManager.instance.pOrganizer.transform.GetChild(0).name };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.All,
                    CachingOption = EventCaching.AddToRoomCache
                };

                SendOptions sendOption = new SendOptions
                {
                    Reliability = false
                };
                PhotonNetwork.RaiseEvent((byte)PhotonEventManager.RaiseEventsCode.WhoIsLastManStanding, data, raiseEventOptions, sendOption);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(float damage, PhotonMessageInfo info)
    {
        this.health -= damage;
        this.healthBar.fillAmount = health / startHealth;

        if (health <= 0 && !isDead)
        {
            isDead = true;
            transform.parent = null; //not alive anymore 
            Die();
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);

            //killfeed event data
            object[] data = new object[] { info.Sender.NickName, info.photonView.Owner.NickName };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOption = new SendOptions
            {
                Reliability = false
            };
            PhotonNetwork.RaiseEvent((byte)PhotonEventManager.RaiseEventsCode.WhoKilledWho, data, raiseEventOptions, sendOption);
            //RacingGameManager.instance.DeathMessage(info.Sender.NickName, info.photonView.Owner.NickName);
        }
    }

    private void Die()
    {
        GetComponent<VehicleMovement>().enabled = false;
        GetComponent<LapController>().enabled = false;
        GetComponent<FiringScript>().enabled = false;
        this.gameObject.AddComponent(typeof(SpectatorHandler));
        GetComponent<SpectatorHandler>().enabled = photonView.IsMine;
    }
}
