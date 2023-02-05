using UnityEngine;

[CreateAssetMenu(fileName = "RandomPitchVolumeAudioEvent", menuName = "Audio Events/RandomPitchVolumeAudioEvent")]
public class AERandomPitchVolume : AudioEventAsset
{
    public AudioClip[] audioClips;

    [Range(0f, 2f)]
    public float volume;

    [Range(0f, 3f)]
    public float pitch;

    [Range(0f, 2f)]
    public float volumeVariation;

    [Range(0f, 3f)]
    public float pitchVariation;

    public bool neverPlayTheSame;
    public bool isLooping;

    private int oldIndexPlayed = -1;

    public override void Play(AudioSource source)
    {
        if (audioClips.Length == 0) return;

        int clipIndexToPlay = Random.Range(0, audioClips.Length);

        if(neverPlayTheSame && audioClips.Length > 1)
        {
            while (clipIndexToPlay == oldIndexPlayed)
            {
                clipIndexToPlay = Random.Range(0, audioClips.Length);
            }
        }

        oldIndexPlayed = clipIndexToPlay;

        // -----

        source.clip = audioClips[clipIndexToPlay];
        source.volume = Mathf.Max(0, Random.Range(volume - volumeVariation, volume + volumeVariation));
        source.pitch = Mathf.Max(0.05f, Random.Range(pitch - pitchVariation, pitch + pitchVariation));
        source.loop = isLooping;

        source.PlayOneShot(source.clip);
    }

    public void ResetFields()
    {
        SetFileNameToDefault();

        volume = pitch = 1;
        volumeVariation = pitchVariation = 0;
    }

    private void Reset()
    {
        ResetFields();
    }
}
