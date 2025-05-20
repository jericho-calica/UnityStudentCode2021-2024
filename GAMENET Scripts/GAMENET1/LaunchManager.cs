using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject PanelEnterGame;
    [SerializeField] private GameObject PanelConnectionStatus;
    [SerializeField] private GameObject PanelLobby;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        PanelEnterGame.SetActive(true);
        PanelConnectionStatus.SetActive(false);
        PanelLobby.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " Connected to Server");
        PanelLobby.SetActive(true);
        PanelConnectionStatus.SetActive(false);
    }

    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(message);
        CreateAndJoinRoom();
    }

    public void ConnectToPhotonServer() //button
    {
        if(!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connected to Photon Server");
            PhotonNetwork.ConnectUsingSettings();
            PanelConnectionStatus.SetActive(true);
            PanelEnterGame.SetActive(false);
        }
    }

    public void JoinRandomRoom() //button
    {
        PhotonNetwork.JoinRandomRoom();
    }
    
    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room " + Random.Range(0, 1000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20; //0 for unlimited

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " has entered " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " has entered " + PhotonNetwork.CurrentRoom.Name + ". Room has now " + PhotonNetwork.CurrentRoom.PlayerCount + " players.");
    }
}
