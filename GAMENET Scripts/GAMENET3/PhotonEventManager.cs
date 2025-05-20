using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class PhotonEventManager : MonoBehaviourPunCallbacks
{
    public enum RaiseEventsCode
    {
        WhoKilledWho = 1,
        WhoIsLastManStanding = 2
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.WhoKilledWho)
        {
            object[] data = (object[])photonEvent.CustomData;
            string killer = (string)data[0];
            string killed = (string)data[1];

            RacingGameManager.instance.DeathMessage(killer, killed);
        }
        else if (photonEvent.Code == (byte)RaiseEventsCode.WhoIsLastManStanding)
        {
            Debug.Log(photonView.Owner.NickName + " AHHHHHHHHHHHHHHHHHHHHHHH");
            object[] data = (object[])photonEvent.CustomData;
            string winner = (string)data[0];

            RacingGameManager.instance.timeText.text = winner + " is the last car standing!";
        }
    }
}
