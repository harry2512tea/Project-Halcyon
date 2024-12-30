using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public enum moveState
{
    walking,
    floating,
    seated
}

public class PlayerMovementV2 : MonoBehaviour
{

    InputSystem_Actions input;

    Rigidbody body;

    bool inGravity = false;
    bool seated = false;
    bool shiftPressed = false;

    public float xSensitivity = 100f;
    public float ySensitivity = 100f;

    Vector3 moveVector = Vector3.zero;
    Vector3 rotation = Vector3.zero;
    Vector2 mouseMove = Vector2.zero;

    [SerializeField]
    float walkSpeed, runSpeed;

    [SerializeField]
    Camera cam;

    moveState movestate = moveState.walking;

    private void Awake()
    {
        input = new InputSystem_Actions();
        body = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.MouseMove.performed += onMouseMove;
        input.Player.MouseMove.canceled += onMouseStop;
        input.Player.Sprint.performed += onShift;
        input.Player.Sprint.canceled += onShiftCanceled;
        input.Player.Roll.performed += onRollPerformed;
        input.Player.Roll.canceled += onRollCanceled;
    }

    private void OnDisable()
    {
        input.Disable();

        input.Player.MouseMove.performed -= onMouseMove;
        input.Player.MouseMove.canceled -= onMouseStop;
        input.Player.Sprint.performed -= onShift;
        input.Player.Sprint.canceled -= onShiftCanceled;
        input.Player.Roll.performed -= onRollPerformed;
        input.Player.Roll.canceled -= onRollCanceled;
    }

    private void Update()
    {
        switch(movestate)
        {
            case moveState.walking:
                break;
            case moveState.floating:
                break;
            case moveState.seated:
                break;
            default:
                break;
        }
    }

    void onMovePerformed(InputAction.CallbackContext _value)
    {
        Vector2 temp = _value.ReadValue<Vector2>();
        moveVector = new Vector3(temp.x, 0.0f, temp.y);
    }
    void onMoveCanceled(InputAction.CallbackContext _value)
    {
        moveVector = Vector3.zero;
    }

    void onMouseMove(InputAction.CallbackContext _value)
    {
        Vector2 temp = _value.ReadValue<Vector2>();
        rotation.x = temp.y;
        rotation.y = temp.x;
    }

    void onMouseStop(InputAction.CallbackContext _value)
    {
        rotation.x = 0;
        rotation.y = 0;
    }

    void onRollPerformed(InputAction.CallbackContext _value)
    { 
        rotation.z = _value.ReadValue<float>();
    }

    void onRollCanceled(InputAction.CallbackContext _value)
    {
        rotation.z = 0;
    }

    void onShift(InputAction.CallbackContext _value)
    {
        shiftPressed = true;
    }

    void onShiftCanceled(InputAction.CallbackContext _value)
    {
        shiftPressed = false;
    }
}
