using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //private Camera _camera;

    private Vector3 _movementInput = Vector3.zero;

    private Vector3 _camRotation = Vector3.zero;

    public float _panCameraSpeed = 100f;

    public float _smoothingCamSpeed = 4f;

    private bool _useMouseInput = true;

    private int _initializedCamera = 10;

    private void Start()
    {
        _movementInput = Vector3.zero;
        _camRotation = transform.rotation.eulerAngles;
    }

    private void OnEnable()
    {
        _movementInput = Vector3.zero;
        _camRotation = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        CameraMovement();
    }

    public void ChangeMouseMovement(bool state)
    {
        _useMouseInput = state;
    }

    private void CameraMovement()
    {
        Vector3 input = Vector3.zero;
        if(_useMouseInput == true) {
            if(_initializedCamera <= 0) {
                input = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f);
            }
            else {
                _initializedCamera--;
            }
        }

        _movementInput = Vector3.Lerp(_movementInput, input, _smoothingCamSpeed * Time.deltaTime);

        float _pitch = Mathf.Clamp(_camRotation.x + (-_movementInput.y * _panCameraSpeed * Time.deltaTime * 10f), -90f, 45f);

        float _yaw = _camRotation.y + (_movementInput.x * _panCameraSpeed * Time.deltaTime * 10f);

        _camRotation = new Vector3(_pitch, _yaw, 0f);

        transform.eulerAngles = _camRotation;
    }
}
