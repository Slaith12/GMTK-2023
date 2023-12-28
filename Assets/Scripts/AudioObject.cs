using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour
{
    public void Init(AudioClip audio, float volume = 1, float pitch = 1)
    {
        if(audio == null)
        {
            Destroy(gameObject);
            return;
        }
        Debug.Log($"Created audio object lasting {audio.length / pitch} seconds.");
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.volume = volume;
        source.pitch = pitch;
        source.PlayOneShot(audio);
        Destroy(gameObject, audio.length / pitch);
    }

    public static AudioObject Create(AudioClip audio, float volume = 1, float pitch = 1)
    {
        Debug.Log("Creating audio object");
        AudioObject audioObject = new GameObject("Audio", typeof(AudioObject)).GetComponent<AudioObject>();
        audioObject.Init(audio, volume, pitch);
        return audioObject;
    }
}
