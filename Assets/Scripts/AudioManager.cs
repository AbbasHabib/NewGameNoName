using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public Sounds[] _sounds;

    public static AudioManager Instance = null;

    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        foreach (Sounds S in _sounds)
        {
            S.Source = gameObject.AddComponent<AudioSource>();
            S.Source.clip = S.clip;

            S.Source.volume = S.Volume;
            S.Source.pitch = S.pitch;
            S.Source.loop = S.Loop;
        }
    }

    private void Start()
    {
        PlayBackGroundMusic();
    }

    private void PlayBackGroundMusic()
    {
        Play("BackGroundMusic");
    }
   
    public void Play(string MusicName)
    {
        Sounds S =  Array.Find(_sounds, Sound => Sound.Name == MusicName);
        if (S == null)
        {
            Debug.LogWarning("Couldn't Find Audio");
            return;
        }
        S.Source.Play();
        return;
    }
}
