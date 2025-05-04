using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    void Start()
    {
        GameEvents.current.onPlayerWins += toWinScreen;
        GameEvents.current.onPlayerDies += toDeathScreen;
    }

    private void toWinScreen()
    {
        SceneManager.LoadScene(2);
    }

    private void toDeathScreen()
    {
        StartCoroutine(waitForDeathScreen(3));
    }

    private IEnumerator waitForDeathScreen(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(3);
    }
}
