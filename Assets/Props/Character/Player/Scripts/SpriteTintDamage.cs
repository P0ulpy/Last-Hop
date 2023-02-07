using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTintDamage : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _tintColor;

    private Color _baseSpriteColor;

    private IEnumerator _CorFadeAlphaText;

    private void Awake()
    {
        _baseSpriteColor = _spriteRenderer.color;
        _CorFadeAlphaText = CorFadeAlphaText(0);
    }

    public void StartTint(float durationFadeOut = 0.5f)
    {
        StopCoroutine(_CorFadeAlphaText);
        _CorFadeAlphaText = CorFadeAlphaText(durationFadeOut);
        StartCoroutine(_CorFadeAlphaText);
    }

    private IEnumerator CorFadeAlphaText(float durationFade)
    {
        float time = 0;
        Color startValue = _tintColor;
        Color targetValue = _baseSpriteColor;

        while (time < durationFade)
        {
            _spriteRenderer.color = Color.Lerp(startValue, targetValue, time / durationFade);
            time += Time.deltaTime;
            yield return null;
        }

        _spriteRenderer.color = targetValue;
    }
}
