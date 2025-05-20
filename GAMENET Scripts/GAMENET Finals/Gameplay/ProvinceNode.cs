using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class ProvinceNode : MonoBehaviour
{
    [Header("Owner Info")]
    [SerializeField] string ownerPlayerString;
    [SerializeField] PlayerCommander ownerPlayerScript;

    [Header("Province Info")]
    [SerializeField] int MyGameManagerListIndex;
    [SerializeField] Vector2 originalPos;

    [Header("Tick Info")]
    [SerializeField] int tickCount = 20;
    [SerializeField] int maxTickCount = 20;

    [Header("Timers")]
    [SerializeField] int tickProductionTimer = 400;
    [SerializeField] int currentTickProductionTimer = 400;
    [SerializeField] int tickSendTimer = 50;
    [SerializeField] int currentTickSendTimer = 50;

    [Header("Child Component References")]
    [SerializeField] GameObject childDraggedObject;
    [SerializeField] SpriteRenderer childDraggedSpriteRenderer;
    [SerializeField] GameObject ownerIndicator;
    [SerializeField] TextMeshProUGUI childDraggedTickCountText;

    [Header("Selection")]
    [SerializeField] bool childSelected = false;
    [SerializeField] bool mouseOnMe = false;

    private void Start()
    {
        originalPos = transform.position;
    }

    private void Update()
    {
        PlayerControls();
        DisplayTicks();
        ProduceMoreTicksUntilMax();
        TimerCounter();
    }

    void PlayerControls()
    {
        if (Input.GetMouseButtonDown(0) && mouseOnMe) //if clicked then child will follow cursor
        {
            childSelected = true;
        }
        else if (Input.GetMouseButtonDown(1) && childSelected) //deselect
        {
            childSelected = false;
            childDraggedObject.transform.position = originalPos;
        }

        if (childSelected)
        {
            //makes sure the child is visible
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            childDraggedObject.transform.position = mousePos;
        }
    }

    void DisplayTicks()
    {
        childDraggedTickCountText.text = tickCount.ToString();
    }

    void ProduceMoreTicksUntilMax()
    {
        if (tickCount < maxTickCount && currentTickProductionTimer == 0)
        {
            currentTickProductionTimer = tickProductionTimer; //reset timer
            tickCount++;
        }
    }

    void TimerCounter() //to count down current timers
    {
        if (currentTickProductionTimer != 0)
        {
            currentTickProductionTimer--;
        }
        
        if (currentTickSendTimer != 0)
        {
            currentTickSendTimer--;
        }
    }

    public void WantToMoveTo(ProvinceNode province)
    {
        if (tickCount > 1 && currentTickSendTimer == 0)
        {
            currentTickSendTimer = tickSendTimer; //timer so ticks dont just laser for the target
            currentTickProductionTimer = tickProductionTimer; //makes sure home province cant keep sending ticks to a province for cheese
            ownerPlayerScript.getMyPhotonViewComponent().RPC("MarchTicks", RpcTarget.AllBuffered, 
                MyGameManagerListIndex, 
                province.getListIndex(), 
                ownerPlayerScript.getDamage(), 
                ownerPlayerScript.getSpeed());
        }
    }

    public void RemoveTick()
    {
        tickCount--;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Tick") //if its a tick
        {
            Tick tempTick = collision.gameObject.GetComponent<Tick>();
            if (tempTick.getOwnerPlayer() == ownerPlayerString && tempTick.getOwnerProvinceIndex() != MyGameManagerListIndex) // if its same team and not same province
            {
                Destroy(collision.gameObject);
                tickCount++;
            }
            else if (tempTick.getOwnerPlayer() != ownerPlayerString && tempTick.getOwnerProvinceIndex() != MyGameManagerListIndex) //if its not mine
            {
                for (int i = 0; i < tempTick.getDamage(); i++) //remove a number of stationed ticks according to attacking tick damage (lethal tick perk)
                {
                    RemoveTick();
                }

                Destroy(collision.gameObject);

                currentTickProductionTimer = tickProductionTimer * 2; //resets timer and delays reproduction of ticks

                if (tickCount < 0) //if reduced to negative (taken)
                {
                    tempTick.getOwnerPlayerScript().getMyPhotonViewComponent().RPC("TakeProvince", RpcTarget.AllBuffered, 
                        MyGameManagerListIndex); //get script reference from tick
                }
            }
        }
    }

    public void setTickProductionTimer(int newProductionTimer)
    {
        this.tickProductionTimer = newProductionTimer;
    }

    public PlayerCommander getOwnerPlayerScript()
    {
        return ownerPlayerScript;
    }

    public int getListIndex()
    {
        return MyGameManagerListIndex;
    }

    public void setListIndex(int i)
    {
        MyGameManagerListIndex = i;
    }

    public void setMouseOnMe(bool choice) //player controls
    {
        mouseOnMe = choice;
    }

    public void setNewOwner(PlayerCommander newOwnerScript, bool photonViewIsMine, int ownerProductionTime)
    {
        ownerPlayerScript = newOwnerScript;
        ownerPlayerString = ownerPlayerScript.getOwnerNickname();
        transform.parent.transform.SetParent(newOwnerScript.gameObject.transform);
        setTickProductionTimer(ownerProductionTime);
        ownerIndicator.SetActive(ownerPlayerScript.getIsMyPhotonView());
        tickCount = 0; //wipe out all negatives
        setColor(ownerPlayerScript.getSpriteRenderer());
    }

    public void setColor(SpriteRenderer newOwnerColor)
    {
        childDraggedSpriteRenderer.color = newOwnerColor.color;
    }

    public Vector2 getOriginalPos()
    {
        return originalPos;
    }
}
