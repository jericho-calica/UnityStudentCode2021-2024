using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage = 0;

    void Update()
    {
        Destroy(gameObject, 2.0f);
    }

    public void setDamage(int damage)
    {
        this.damage = damage;
    }

    //destroy bullet on contact with an object with a collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //if player is hit, make them take damage according to the damage of the gun
            collision.gameObject.GetComponent<Player>().takeDamage(damage);
        }

        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().takeDamage(damage);
        }

        if (collision.gameObject.tag != "Bullet") //if not bullet, destroy itself
        {
            Destroy(gameObject);
        }
    }
}
