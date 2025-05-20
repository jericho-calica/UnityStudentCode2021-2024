using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerCommander playerCommanderScript;

    void Start()
    {
        this.gameObject.name = photonView.Owner.NickName; //labels node to player's nickname
        this.transform.SetParent(GameManager.instance.getPlayerOrganizer().transform); //becomes child of playerOrganizer

        //custom properties
        object playerSelectionNumber;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_PERK, out playerSelectionNumber))
        {
            Debug.Log("perk number: " + (int)playerSelectionNumber); //give perk
            switch ((int)playerSelectionNumber)
            {
                case 0: //faster ticks
                    playerCommanderScript.setSpeed(1f);
                    break;
                case 1: //faster production
                    playerCommanderScript.setProduction(250);
                    break;
                case 2: //lethal ticks
                    playerCommanderScript.setDamage(2);
                    break;
            }
        }

        playerCommanderScript.getMyPhotonViewComponent().RPC("TakeProvince", RpcTarget.AllBuffered, 
            PhotonNetwork.LocalPlayer.ActorNumber * 2); //takes a province as a starting capital
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //makes sure map does not despawn
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).transform.SetParent(GameManager.instance.getMap().transform);
        }
    }
}
