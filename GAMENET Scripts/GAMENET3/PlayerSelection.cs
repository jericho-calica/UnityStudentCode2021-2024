using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSelection : MonoBehaviour
{
    public GameObject[] SelectablePlayers;
    public int playerSelectionNumber;

    void Start()
    {
        playerSelectionNumber = 0;

        ActivePlayer(playerSelectionNumber);
    }

    void Update()
    {
        
    }

    private void ActivePlayer(int x)
    {
        foreach(GameObject go in SelectablePlayers)
        {
            go.SetActive(false);
        }

        SelectablePlayers[x].SetActive(true);

        //setting player selection
        ExitGames.Client.Photon.Hashtable playerSelectionProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_SELECTION_NUMBER, playerSelectionNumber} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProperties);
    }

    public void goToNextPlayer()
    {
        playerSelectionNumber++;

        if(playerSelectionNumber >= SelectablePlayers.Length)
        {
            playerSelectionNumber = 0;
        }

        ActivePlayer(playerSelectionNumber);
    }

    public void goToPrevPlayer()
    {
        playerSelectionNumber--;

        if (playerSelectionNumber < 0)
        {
            playerSelectionNumber = SelectablePlayers.Length - 1;
        }

        ActivePlayer(playerSelectionNumber);
    }
}
