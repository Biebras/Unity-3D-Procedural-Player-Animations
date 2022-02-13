using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Walking")]
    [SerializeField] private float _maxMove = 13;
    [SerializeField] private float _acceleration = 180;
    [SerializeField] private float _deceleration = 90;

    [SerializeField] private float _gravity = 10;
    [SerializeField] private float _maxFallSpeed = 40;
    [SerializeField] private float _minFallSpeed = 8;

    [SerializeField] private Vector3 _velocity;
    private Vector3 _lastPosition;
    private Vector3 _rawMovement;
    private Vector3 _nextPos;
    private Camera _camera;

    private Transform _transform;
    private PlayerCollision _playerCollision;
    private PlayerInput _playerInput;
    private Rigidbody rb;

    private void Awake()
    {
        _transform = transform;
        _playerCollision = GetComponent<PlayerCollision>();
        _playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //assuming we only using the single camera:
        _camera = Camera.main;
    }

    private void Update()
    {
        CalculateVelocity();

        CalculateGravity();

        Walk();

        ClampSpeedY();

        Move();
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    private void CalculateVelocity()
    {
        _velocity = (_transform.position - _lastPosition) / Time.deltaTime;
        _lastPosition = _transform.position;
    }

    private void CalculateGravity()
    {
        _rawMovement.y -= _gravity * Time.deltaTime;
    }

    private void Walk()
    {
        var input = _playerInput.GetMovementInput();
        var camForward = _camera.transform.forward;
        var camRight = _camera.transform.right;

        // project forward and right vectors on the horizontal plane(y = 0)
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        var desiredMoveDirection = (camRight * input.x + camForward * input.y) * _maxMove;

        if (input.magnitude != 0)
        {
            _rawMovement.x = Mathf.MoveTowards(_rawMovement.x, desiredMoveDirection.x, _acceleration * Time.deltaTime);
            _rawMovement.z = Mathf.MoveTowards(_rawMovement.z, desiredMoveDirection.z, _acceleration * Time.deltaTime);
        }
        else
        {
            _rawMovement.x = Mathf.MoveTowards(_rawMovement.x, 0, _deceleration * Time.deltaTime);
            _rawMovement.z = Mathf.MoveTowards(_rawMovement.z, 0, _deceleration * Time.deltaTime);
        }
    }

    private void ClampSpeedY()
    {
        if (_rawMovement.y < 0)
            _rawMovement.y = Mathf.Clamp(_rawMovement.y, -_maxFallSpeed, -_minFallSpeed);
    }

    private void Move()
    {
        var pos = _transform.position;
        var move = _rawMovement * Time.deltaTime;
        _nextPos = pos + move;

        _playerCollision.HandleCollisions(_nextPos, ref move);

        _transform.position += move;
    }
}
