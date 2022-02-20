using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerVisualization : MonoBehaviour
{
    [SerializeField] private Transform _playerGFX;

    [Header("Player Walk Animation")]
    [SerializeField] public float _stepRadius = 0.5f;
    [SerializeField] public float _stepAngle;

    [Header("Player Rotation")]
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotationOffset = -90;

    [Header("Player Tilt")]
    [SerializeField] private float _maxTilt = 30;
    [SerializeField] private float _tiltSpeed= 10;

    private int _nextStepAngle;

    private Transform _transform;
    private KinematicBody _kinematicBody;
    private PlayerInput _playerInput;
    private RigAnimationController _rigAnimation;

    private void Awake()
    {
        _transform = transform;
        _kinematicBody = GetComponent<KinematicBody>();
        _playerInput = GetComponent<PlayerInput>();
        _rigAnimation = GetComponent<RigAnimationController>();
    }

    private void Update()
    {
        WalkAnimation();
        HandleRotation();
        TiltToAcceleration();
    }

    private void WalkAnimation()
    {
        var vel = _kinematicBody.Velocity;
        vel.y = 0;
        _stepRadius = vel.magnitude / 20;
        IncramentStepAngle(vel.magnitude * 20 * Time.deltaTime);

        if (_nextStepAngle / 90 != (int)_stepAngle / 90)
            return;

        _rigAnimation.PlayAnimation("Run");
        _nextStepAngle += 90;

        if (_nextStepAngle >= 360)
            _nextStepAngle -= 360;
    }

    private void IncramentStepAngle(float amount)
    {
        _stepAngle += amount;

        if (_stepAngle > 360)
            _stepAngle -= 360;
    }

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

    private void OnDrawGizmos()
    {
        Handles.color = Color.white;
        DrawCircle(_stepRadius, _stepAngle);
    }

#if UNITY_EDITOR
    private void DrawCircle(float radius, float angle)
    {
        if (_transform == null)
            _transform = transform;
        
        Handles.color = Color.white;
        Handles.DrawWireDisc(_transform.position, _transform.right, radius);
        DrawLine(radius, angle);
        DrawLine(radius, angle + 90);
    }

    private void DrawLine(float radius, float angle)
    {
        var startAngle = Mathf.Atan2(Vector3.forward.z, Vector3.forward.y) * Mathf.Rad2Deg;
        var newAngle = (startAngle + angle) * Mathf.Deg2Rad;
        var dir = new Vector3(0, Mathf.Cos(newAngle), Mathf.Sin(newAngle)).normalized;
        var worldDir = _transform.TransformDirection(dir);

        var pos1 = _transform.position + worldDir * radius;
        var pos2 = _transform.position - worldDir * radius;
        Handles.DrawLine(pos1, pos2);
    }
#endif
}
