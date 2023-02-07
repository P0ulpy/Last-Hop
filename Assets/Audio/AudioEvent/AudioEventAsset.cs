using UnityEngine;

public class AudioEventAsset : AudioEvent
{
    [HideInInspector]
    public AudioSource audioSource;

    public override void Play(AudioSource source)
    {
        source.Play();
    }

    public void Stop(AudioSource source)
    {
        source.Stop();
    }

    public virtual void Play()
    {
        if(audioSource)
        {
            Play(audioSource);
        }
    }

    public virtual void Stop()
    {
        if (audioSource)
        {
            Stop(audioSource);
        }
    }
}
