using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AndroidAudioBypass;

public class AudioManager : Singleton<AudioManager>
{
    public AudioClip[] bgmClip;
    public AudioSource bgmPlayer;
    public static float OriginalBGMVolume = 1.0f;

    public List<BypassAudioSource> soundPool = new List<BypassAudioSource>();
    private List<float> originVolume = new List<float>();

    private void Start()
    {
        for(int i=0; i < soundPool.Count; i++)
        {
            originVolume.Add(soundPool[i].m_volume); // 사운드 스탑이 없어서 볼륨컨트롤을 위해 초기 음량을 저장.
        }
    }

    public void playBGM(int _index = 0)
    {
        bgmPlayer.Stop();
        bgmPlayer.clip = bgmClip[_index];
        bgmPlayer.Play();
    }

    public void Play(int _index)
    {
        soundPool[_index].m_volume = originVolume[_index];
        soundPool[_index].Play();     
    }
 
    public void Stop(int _index)
    {
        soundPool[_index].m_volume = 0.0f;
    }

    public static IEnumerator FadeOut(AudioSource _source, float _speed)
    {
        while (true)
        {
			if(_source == null)
			{
				yield break;
			}

            if (_source.volume <= 0.0f)
            {
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
			if (_source == null)
				yield break;

			if (_source.volume >= OriginalBGMVolume)
                yield break;

            _source.volume += _speed;
            yield return null;
        }
    }

	public static IEnumerator FadeOutInGameBGM(AudioSource _source, float _speed)
	{
		_source.volume = OriginalBGMVolume;
		while (true)
		{
			if (_source == null)
				yield break;

			if (_source.volume <= 0.0f)
			{
				_source.volume = OriginalBGMVolume;
				Instance.playBGM(0);
				yield break;
			}

			_source.volume -= _speed;
			yield return null;
		}
	}
}
