using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSelection : MonoBehaviour
{
    int playerSelectionNumber;

    private void Start()
    {
        PerkSelected(0); //sets first perk as default
        ColorSelected(0);
    }

    public void PerkSelected(int value)
    {
        //set constant
        ExitGames.Client.Photon.Hashtable playerSelectionProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_SELECTION_PERK, value } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProperties);
    }
    
    public void ColorSelected(int value)
    {
        ExitGames.Client.Photon.Hashtable playerSelectionProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_SELECTION_COLOR, value } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProperties);
    }
    
}
