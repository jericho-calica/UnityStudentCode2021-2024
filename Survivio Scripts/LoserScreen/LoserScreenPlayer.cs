using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoserScreenPlayer : MonoBehaviour
{
    void Start()
    {
        Audio.instance.PlaySound(sfxenum.gameloss);
    }

    void Update()
    {
        transform.Rotate(0, 0, 180 * Time.deltaTime);
    }
}
