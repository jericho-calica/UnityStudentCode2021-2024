using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class PlayerScore
    {
        public string pName;
        public int pKills;

        public void increaseScore(int x)
        {
            this.pKills += x;
        }
    }

    public GameObject playerPrefab;
    public List<Transform> respawnPoints;
    public List<PlayerScore> ScoreBoard;

    public GameObject playerUI;
    public GameObject parentKillFeed;
    public GameObject childKillFeed;

    private int killsToWin = 10;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            respawnPoints.Add(transform.GetChild(i));
        }

        if (PhotonNetwork.IsConnectedAndReady)
        {
            //int randomPointX = Random.Range(-10, 10);
            //int randomPointZ = Random.Range(-10, 10);
            Transform pointToRespawn = getSpawnPoint();
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(pointToRespawn.position.x, pointToRespawn.position.y, pointToRespawn.position.z), Quaternion.identity);
        }
    }

    
    void Update()
    {
        DebugPrintScoreBoard();
        CheckForWinner();
    }

    public Transform getSpawnPoint()
    {
        return respawnPoints[Random.Range(0, respawnPoints.Count)];
        //return respawnPoints[0];
    }

    public void addScore(string killer, string killed)
    {
        GameObject x = Instantiate(childKillFeed);
        x.transform.SetParent(parentKillFeed.transform);
        x.transform.localScale = Vector3.one;
        x.GetComponentInChildren<TextMeshProUGUI>().text = killer + " shot " + killed;
        Destroy(x, 5.0f);

        for(int i = 0; i < ScoreBoard.Count; i++)
        {
            if (ScoreBoard[i].pName == killer) //if player is already on the scoreboard, add score
            {
                ScoreBoard[i].increaseScore(1);
                Debug.Log("early return index: " + i);
                return;
            }
        }

        //add player to scoreboard with default values if function did not return early
        Debug.Log("made new score");
        PlayerScore temp = new PlayerScore
        {
            pName = killer,
            pKills = 1
        };
        ScoreBoard.Add(temp);
    }

    private void DebugPrintScoreBoard()
    {
        for (int i = 0; i < ScoreBoard.Count; i++)
        {
            Debug.Log(ScoreBoard[i].pName + " has a score of " + ScoreBoard[i].pKills);
        }
    }

    private void CheckForWinner()
    {
        foreach (PlayerScore x in ScoreBoard)
        {
            if (x.pKills >= killsToWin) //winner checker
            {
                Debug.Log(x.pName + " wins!!!");
                playerUI.transform.Find("Winner Text").GetComponent<TextMeshProUGUI>().text = x.pName + " WINS";
                StartCoroutine(SomeoneWon());
            }
        }
    }

    public void setPlayerUI(GameObject ui)
    {
        playerUI = ui;
        parentKillFeed = ui.transform.Find("KillFeed").gameObject;
    }

    IEnumerator SomeoneWon()
    {
        yield return new WaitForSeconds(10.0f);
        if (PhotonNetwork.InRoom) //leave when still only in room
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
