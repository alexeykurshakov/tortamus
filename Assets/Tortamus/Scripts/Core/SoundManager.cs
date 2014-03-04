using System;
using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public enum Sounds
    {
        GearRotate,   	// Фон крутящейся шестеренки, фоном, слабо
        DiskRotate,     // Звук кручения диска. скорость воспроизведения звука прямо зависит от скорости кручения
        BallTap,        // Звук поднятого тапом с лотка шарика
        BallPlugin,     // Звук вставленного в гнездо диска шарика
        ButtonOnActive, // Звук ставшей активной центральной кнопки
        GearsWinRotate, // Звук сцепки шестеренок и их совместнго вращения
        DiskStopScream  // Звук резкого торможения диска, когда останавливаешь его пальцем
    }

    private AudioSource[] _sounds;

	private bool _isSoundEnabled;
    public bool IsSoundEnabled 
	{ 
		get { return _isSoundEnabled; }
		set
		{
			if (_isSoundEnabled == value)
				return;

			_isSoundEnabled = value;
			AudioListener.volume = value ? 1 : 0;
		}
	}

    public static SoundManager Instance { get; private set; }

    public AudioSource GetSound(Sounds type)
    {
        return _sounds[(int) type];        
    }

	public void StopAll()
	{
		foreach (var sound in _sounds)
		{
			sound.Stop ();
		}
	}

    private void Awake()
    {
        Instance = this;

		_isSoundEnabled = true;

        var enumSounds = (Sounds[])Enum.GetValues(typeof(Sounds));
        _sounds = new AudioSource[enumSounds.Length];

        foreach (Sounds sound in enumSounds)
        {
            _sounds[(int)sound] = CreateAudioSource(sound.ToString());
        }
    }

    private AudioSource CreateAudioSource(string clipName)
    {
        var obj = new GameObject();
        obj.name = clipName;

        var val = obj.AddComponent<AudioSource>();
        val.transform.parent = transform;
        val.transform.localPosition = new Vector3();
        val.clip = Resources.Load("Sounds/" + clipName) as AudioClip;

        return val;
    }

}
