using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerCommander : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView myPhotonView;

    [SerializeField] GameObject TickPrefab;

    float ownerTickSpeed = 0.5f;
    int ownerProduction = 400;
    int ownerTickDamage = 1;

    [SerializeField] SpriteRenderer ownerColor;

    private void Start()
    {

    }

    [PunRPC]
    public void MarchTicks(int senderIndex, int receiverIndex, int senderTickDamage, float senderTickSpeed)
    {
        //sender reference
        ProvinceNode senderScript = GameManager.instance.getProvincesScripts()[senderIndex];
        //receiver reference
        GameObject receiverObject = GameManager.instance.getProvincesObject()[receiverIndex].transform.GetChild(0).gameObject;
        //spawn in random space within province circle
        GameObject tempTick = Instantiate(TickPrefab, 
            new Vector2(
                senderScript.getOriginalPos().x + UnityEngine.Random.Range(-0.1f, 0.1f),
                senderScript.getOriginalPos().y + UnityEngine.Random.Range(-0.1f, 0.1f)), 
            Quaternion.identity);
        tempTick.GetComponent<Tick>().Initialize(getOwnerNickname(), senderIndex, receiverObject.transform.position, this, senderTickDamage, senderTickSpeed);
        senderScript.RemoveTick();
    }

    [PunRPC]
    public void TakeProvince(int takenProvinceIndex)
    {
        GameManager.instance.getProvincesScripts()[takenProvinceIndex].setNewOwner(this, getIsMyPhotonView(), ownerProduction);
    }

    //stats
    public float getSpeed()
    {
        return ownerTickSpeed;
    }
    public void setSpeed(float newSpeed)
    {
        ownerTickSpeed = newSpeed;
    }

    public int getProduction()
    {
        return ownerProduction;
    }

    public void setProduction(int newProduction)
    {
        this.ownerProduction = newProduction;
    }

    public int getDamage()
    {
        return ownerTickDamage;
    }
    
    public void setDamage(int newDamage)
    {
        ownerTickDamage = newDamage;
    }

    public SpriteRenderer getSpriteRenderer()
    {
        return ownerColor;
    }

    //photon view
    public string getOwnerNickname()
    {
        return photonView.Owner.NickName;
    }

    public bool getIsMyPhotonView()
    {
        return photonView.IsMine;
    }

    public PhotonView getMyPhotonViewComponent()
    {
        return myPhotonView;
    }
}
