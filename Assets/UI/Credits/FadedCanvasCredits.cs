using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadedCanvasCredits : MonoBehaviour
{
    [Header("Text credits. Don't forget to add all children you want :o")]
    [SerializeField] private CanvasGroup[] _creditsCanvas;

    [Header("Durations")]
    [SerializeField] private float _durationFadeIn;
    [SerializeField] private float _durationTextDisplays;
    [SerializeField] private float _durationFadeOut;
    [SerializeField] private float _durationNoTextDisplay;

    private int _currentCanvasGroupIndex;

    private void OnEnable()
    {
        foreach (var canvas in _creditsCanvas)
        {
            canvas.alpha = 0;
            canvas.gameObject.SetActive(false);
        }

        _currentCanvasGroupIndex = 0;

        StartCoroutine(CorWaitAndShowText(false));
    }

    private IEnumerator CorWaitAndShowText(bool isWaitingBeforeFade = true)
    {
        // Wait
        yield return new WaitForSeconds(isWaitingBeforeFade ? _durationNoTextDisplay : 0);

        _creditsCanvas[_currentCanvasGroupIndex].gameObject.SetActive(true);

        //Fade in
        StartCoroutine(CorFadeAlphaText(1, _durationFadeIn, () =>
        {
            StartCoroutine(CorWaitAndHideText());
        }));
    }

    private IEnumerator CorWaitAndHideText(bool isWaitingBeforeFade = true)
    {
        yield return new WaitForSeconds(isWaitingBeforeFade ? _durationTextDisplays : 0);
        
        //Fade out
        StartCoroutine(CorFadeAlphaText(0, _durationFadeIn, () =>
        {
            _creditsCanvas[_currentCanvasGroupIndex].gameObject.SetActive(false);

            _currentCanvasGroupIndex++;

            //Start new text display
            if (_currentCanvasGroupIndex < _creditsCanvas.Length)
            {
                StartCoroutine(CorWaitAndShowText());
            }
            else
            {
                Debug.Log("Gg le jeu est fini");
            }
        }));
    }

    private IEnumerator CorFadeAlphaText(float targetAlpha, float durationFade, Action onCoroutineEnd)
    {
        float time = 0;
        float startAlpha = _creditsCanvas[_currentCanvasGroupIndex].alpha;

        while (time < durationFade)
        {
            _creditsCanvas[_currentCanvasGroupIndex].alpha = Mathf.Lerp(startAlpha, targetAlpha, time / durationFade);
            time += Time.deltaTime;
            yield return null;
        }

        _creditsCanvas[_currentCanvasGroupIndex].alpha = targetAlpha;

        onCoroutineEnd();
    }
}
