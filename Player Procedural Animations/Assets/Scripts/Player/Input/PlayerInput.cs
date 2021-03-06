using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private UserControls _inputActions;

    private void Awake()
    {
        _inputActions = new UserControls();
        _inputActions.Player.Enable();
    }

    public Vector3 GetMovementInput()
    {
        float backForward = _inputActions.Player.BackForward.ReadValue<float>();
        float leftRight = _inputActions.Player.LeftRight.ReadValue<float>();

        return new Vector3(leftRight, 0, backForward).normalized;
    }
}
