using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class RacingGameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] vehiclePrefabs;
    public Transform[] startingPositions;
    public GameObject[] finisherTextUi;

    public static RacingGameManager instance = null;

    public Text timeText;
    public TextMeshProUGUI spectatingText; //to give to spectator handler

    public List<GameObject> lapTriggers = new List<GameObject>();

    public GameObject killfeed; //parent
    public GameObject killlistPrefab; //child

    public GameObject pOrganizer;

    GameObject clientCar;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            instance = this;
            //Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                //Debug.Log((int) playerSelectionNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = startingPositions[actorNumber - 1].position;
                clientCar = PhotonNetwork.Instantiate(vehiclePrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
            }
        }

        foreach (GameObject go in finisherTextUi)
        {
            go.SetActive(false);
        }
    }

    public void DeathMessage(string killer, string killed)
    {
        GameObject go = Instantiate(killlistPrefab);
        go.transform.SetParent(killfeed.transform);
        go.transform.localScale = Vector3.one;
        go.GetComponentInChildren<TextMeshProUGUI>().text = killer + " destroyed " + killed;
        Destroy(go, 10.0f);
    }
}
