using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpectatorHandler : MonoBehaviourPunCallbacks
{
    GameObject PlayerParent;
    TextMeshProUGUI st;
    GameObject currentCar;
    Camera currentCam;
    void Start()
    {
        //will not change
        PlayerParent = RacingGameManager.instance.pOrganizer;
        st = RacingGameManager.instance.spectatingText;
        setUp();
    }


    void Update()
    {
        if (PlayerParent.transform.childCount == 0 || currentCar== null || currentCam == null) //if no players or dead
        {
            GameObject.Find("Camera").GetComponent<Camera>().enabled = true;
            this.enabled = false;
        }
        if (currentCar.GetComponent<PlayerSetup>().isDead) //spectated player died
        {
            currentCam.enabled = false;
            setUp();
        }
        if (PlayerParent.transform.childCount > 1) //switch
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentCam.enabled = false;
                setUp(0);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                currentCam.enabled = false;
                setUp(1);
            }
        }
    }

    void setUp(int change = 0)
    {
        //may change
        currentCar = PlayerParent.transform.GetChild(change).gameObject;
        currentCam = currentCar.transform.Find("Camera").GetComponent<Camera>();
        //initialization
        st.text = PlayerParent.transform.GetChild(change).gameObject.name;
        currentCam.enabled = true;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PlayerParent.transform.childCount <= 0) //no more players
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
