using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerScreenPlayer : MonoBehaviour
{
    void Start()
    {
        Audio.instance.PlaySound(sfxenum.gamewin);
    }

    void Update()
    {
        transform.Rotate(0, 0, 180 * Time.deltaTime);
    }
}
