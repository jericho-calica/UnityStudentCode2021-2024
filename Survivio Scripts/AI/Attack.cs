using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : State
{
    public Wander wander;
    public bool isTooFar = true;
    GameObject Enemy;

    void Start()
    {
        Enemy = transform.parent.transform.parent.gameObject;
    }

    public override State RunCurrentState()
    {
        Enemy.GetComponent<Enemy>().attack();

        if (Enemy.GetComponent<Enemy>().targetDistance > 10)
        {
            isTooFar = true;
        }
            

        if (Enemy.GetComponent<Enemy>().target == null || isTooFar) //go back to wonder if too far or dead
        {
            wander.canSeeSomeone = false;
            return wander;
        }
        else
        {
            return this;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Enemy")
        {
            isTooFar = false;
        }
    }
}
