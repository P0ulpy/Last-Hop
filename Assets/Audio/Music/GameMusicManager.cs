using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameMusicManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource[] _allAudioSources;
    [SerializeField] private string[] _stringExposedVolumeParams;
    [SerializeField] private bool _startMusicAtBeginPlay;
    [SerializeField] private float _durationFadeEndLoopTransition;

    private int _currentStringParamIndex;

    public bool _willPlayNewClip; // JUST USE IT FROM THE OUTSIDE TO DEBUG AND FORCE MUSIC CHANGE
    private bool _isMusicSystemActive;

    private IEnumerator _CorPrepareNextMusicTransition;

    /*
     * Dès qu'on lance une musique, on lance d'avance la prochaine musique, même si y'a pas besoin
     Dès qu'on souhaite lancer une musique de l'extérieur, _willPlayNewLayer = true;
     Quand la prochaine musique se lance
        Si on doit changer de musique, on stop la courante.
        Sinon on stop celle qui vient de se lancer, puis on la relance le temps de la musique courante.
     */

    private void Awake()
    {
        _willPlayNewClip = false;
        _isMusicSystemActive = false;

        _CorPrepareNextMusicTransition = CorPrepareNextMusicTransition(0);

        _currentStringParamIndex = 0;

        foreach (var stringParam in _stringExposedVolumeParams)
        {
            _audioMixer.SetFloat(stringParam, Mathf.Log10(0.0001f) * 20);
        }
    }

    private void Start()
    {
        if(_startMusicAtBeginPlay)
        {
            StartMusicSystem();
        }
    }

    // ======================================

    public void StartMusicSystem()
    {
        _isMusicSystemActive = true;

        foreach(var stringParam in _stringExposedVolumeParams)
        {
            _audioMixer.SetFloat(stringParam, Mathf.Log10(0.0001f) * 20);
        }

        _audioMixer.SetFloat(_stringExposedVolumeParams[0], 1.0f);

        StartPlayingNextLayer();

        double startTime = AudioSettings.dspTime + 0.2;
        foreach (var audioSource in _allAudioSources)
        {
            audioSource.PlayScheduled(startTime);
        }
    }

    public void StopMusicSystem()
    {
        _isMusicSystemActive = false;

        foreach (var audioSource in _allAudioSources)
        {
            audioSource.Stop();
        }
    }

    public void PlayNextMusicAfterCurrentOne()
    {
        _willPlayNewClip = true;
    }

    // ======================================

    private void StartPlayingNextLayer()
    {
        if (!_isMusicSystemActive || _currentStringParamIndex >= _stringExposedVolumeParams.Length - 1)
        {
            Debug.Log("End of Music");
        }

        double durationBeforePlayNextLayer = GetAudioClipLength(_allAudioSources[_currentStringParamIndex].clip) - _durationFadeEndLoopTransition;

        StopCoroutine(_CorPrepareNextMusicTransition);
        _CorPrepareNextMusicTransition = CorPrepareNextMusicTransition(durationBeforePlayNextLayer);
        StartCoroutine(_CorPrepareNextMusicTransition);
    }

    private IEnumerator CorPrepareNextMusicTransition(double duration)
    {
        yield return new WaitForSeconds((float)duration);

        if(_willPlayNewClip)
        {
            _willPlayNewClip = false;

            // Fade out current one, fade in new one
            StartCoroutine(CorCrossFadeTransition(_stringExposedVolumeParams[_currentStringParamIndex], _stringExposedVolumeParams[_currentStringParamIndex + 1]));

            _currentStringParamIndex++;
        }

        StartPlayingNextLayer();
    }

    private IEnumerator CorCrossFadeTransition(string exposedParamFadeOut, string exposedParamFadeIn)
    {
        float time = 0;
        float minValue = 0.0001f;
        float maxValue = 1.0f;

        while (time <= _durationFadeEndLoopTransition)
        {
            time += Time.deltaTime;

            _audioMixer.SetFloat(exposedParamFadeOut, Mathf.Log10(Mathf.Lerp(maxValue, minValue, time / _durationFadeEndLoopTransition)) * 20);
            _audioMixer.SetFloat(exposedParamFadeIn, Mathf.Log10(Mathf.Lerp(minValue, maxValue, time / _durationFadeEndLoopTransition)) * 20);

            yield return null;
        }

        _audioMixer.SetFloat(exposedParamFadeOut, Mathf.Log10(minValue) * 20);
        _audioMixer.SetFloat(exposedParamFadeIn, Mathf.Log10(maxValue) * 20);
    }

    // ======================================
    private double GetAudioClipLength(AudioClip audioClip)
    {
        return (double)audioClip.samples / audioClip.frequency;
    }
}
