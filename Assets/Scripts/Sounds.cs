using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sounds
{
    public string Name;

    public AudioClip clip;

    [Range(0f,1f)]
    public float Volume;
    [Range(0.1f, 3f)]
    public float pitch;

    public bool Loop;

    [HideInInspector]
    public AudioSource Source;
}
