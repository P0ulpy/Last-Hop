using System;
using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
	private Camera _cameraMovement;
	private Vector3 _originalPos;

	private IEnumerator _CorScreenShake;

	private void Awake()
	{
		_cameraMovement = GetComponent<Camera>();

		_originalPos = transform.position;

		_CorScreenShake = CorScreenShake(0, 0, 0, () => { });
	}

	public void SetNewOriginPos(Vector3 newOriginPos)
    {
		_originalPos = newOriginPos;
    }

	public void Shake(float shakeDuration = .5f, float shakeRefreshCooldown = .025f, float maxShakeForce = .8f)
	{
		StopCoroutine(_CorScreenShake);
		_CorScreenShake = CorScreenShake(shakeDuration, shakeRefreshCooldown, maxShakeForce, () => { });
		StartCoroutine(_CorScreenShake);
	}

	public void Shake(float shakeDuration, float shakeRefreshCooldown, float maxShakeForce, Action onEndShakeCallback)
    {
		StopCoroutine(_CorScreenShake);
		_CorScreenShake = CorScreenShake(shakeDuration, shakeRefreshCooldown, maxShakeForce, onEndShakeCallback);
		StartCoroutine(_CorScreenShake);
    }

	private IEnumerator CorScreenShake(float shakeDuration, float shakeRefreshCooldown, float maxShakeForce, Action onEndShakeCallback)
    {
		float time = 0;
		float timeRefreshed = 0;
		float currShakeAmount;

        while(time < shakeDuration)
        {
			if(timeRefreshed > shakeRefreshCooldown)
            {
				float t = (time / shakeDuration);
				float customLerpCurve = Mathf.Lerp(maxShakeForce, 0, t);

				currShakeAmount = customLerpCurve;

				_cameraMovement.transform.position = _originalPos + UnityEngine.Random.insideUnitSphere * currShakeAmount;

				timeRefreshed = 0;
			}

			yield return null;

			time += Time.deltaTime;
			timeRefreshed += Time.deltaTime;
		}

		_cameraMovement.transform.position = _originalPos;
		onEndShakeCallback();
	}
}