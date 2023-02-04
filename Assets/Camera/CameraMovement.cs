using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float _durationZoomIn;
    [SerializeField] private float _durationZoomOut;

    private Camera _camera;
    private ScreenShake _screenShake;

    private Vector3 _originalCameraPosition;
    private float _originalCameraOrthoSize;
    private bool _isZooming;

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

        while (time < duration)
        {
            SetOrthoSizeCamera(Mathf.SmoothStep(startOrthoSize, targetOrthoSize, time / duration));

            time += Time.deltaTime;
            yield return null;
        }

        SetOrthoSizeCamera(targetOrthoSize);

        _isZooming = false;
    }

    public void SetOrthoSizeCamera(float orthoSize)
    {
        _camera.orthographicSize = orthoSize;

        float newCamY = _originalCameraPosition.y + orthoSize - _originalCameraOrthoSize;
        Vector3 newCameraPosition = new Vector3(_camera.transform.position.x, newCamY, _camera.transform.position.z);

        _screenShake.SetNewOriginPos(new Vector3(transform.position.x, newCamY, transform.position.z));
        _camera.transform.SetPositionAndRotation(newCameraPosition, _camera.transform.rotation);
    }
}
