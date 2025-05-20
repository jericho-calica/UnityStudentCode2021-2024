using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CountdownManager : MonoBehaviourPunCallbacks
{
    public Text timerText;

    public float timeToStartRace = 5.0f;

    private FiringScript fs;

    // Start is called before the first frame update
    void Start()
    {
        timerText = RacingGameManager.instance.timeText;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (timeToStartRace > 0)
            {
                timeToStartRace -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToStartRace);
            }
            else if (timeToStartRace < 0)
            {
                photonView.RPC("StartRace", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void SetTime(float time)
    {
        if (timerText != null)
        {
            if (time > 0)
            {
                timerText.text = time.ToString("F1");
            }
            else
            {
                timerText.text = "";
            }
        }
    }

    [PunRPC]
    public void StartRace()
    {
        GetComponent<VehicleMovement>().isControlEnabled = true;
        //firing system
        if (GetComponent<FiringRaycast>())
        {
            fs = GetComponent<FiringRaycast>();
        }
        else if (GetComponent<FiringProjectile>())
        {
            fs = GetComponent<FiringProjectile>();
        }
        fs.isControlEnabled = true;

        this.enabled = false;
    }
}
