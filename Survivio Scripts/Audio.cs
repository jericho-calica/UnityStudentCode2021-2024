using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum sfxenum
{
    enemydeath, gameloss, gamewin, getauto, getpistol, getshotgun, shotauto, shotpistol, shotshotgun
}

public class Audio : MonoBehaviour
{
    public static Audio instance;
    public AudioSource Source;
    List<AudioClip> Clips = new List<AudioClip>();

    void Awake()
    {
        instance = this;
        foreach (string audio in System.Enum.GetNames(typeof(sfxenum)))
        {
            Clips.Add(Resources.Load<AudioClip>("sfx/" + audio));
        }
    }

    void OnDestroy()
    {
        instance = null;
    }

    public void PlaySound(sfxenum audio)
    {
        Source.PlayOneShot(Clips[(int)audio]);
    }
}
