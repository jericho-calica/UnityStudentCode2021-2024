using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void GoToGame(int _whichGame)
    {
        SceneManager.LoadScene(_whichGame);
    }
}
