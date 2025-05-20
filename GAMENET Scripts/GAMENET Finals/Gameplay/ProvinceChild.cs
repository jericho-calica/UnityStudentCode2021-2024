using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvinceChild : MonoBehaviour
{
    [SerializeField] ProvinceNode parentScript;

    bool mouseOnMe = false;

    void Start()
    {

    }

    private void OnMouseEnter()
    {
        Debug.Log("We detect a mouse on " + parentScript.gameObject.transform.parent.name);
        if (parentScript.getOwnerPlayerScript() != null)
        {
            if (parentScript.getOwnerPlayerScript().getIsMyPhotonView()) //activate and be draggable if mine
            {
                mouseOnMe = true;
                parentScript.setMouseOnMe(mouseOnMe);
            }
        }
    }

    private void OnMouseExit()
    {
        if (parentScript.getOwnerPlayerScript() != null)
        {
            if (parentScript.getOwnerPlayerScript().getIsMyPhotonView())
            {
                mouseOnMe = false;
                parentScript.setMouseOnMe(mouseOnMe);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (mouseOnMe && collision.tag == "Province" && collision.GetComponent<ProvinceNode>() != parentScript) //attack province
        {
            Debug.Log(collision.gameObject.name);

            parentScript.WantToMoveTo(collision.GetComponent<ProvinceNode>());
        }
    }
}
