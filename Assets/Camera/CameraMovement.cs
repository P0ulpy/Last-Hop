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

    void Start()
    {
        _camera = GetComponent<Camera>();
        _screenShake = GetComponent<ScreenShake>();

        _originalCameraPosition = transform.position;
        _originalCameraOrthoSize = _camera.orthographicSize;
        _isZooming = false;

        _CorLerpOrthoSizeCamera = CorLerpOrthoSizeCamera(0, 0);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            ZoomCameraIn(3.0f);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            ZoomCameraOut(3.0f);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ScreenShake();
        }
    }

    public void ZoomCameraTo(float targetOrthoSize)
    {
        float duration = targetOrthoSize > _camera.orthographicSize ? _durationZoomOut : _durationZoomIn;

        StopCoroutine(_CorLerpOrthoSizeCamera);
        _CorLerpOrthoSizeCamera = CorLerpOrthoSizeCamera(targetOrthoSize, duration);
        StartCoroutine(_CorLerpOrthoSizeCamera);
    }

    public void ZoomCameraIn(float orthoSizeToRemove)
    {
        StopCoroutine(_CorLerpOrthoSizeCamera);
        _CorLerpOrthoSizeCamera = CorLerpOrthoSizeCamera(_camera.orthographicSize - orthoSizeToRemove, _durationZoomIn);
        StartCoroutine(_CorLerpOrthoSizeCamera);
    }

    public void ZoomCameraOut(float orthoSizeToAdd)
    {
        StopCoroutine(_CorLerpOrthoSizeCamera);
        _CorLerpOrthoSizeCamera = CorLerpOrthoSizeCamera(_camera.orthographicSize + orthoSizeToAdd, _durationZoomOut);
        StartCoroutine(_CorLerpOrthoSizeCamera);
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
