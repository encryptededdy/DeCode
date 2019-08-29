using Assets.UltimateIsometricToolkit.Scripts.Core;
using UnityEngine;

public class SmoothCameraMover : MonoBehaviour
{
    private Transform _cameraTransform;
    private Camera _camera;
    public float smoothTime = 1F;
    private Vector3 _velocity = Vector3.zero;
    private float _zoomVelocity = 0f;
    private Vector3 _targetPosition;
    private float _targetZoom;
    
    // Start is called before the first frame update
    void Start()
    {
        _cameraTransform = GetComponent<IsoTransform>().transform;
        _camera = GetComponent<Camera>();
        _targetPosition = new Vector3(0, 0, 0);
        _targetZoom = 6f;
    }

    // Update is called once per frame
    void Update()
    {
        _camera.orthographicSize =
            Mathf.SmoothDamp(_camera.orthographicSize, _targetZoom, ref _zoomVelocity, smoothTime);
        // Smoothly move the camera towards that target position
        _cameraTransform.position = Vector3.SmoothDamp(_cameraTransform.position, _targetPosition, ref _velocity, smoothTime);
    }

    public void ZoomEntrance()
    {
        _targetPosition = new Vector3(-6, 1, 0);
        _targetZoom = 3f;
    }
    public void ZoomNormal()
    {
        _targetPosition = new Vector3(0, 0, 0);
        _targetZoom = 6f;
    }
    
    public void ZoomNormalMagnified()
    {
        _targetPosition = new Vector3(0, 1, 0);
        _targetZoom = 5f;
    }
    
    public void ZoomExit()
    {
        _targetPosition = new Vector3(3, 0, 0);
        _targetZoom = 3f;
    }

    public void ZoomEntranceStack()
    {
        _targetPosition = new Vector3(-4, 1, 1);
        _targetZoom = 3f;
    }
}
