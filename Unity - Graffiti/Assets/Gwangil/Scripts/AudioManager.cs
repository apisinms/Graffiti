using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AndroidAudioBypass;

public class AudioManager : Singleton<AudioManager>
{
    public AudioClip[] bgmClip;
    public AudioSource bgmPlayer;
    public static float OriginalBGMVolume = 0.3f;

    public List<BypassAudioSource> soundPool = new List<BypassAudioSource>();

    public void playBGM(int _index)
    {
        bgmPlayer.Stop();
        bgmPlayer.clip = bgmClip[_index];
        bgmPlayer.Play();
    }

    public void Play(int index)
    {
        soundPool[index].Play();
    }

    public static IEnumerator FadeOut(AudioSource _source, float _speed)
    {
        while (true)
        {
            if (_source.volume <= 0.0f)
            {
                Debug.Log("페이드아웃 탈출");
                yield break;
            }

            _source.volume -= _speed;
            yield return null;
        }
    }

    public static IEnumerator FadeIn(AudioSource _source, float _speed)
    {
        _source.volume = 0.0f;
        while (true)
        {
            if (_source.volume >= OriginalBGMVolume)
                yield break;

            _source.volume += _speed;
            yield return null;
        }
    }
}
