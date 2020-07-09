using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{

    [Serializable]
    public struct SoundEffect
    {
        public string name;
        public AudioClip sound;
    }

    public SoundEffect[] sounds;


    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }


    AudioClip FindClip(string id)
    {
        foreach (SoundEffect soundFx in sounds)
        {
            if (soundFx.name == id)
                return soundFx.sound;
        }

        return null;
    }

    public void PlaySound(string soundName)
    {
        audioSource.PlayOneShot(FindClip(soundName));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
