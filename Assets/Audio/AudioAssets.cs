using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioAssets : MonoBehaviour
{
    [System.Serializable]
    public struct AudioCategory
    {
        [SerializeField]
        private string categoryName;
        public string CategoryName => categoryName;

        [SerializeField] private AudioMixerGroup _audioMixer;
        public AudioMixerGroup AudioMixer => _audioMixer;

        [SerializeField]
        private AudioEventAsset[] audioEvents;
        public AudioEventAsset[] AudioEvents => audioEvents;
    }

    [SerializeField]
    private AudioMixerGroup _defaultAudioMixer;

    [SerializeField]
    private AudioCategory[] audioCategories;

    public void PlayAudioEvent(AudioEventAsset audioEvent)
    {
        AudioEventAsset soundToFind;

        foreach (var audioCat in audioCategories)
        {
            soundToFind = Array.Find(audioCat.AudioEvents, audioE => audioE == audioEvent);
            if(soundToFind != null)
            {
                soundToFind.Play();
                return;
            }
        }

        Debug.LogWarning("The sound  you tried to play isn't registered in AudioAssets object");
    }

    private void Awake()
    {
        if(audioCategories != null)
        {
            foreach (var cat in audioCategories)
            {
                AudioMixerGroup audioMixerToAssign = cat.AudioMixer ? cat.AudioMixer : _defaultAudioMixer;

                foreach (AudioEventAsset audioEvent in cat.AudioEvents)
                {
                    audioEvent.audioSource = gameObject.AddComponent<AudioSource>();
                    audioEvent.audioSource.outputAudioMixerGroup = audioMixerToAssign;
                }
            }
        }
    }
}
