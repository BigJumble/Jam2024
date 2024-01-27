using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static AudioClip witch, diging, success, makingPotion, walking;

    static AudioSource[] audioSources;

    void Start()
    {
        witch = Resources.Load<AudioClip>("Sounds/witch");
        diging = Resources.Load<AudioClip>("Sounds/diging");
        success = Resources.Load<AudioClip>("Sounds/success");
        makingPotion = Resources.Load<AudioClip>("Sounds/making_potion");

        audioSources = GetComponents<AudioSource>();
    }

    // SoundManagerScript.PlaySound("walking");


    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "witch":
                audioSources[0].PlayOneShot(witch);
                break;
            case "diging":
                audioSources[0].PlayOneShot(diging);
                break;
            case "success":
                audioSources[0].PlayOneShot(success);
                break;
            case "making_potion":
                audioSources[0].PlayOneShot(makingPotion);
                break;
            case "walking":
                if (!audioSources[2].isPlaying)
                {
                    audioSources[2].Play();
                }
                break;
            case "stop_walking":
                if (audioSources[2].isPlaying)
                {
                    audioSources[2].Stop();
                }
                break;
        }

    }

}
