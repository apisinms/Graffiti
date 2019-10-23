using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AndroidAudioBypass;

public class AudioManager : Singleton<AudioManager>
{
    public List<BypassAudioSource> soundPool = new List<BypassAudioSource>();
    public AudioSource as_bgm;

    private void Start()
    {
        as_bgm.Play();
    }
    public void Play(int index)
    {
        soundPool[index].Play();
    }
}
