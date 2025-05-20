using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Tick : MonoBehaviour
{
    [SerializeField] string ownerPlayer;
    [SerializeField] int ownerProvinceIndex;
    [SerializeField] PlayerCommander ownerPlayerScript;
    [SerializeField] int damage = 1;
    [SerializeField] float moveSpeed = 0.5f;
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] CircleCollider2D cc2d;
    [SerializeField] SpriteRenderer sr;
    Vector2 target;

    private void Update()
    {
        if (target != null)
        {
            Vector2 direction = target - (Vector2)transform.position;
            rb2d.MovePosition(rb2d.position + direction.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    [PunRPC]
    public void Initialize(string newOwner, int startingProvince, Vector2 location, PlayerCommander newOwnerScript, int newDamage, float newMovespeed)
    {
        setOwnerPlayer(newOwner);
        setOwnerPlayerScript(newOwnerScript);
        setOwnerProvinceIndex(startingProvince);
        setTarget(location);
        setDamage(newDamage);
        setSpeed(newMovespeed);
        cc2d.enabled = true;
        this.gameObject.name = ownerPlayer + "'s tick";
        sr.color = ownerPlayerScript.getSpriteRenderer().color;
    }

    public string getOwnerPlayer()
    {
        return ownerPlayer;
    }

    public void setOwnerPlayer(string newOwner)
    {
        this.ownerPlayer = newOwner;
    }

    public PlayerCommander getOwnerPlayerScript()
    {
        return ownerPlayerScript;
    }

    public void setOwnerPlayerScript(PlayerCommander newOwnerScript)
    {
        this.ownerPlayerScript = newOwnerScript;
    }

    public int getOwnerProvinceIndex()
    {
        return ownerProvinceIndex;
    }

    public void setOwnerProvinceIndex(int newOwnerProvince)
    {
        this.ownerProvinceIndex = newOwnerProvince;
    }

    public int getDamage()
    {
        return damage;
    }

    public void setDamage(int damage)
    {
        this.damage = damage;
    }

    public float getSpeed()
    {
        return moveSpeed;
    }

    public void setSpeed(float num)
    {
        this.moveSpeed = num;
    }

    public void setTarget(Vector2 location)
    {
        this.target = location;
    }
}
