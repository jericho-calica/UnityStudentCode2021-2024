using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Status Panel")]
    public Text connectionStatusText;

    [Header("Login UI Panel")]
    public InputField playerNameInput;
    public GameObject loginUIPanel;

    [Header("Game Options Panel")]
    public GameObject gameOptionsPanel;

    [Header("Create Room Panel")]
    public GameObject createRoomPanel;
    public InputField roomNameInputField;
    public InputField playerCountInputField;

    [Header("Join Random Room Panel")]
    public GameObject joinRandomRoomPanel;

    [Header("Show Room List Panel")]
    public GameObject showRoomListPanel;
    public GameObject roomItemPrefab;
    public GameObject roomListParent;

    [Header("Inside Room Panel")]
    public GameObject insideRoomPanel;
    public Text roomInfoText;
    public GameObject playerListItemPrefab;
    public GameObject playerListViewParent;
    public GameObject startGameButton;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListGameObjects;
    private Dictionary<int, GameObject> playerListGameObjects;

    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListGameObjects = new Dictionary<string, GameObject>();
        ActivatePanel(loginUIPanel);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;

        if(PhotonNetwork.MasterClient?.NickName != PhotonNetwork.LocalPlayer?.NickName)
        {
            startGameButton?.SetActive(false);
        }
    }
    #endregion

    #region UI Callbacks
    //buttons
    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;
        if(string.IsNullOrEmpty(playerName))
        {
            Debug.Log("invalid name");
            PhotonNetwork.LocalPlayer.NickName = Random.Range(0, 100000).ToString();
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInputField.text;

        if(string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(playerCountInputField.text);

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(gameOptionsPanel);
    }

    public void OnShowRoomListButtonClicked()
    {
        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        ActivatePanel(showRoomListPanel);
    }

    public void OnBackButtonClicked()
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        ActivatePanel(gameOptionsPanel);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnJoinRandomRoomClicked()
    {
        ActivatePanel(joinRandomRoomPanel);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    #endregion

    #region PUN Callbacks
    public override void OnConnected()
    {
        Debug.Log("connected to internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " connected to photon servers");
        ActivatePanel(gameOptionsPanel);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " created");
        Debug.Log(PhotonNetwork.MasterClient.NickName + " is the host");
        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " connected to " + PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(insideRoomPanel);

        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        if(playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerItem = Instantiate(playerListItemPrefab);
            playerItem.transform.SetParent(playerListViewParent.transform);
            playerItem.transform.localScale = Vector3.one;
            //finds the text to put client's nickname
            playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;
            //if the player in the room is you, it turns on the indicator
            playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

            playerListGameObjects.Add(player.ActorNumber, playerItem);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) 
    {
        //clears any duplicated game objects in the list
        ClearRoomListGameObjects();

        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);
        Debug.Log("is master client: " + PhotonNetwork.LocalPlayer.IsMasterClient);

        //goes through each room in the list to remove or add them to the dictionary
        foreach (RoomInfo x in roomList)
        {
            Debug.Log(x.Name + " room active in list");
            //Debug.Log(PhotonNetwork.MasterClient.NickName + " is the host");
            //remove if room is not open, not public, or full
            if(!x.IsOpen || !x.IsVisible || x.RemovedFromList)
            {
                if(cachedRoomList.ContainsKey(x.Name))
                {
                    cachedRoomList.Remove(x.Name);
                }
            }
            else
            {
                //updates room info
                if(cachedRoomList.ContainsKey(x.Name))
                {
                    cachedRoomList[x.Name] = x;
                }
                else //if room does not exist yet in dictionary then add it
                {
                    cachedRoomList.Add(x.Name, x);
                }
            }
        }

        foreach(RoomInfo y in cachedRoomList.Values)
        {
            GameObject listItem = Instantiate(roomItemPrefab);
            listItem.transform.SetParent(roomListParent.transform);
            listItem.transform.localScale = Vector3.one;

            listItem.transform.Find("RoomNameText").GetComponent<Text>().text = y.Name;
            listItem.transform.Find("RoomPlayersText").GetComponent<Text>().text = "Player count: " + y.PlayerCount + "/" + y.MaxPlayers;
            listItem.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnJoinRoomClicked(y.Name));

            roomListGameObjects.Add(y.Name, listItem);
        }
    }

    public override void OnLeftLobby()
    {
        ClearRoomListGameObjects();
        cachedRoomList.Clear();
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        Debug.Log(PhotonNetwork.MasterClient.NickName + " is the host");

        GameObject playerItem = Instantiate(playerListItemPrefab);
        playerItem.transform.SetParent(playerListViewParent.transform);
        playerItem.transform.localScale = Vector3.one;
        //finds the text to put client's nickname
        playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;
        //if the player in the room is you, it turns on the indicator
        playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);

        playerListGameObjects.Add(player.ActorNumber, playerItem);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //when host leaves, photon makes a new host and the button becomes open to them
        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);

        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        Destroy(playerListGameObjects[otherPlayer.ActorNumber]);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);
    }

    public override void OnLeftRoom()
    {
        //updates each client player view and client leaving room
        foreach(var gameObject in playerListGameObjects.Values)
        {
            Destroy(gameObject);
        }
        playerListGameObjects.Clear();
        playerListGameObjects = null;
        ActivatePanel(gameOptionsPanel);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(message);

        string roomName = "Room " + Random.Range(1000, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    #endregion

    #region Private Methods
    private void OnJoinRoomClicked(string roomName) //adds this button fucntion to instantiated rooms
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(roomName);
    }

    private void ClearRoomListGameObjects()
    {
        foreach(var item in roomListGameObjects.Values)
        {
            Destroy(item);
        }

        roomListGameObjects.Clear();
    }
    #endregion

    #region Public Methods
    public void ActivatePanel(GameObject panelToBeActivated)
    {
        loginUIPanel.SetActive(panelToBeActivated.Equals(loginUIPanel));
        gameOptionsPanel.SetActive(panelToBeActivated.Equals(gameOptionsPanel));
        createRoomPanel.SetActive(panelToBeActivated.Equals(createRoomPanel));
        joinRandomRoomPanel.SetActive(panelToBeActivated.Equals(joinRandomRoomPanel));
        showRoomListPanel.SetActive(panelToBeActivated.Equals(showRoomListPanel));
        insideRoomPanel.SetActive(panelToBeActivated.Equals(insideRoomPanel));
    }
    #endregion

}
