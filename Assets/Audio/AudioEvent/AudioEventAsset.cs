using UnityEngine;

public class AudioEventAsset : AudioEvent
{
    [HideInInspector]
    public AudioSource audioSource;

    public override void Play(AudioSource source)
    {
        source.Play();
    }

    public virtual void Play()
    {
        if(audioSource)
        {
            Play(audioSource);
        }
    }
}
