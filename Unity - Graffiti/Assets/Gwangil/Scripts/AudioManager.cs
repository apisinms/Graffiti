using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AndroidAudioBypass;

public class AudioManager : Singleton<AudioManager>
{
    public List<BypassAudioSource> soundPool = new List<BypassAudioSource>();

    public void Play(int index)
    {
        soundPool[index].Play();
    }
}
