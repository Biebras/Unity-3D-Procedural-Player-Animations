using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualization : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotationOffset = 90;

    private Transform _transform;
    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _transform = transform;
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        HandleRotation();
    }

    private void HandleRotation()
    {
        var velocity = _playerMovement.GetVelocity().normalized;

        if (velocity.magnitude == 0)
            return;

        var curAngle = _transform.eulerAngles.y;
        var angle = Mathf.Atan2(velocity.z, -velocity.x) * Mathf.Rad2Deg + _rotationOffset;
        var lerp = Mathf.LerpAngle(curAngle, angle, _rotationSpeed * Time.deltaTime);
        _transform.eulerAngles = Vector3.up * lerp;
    }
}
