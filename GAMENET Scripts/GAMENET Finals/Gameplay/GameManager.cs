using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerOrganizer;
    [SerializeField] GameObject map;
    [SerializeField] List<GameObject> provincesObject = new List<GameObject>();
    [SerializeField] List<ProvinceNode> provincesScripts = new List<ProvinceNode>();
    [SerializeField] List<GameObject> playerNodePrefabs = new List<GameObject>();
    GameObject clientNode;


    [SerializeField] Camera camera;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionColor;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_COLOR, out playerSelectionColor)) //make player
            {
                Debug.Log("color number: " + (int)playerSelectionColor); //give color

                clientNode = PhotonNetwork.Instantiate(playerNodePrefabs[(int)playerSelectionColor].name, this.transform.position, Quaternion.identity);
            }

            SpawnPlayersAndParentProvinces();

            //provincesObject[0].transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>().color = Random.ColorHSV();
        }
    }

    void Update()
    {
        
    }

    void SpawnPlayersAndParentProvinces()
    {
        for (int i = 0; i < map.transform.childCount; i++) //adds provinces to list for debugging
        {
            provincesObject.Add(map.transform.GetChild(i).gameObject);
            provincesScripts.Add(map.transform.GetChild(i).GetChild(0).GetComponent<ProvinceNode>());
            map.transform.GetChild(i).GetChild(0).GetComponent<ProvinceNode>().setListIndex(i);
        }

        playerOrganizer.transform.SetParent(map.transform);

        int rng = UnityEngine.Random.Range(0, provincesObject.Count);
        Debug.Log("rng: " + rng);
        //if selected province is not null (no owner), repeat until an independent province is selected
        /*
        while (provinces[rng].GetComponentInChildren<ProvinceNode>().getOwnerPlayer() != null)
        {
            rng = UnityEngine.Random.Range(0, provinces.Count);
        }
        //take independent province
        clientNode.GetComponent<PhotonView>().RPC("TakeProvince", RpcTarget.AllBuffered, rng);
        */
    }

    public GameObject getPlayerOrganizer()
    {
        return playerOrganizer;
    }

    public GameObject getMap()
    {
        return map;
    }

    public List<GameObject> getProvincesObject()
    {
        return provincesObject;
    }

    public List<ProvinceNode> getProvincesScripts()
    {
        return provincesScripts;
    }
}
