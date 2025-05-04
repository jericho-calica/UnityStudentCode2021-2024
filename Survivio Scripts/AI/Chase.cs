using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : State
{
    public Attack attack;
    public bool isInAttackRange = false;
    GameObject Enemy;

    void Start()
    {
        Enemy = transform.parent.transform.parent.gameObject;
        StopCoroutine(Enemy.GetComponent<Enemy>().randomWalk());
    }

    public override State RunCurrentState()
    {
        Enemy.GetComponent<Enemy>().chase();

        if (isInAttackRange)
        {
            attack.isTooFar = false;
            return attack;
        }
        else
        {
            return this;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Enemy")
        {
            isInAttackRange = true;
        }
    }
}
