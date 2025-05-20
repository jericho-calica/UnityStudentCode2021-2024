using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LeaveGameButton : MonoBehaviourPunCallbacks
{
    public void OnLeaveButtonClicked()
    {
        if (PhotonNetwork.InRoom) //if in room and is client that wants to leave
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
