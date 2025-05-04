using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : State
{
    public Chase chase;
    public bool canSeeSomeone = false;
    GameObject Enemy;

    void Start()
    {
        //easier access
        Enemy = transform.parent.transform.parent.gameObject;
    }

    public override State RunCurrentState() //acts as update for ai
    {
        Enemy?.GetComponent<Enemy>().wander();

        if (canSeeSomeone)
        {
            chase.isInAttackRange = false;
            return chase;
        }
        else
        {
            return this;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player" || collision.tag == "Enemy")
        {
            canSeeSomeone = true;
        }
        //no else because it will detect obstacles
    }
}
