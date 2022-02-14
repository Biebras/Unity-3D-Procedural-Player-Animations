using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualization : MonoBehaviour
{
    [SerializeField] private Transform playerGFX;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotationOffset = 90;

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
    }

    //Roatates player towards velocity when moving x and z axis
    private void HandleRotation()
    {
        var velocity = _kinematicBody.Velocity.normalized;
        var input = _playerInput.GetMovementInput();

        if (input.magnitude == 0)
            return;

        var curAngle = _transform.eulerAngles.y;
        var angle = Mathf.Atan2(velocity.z, -velocity.x) * Mathf.Rad2Deg + _rotationOffset;
        var lerp = Mathf.LerpAngle(curAngle, angle, _rotationSpeed * Time.deltaTime);
        _transform.eulerAngles = Vector3.up * lerp;
    }

    private void HandlePlayerTilt()
    {
        playerGFX.position = playerGFX.position;
    }
}
