using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform _backgroundTransform;
    
    [Header("Config")]
    [SerializeField] private float _durationZoomIn;
    [SerializeField] private float _durationZoomOut;

    private Camera _camera;
    private ScreenShake _screenShake;

    private Vector3 _originalCameraPosition;
    private float _originalCameraOrthoSize;
    private bool _isZooming;
    
    private Vector3 _originalBackgroundPosition;
    private float _originalBackgroundScale;

    private IEnumerator _CorLerpOrthoSizeCamera;

    private void Awake()
    {
        _CorLerpOrthoSizeCamera = CorLerpOrthoSizeCamera(0, 0);
    }

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _screenShake = GetComponent<ScreenShake>();

        _originalCameraPosition = transform.position;
        _originalCameraOrthoSize = _camera.orthographicSize;
        _isZooming = false;
        
        _originalBackgroundPosition = _backgroundTransform.position;
        _originalBackgroundScale = _backgroundTransform.localScale.x;
    }

    public void ZoomCameraTo(float targetOrthoSize)
    {
        StartZoom(targetOrthoSize, targetOrthoSize > _camera.orthographicSize ? _durationZoomOut : _durationZoomIn);
    }

    public void ZoomCameraIn(float orthoSizeToRemove)
    {
        StartZoom(_camera.orthographicSize - orthoSizeToRemove, _durationZoomIn);
    }

    public void ZoomCameraOut(float orthoSizeToAdd)
    {
        StartZoom(_camera.orthographicSize + orthoSizeToAdd, _durationZoomOut);
    }

    public void ScreenShake(float shakeDuration = .5f, float shakeRefreshCooldown = .025f, float maxShakeForce = .8f)
    {
        if (_isZooming)
        {
            Debug.LogWarning("CameraMovement : Can't shake while zoom");
            return;
        }

        _screenShake.Shake(shakeDuration, shakeRefreshCooldown, maxShakeForce);
    }

    private void StartZoom(float orthoSize, float duration)
    {
        if(_CorLerpOrthoSizeCamera != null)
            StopCoroutine(_CorLerpOrthoSizeCamera);

        _CorLerpOrthoSizeCamera = CorLerpOrthoSizeCamera(orthoSize, duration);
        StartCoroutine(_CorLerpOrthoSizeCamera);
    }

    private IEnumerator CorLerpOrthoSizeCamera(float targetOrthoSize, float duration)
    {
        _isZooming = true;

        float time = 0;
        float startOrthoSize = _camera.orthographicSize;

        float targetBackgroundOrthoSize = targetOrthoSize - _camera.orthographicSize;

        Vector3 startBackgroundPos = _backgroundTransform.position;
        Vector3 targetBackgroundPos = _backgroundTransform.position + new Vector3(targetBackgroundOrthoSize, targetBackgroundOrthoSize, targetBackgroundOrthoSize);
        float startBackgroundSize = _backgroundTransform.localScale.x;
        float targetBackgroundSize = _backgroundTransform.localScale.x + targetBackgroundOrthoSize;

        while (time < duration)
        {
            float t = time / duration;
            
            SetOrthoSizeCamera(Mathf.SmoothStep(startOrthoSize, targetOrthoSize, t));

            _backgroundTransform.position = Vector3.Lerp(startBackgroundPos, targetBackgroundPos, t);

            float s = Mathf.Lerp(startBackgroundSize, targetBackgroundSize, t);
            _backgroundTransform.localScale = new Vector3(s, s, s);

            time += Time.deltaTime;
            yield return null;
        }

        SetOrthoSizeCamera(targetOrthoSize);

        _isZooming = false;
    }

    private void SetOrthoSizeCamera(float orthoSize)
    {
        _camera.orthographicSize = orthoSize;

        float newCamY = _originalCameraPosition.y + orthoSize - _originalCameraOrthoSize;
        Vector3 newCameraPosition = new Vector3(_camera.transform.position.x, newCamY, _camera.transform.position.z);

        _screenShake.SetNewOriginPos(new Vector3(transform.position.x, newCamY, transform.position.z));
        _camera.transform.SetPositionAndRotation(newCameraPosition, _camera.transform.rotation);
    }
}
