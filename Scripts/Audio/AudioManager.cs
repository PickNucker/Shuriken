using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    Dictionary<string, AudioClip> allSounds;

    void Awake()
    {
        instance = this;

        //Load Sounds:
        allSounds = new Dictionary<string, AudioClip>();
        foreach (AudioClip ac in Resources.LoadAll<AudioClip>("Audio"))
        {
            if(!allSounds.ContainsKey(ac.name))
                allSounds.Add(ac.name, ac);
        }
    }

    public static AudioClip GetSound(string soundName)
    {
        return instance.allSounds[soundName];
    }
}
