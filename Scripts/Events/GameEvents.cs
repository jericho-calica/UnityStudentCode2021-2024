using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake() //singleton
    {
        current = this;
    }

    public event Action onPlayerWins;
    public event Action onPlayerDies;
    public event Action onEnemyDeath;

    //check if null first before doing action
    public void PlayerWins()
    {
        if (onPlayerWins != null)
        {
            onPlayerWins();
        }
    }

    public void PlayerDies()
    {
        if (onPlayerDies != null)
        {
            onPlayerDies();
        }
    }

    public void EnemyDeath()
    {
        if (onEnemyDeath != null)
        {
            onEnemyDeath();
        }
    }
}
