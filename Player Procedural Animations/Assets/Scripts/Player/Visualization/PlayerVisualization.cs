using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualization : MonoBehaviour
{
    [SerializeField] private Transform _playerGFX;

    [Header("Player Rotation")]
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotationOffset = -90;

    [Header("Player Tilt")]
    [SerializeField] private float _maxTilt = 30;
    [SerializeField] private float _tiltSpeed= 10;

    private Transform _transform;
    private KinematicBody _kinematicBody;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _transform = transform;
        _kinematicBody = GetComponent<KinematicBody>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        HandleRotation();
        TiltToAcceleration();
    }

    //Roatates player towards velocity when moving x and z axis
    private void HandleRotation()
    {
        var velocity = _kinematicBody.Velocity.normalized;
        var input = _playerInput.GetMovementInput();

        if (velocity.magnitude == 0 || input.magnitude == 0)
            return;

        var curAngle = _transform.eulerAngles.y;
        var angle = Mathf.Atan2(velocity.z, -velocity.x) * Mathf.Rad2Deg + _rotationOffset;
        var lerp = Mathf.LerpAngle(curAngle, angle, _rotationSpeed * Time.deltaTime);
        _transform.eulerAngles = Vector3.up * lerp;
    }

    void TiltToAcceleration()
    {
        var acceleration = _kinematicBody.Acceleration;
        Vector3 tilt = CalculateTilt(acceleration);
        Quaternion targetRotation = Quaternion.Euler(tilt);
        _playerGFX.transform.rotation = Quaternion.Lerp(_playerGFX.transform.rotation, targetRotation, _tiltSpeed * Time.deltaTime);
    }

    Vector3 CalculateTilt(Vector3 acceleration)
    {
        acceleration.y = 0;
        Vector3 tiltAxis = Vector3.Cross(acceleration, Vector3.up);
        float angle = Mathf.Clamp(-acceleration.magnitude, -_maxTilt, _maxTilt);
        Quaternion targetRotation = Quaternion.AngleAxis(angle, tiltAxis) * transform.rotation;
        return targetRotation.eulerAngles;
    }
}
