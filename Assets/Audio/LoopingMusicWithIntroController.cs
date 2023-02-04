using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LoopingMusicWithIntroController : MonoBehaviour
{
    private enum MusicState { Off, Intro, Loop1, Loop2 };

    [SerializeField] private AudioMixer _audioMixer;

    [SerializeField] private AudioSource _asMusicFull;
    [SerializeField] private AudioSource _asEndMusicMixer1;
    [SerializeField] private AudioSource _asEndMusicMixer2;

    [SerializeField] private string _volumeExpParamIntro;
    [SerializeField] private string _volumeExpParamLoop1;
    [SerializeField] private string _volumeExpParamLoop2;

    [SerializeField] private float _durationFadeInBeginMusic;
    [SerializeField] private float _durationFadeEndLoopTransition;

    private MusicState _musicState;

    private double _durationIntro;
    private double _durationLoop;
    private double _nextStartTime;

    private bool _isPlayingMusic;

    private IEnumerator _CorFadeIn;
    private IEnumerator _CorCrossFadeTransition;

    private void OnEnable()
    {
        ResetLoopSystem();

        PlayMusic();
    }

    private void OnDisable()
    {
        StopMusic();
    }

    public void PlayMusic()
    {
        if (_isPlayingMusic)
        {
            Debug.LogWarning("LoopingMusicWithIntroController: Music is already playing !");
            return;
        }

        _isPlayingMusic = true;

        // Play Intro
        _musicState = MusicState.Intro;

        double startTime = AudioSettings.dspTime + 0.2;

        _asMusicFull.PlayScheduled(startTime);

        _nextStartTime = startTime + _durationIntro - _durationFadeEndLoopTransition;

        StartCoroutine(_CorFadeIn);
    }

    private void Update()
    {
        if (!_isPlayingMusic)
            return;

        double dspTime = AudioSettings.dspTime;
        bool prepareNextAudio = dspTime > (_nextStartTime);

        switch (_musicState)
        {
            case MusicState.Intro:
            case MusicState.Loop1:
                if (prepareNextAudio)
                {
                    StartCoroutine(_CorCrossFadeTransition);
                    Debug.Log("Loop1: " + dspTime + " > " + (_nextStartTime));

                    _asEndMusicMixer2.PlayScheduled(_nextStartTime);

                    if(_musicState == MusicState.Intro)
                    {
                        _nextStartTime += _durationLoop - _durationFadeEndLoopTransition;

                        _CorCrossFadeTransition = CorCrossFadeTransition(_volumeExpParamIntro, _volumeExpParamLoop2);
                    }
                    else
                    {
                        _nextStartTime += _durationLoop - _durationFadeEndLoopTransition;

                        _CorCrossFadeTransition = CorCrossFadeTransition(_volumeExpParamLoop1, _volumeExpParamLoop2);
                    }

                    _musicState = MusicState.Loop2;
                    StartCoroutine(_CorCrossFadeTransition);
                }
                break;

            case MusicState.Loop2:
                if (prepareNextAudio)
                {
                    Debug.Log("Loop2: " + dspTime + " > " + (_nextStartTime));

                    _asEndMusicMixer1.PlayScheduled(_nextStartTime);
                    _nextStartTime += _durationLoop - _durationFadeEndLoopTransition;
                    _musicState = MusicState.Loop1;

                    _CorCrossFadeTransition = CorCrossFadeTransition(_volumeExpParamLoop2, _volumeExpParamLoop1);
                    StartCoroutine(_CorCrossFadeTransition);
                }
                break;

            case MusicState.Off:
                Debug.LogWarning("LoopingMusicWithIntroController: Trying to play when music is stopped !");
                break;
        }

    }

    private IEnumerator CorCrossFadeTransition(string exposedParamFadeOut, string exposedParamFadeIn)
    {
        float time = 0;
        float minValue = 0.0001f;
        float maxValue = 1.0f;

        while (time <= _durationFadeEndLoopTransition)
        {
            time += Time.deltaTime;

            _audioMixer.SetFloat(exposedParamFadeOut, Mathf.Log10(Mathf.Lerp(maxValue, minValue, time / _durationFadeEndLoopTransition)) * 7);
            _audioMixer.SetFloat(exposedParamFadeIn, Mathf.Log10(Mathf.Lerp(minValue, maxValue, time / _durationFadeEndLoopTransition)) * 7);

            yield return null;
        }

        _audioMixer.SetFloat(exposedParamFadeOut, minValue);
        _audioMixer.SetFloat(exposedParamFadeIn, maxValue);
    }

    private IEnumerator CorFadeIn()
    {
        float time = 0;
        float minValue = 0.0001f;
        float maxValue = 1.0f;

        while (time <= _durationFadeInBeginMusic)
        {
            time += Time.deltaTime;

            // TODO : Change this Log10 slope to make the transition of your choice
            _audioMixer.SetFloat(_volumeExpParamIntro, Mathf.Log10(Mathf.Lerp(minValue, maxValue, time / _durationFadeInBeginMusic)) * 20);

            yield return null;
        }

        _audioMixer.SetFloat(_volumeExpParamIntro, maxValue);
    }

    private void StopMusic()
    {
        ResetLoopSystem();

        _asMusicFull.Stop();
        _asEndMusicMixer1.Stop();
        _asEndMusicMixer2.Stop();

    }

    private void ResetLoopSystem()
    {
        _musicState = MusicState.Off;
        _isPlayingMusic = false;
        _nextStartTime = 0;

        _durationIntro = GetAudioClipLength(_asMusicFull.clip);
        _durationLoop = GetAudioClipLength(_asEndMusicMixer1.clip);

        if (_CorFadeIn != null && _CorCrossFadeTransition != null)
        {
            StopCoroutine(_CorFadeIn);
            StopCoroutine(_CorCrossFadeTransition);
        }

        _CorFadeIn = CorFadeIn();
        _CorCrossFadeTransition = CorCrossFadeTransition(_volumeExpParamIntro, _volumeExpParamLoop1);
    }

    private double GetAudioClipLength(AudioClip audioClip)
    {
        return (double)audioClip.samples / audioClip.frequency;
    }
}
