using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RCS : SubSystemBase
{
    InputSystem_Actions input;
    bool active = false;

    Transform relativeMovement;

    Vector3 moveVector = Vector3.zero;
    Vector3 rotation = Vector3.zero;
    Vector2 mouseMove = Vector2.zero;

    public float xSensitivity = 5f;
    public float ySensitivity = 5f;
    float rollBoost = 15.0f;

    [SerializeField]
    float thrusterForce;
    [SerializeField]
    float rotationThrusterForce;

    float nitrogenFuel, maxNitrogenFuel;

    Rigidbody body;

    private void Awake()
    {
        input = new InputSystem_Actions();
        body = GetComponent<Rigidbody>();
        relativeMovement = transform;
    }

    private void Update()
    {
        if (active)
        {
            body.AddForce(relativeMovement.TransformDirection(moveVector) * thrusterForce * Time.deltaTime);
            body.AddTorque(relativeMovement.TransformVector(rotation) * rotationThrusterForce * Time.deltaTime);
        }
    }

    private void OnEnable()
    {
        input.Enable();

        input.Modules.Move.performed += onMovePerformed;
        input.Modules.Move.canceled += onMoveCanceled;
        input.Modules.Look.performed += onMouseMove;
        input.Modules.Look.canceled += onMouseStop;
        input.Modules.Roll.performed += onRollPerformed;
        input.Modules.Roll.canceled += onRollCanceled;
        //input.Modules.Vertical.performed += onVerticalMovementPerformed;
        //input.Modules.Vertical.canceled += onVerticalMovementCanceled;
        input.Modules.Stabilise.performed += onShift;
        input.Modules.Stabilise.canceled += onShiftCanceled;

    }

    private void OnDisable()
    {
        input.Disable();

        input.Modules.Move.performed -= onMovePerformed;
        input.Modules.Move.canceled -= onMoveCanceled;
        input.Modules.Look.performed -= onMouseMove;
        input.Modules.Look.canceled -= onMouseStop;
        input.Modules.Roll.performed -= onRollPerformed;
        input.Modules.Roll.canceled -= onRollCanceled;
        //input.Modules.Vertical.performed -= onVerticalMovementPerformed;
        //input.Modules.Vertical.canceled -= onVerticalMovementCanceled;
        input.Modules.Stabilise.performed -= onShift;
        input.Modules.Stabilise.canceled -= onShiftCanceled;
    }

    public void activate()
    { 
        active = true;
        body.angularDrag = 0.05f;
    }
    public void deactivate()
    { 
        active = false;
        body.angularDrag = 0.95f;
    }

    public void UpdateReferenceFrame(Transform _transform)
    {
        relativeMovement = _transform;
    }

    void onMovePerformed(InputAction.CallbackContext _value)
    {
        //Debug.Log("Move Performed");
        moveVector = _value.ReadValue<Vector3>();
    }

    void onMoveCanceled(InputAction.CallbackContext _value)
    {
        moveVector = Vector3.zero;
        //Debug.Log("Move Cancelled");
    }

    void onMouseMove(InputAction.CallbackContext _value)
    {
        Vector2 temp = _value.ReadValue<Vector2>();
        rotation.x = -temp.y * ySensitivity;
        rotation.y = temp.x * xSensitivity;
    }

    void onMouseStop(InputAction.CallbackContext _value)
    {
        rotation.x = 0;
        rotation.y = 0;
    }

    void onRollPerformed(InputAction.CallbackContext _value)
    {
        rotation.z = -_value.ReadValue<float>() * rollBoost;
    }

    void onRollCanceled(InputAction.CallbackContext _value)
    {
        rotation.z = 0;
    }

    void onShift(InputAction.CallbackContext _value)
    {
        body.angularDrag = 0.95f;
    }

    void onShiftCanceled(InputAction.CallbackContext _value)
    {
        body.angularDrag = 0.05f;
    }

    void onVerticalMovementPerformed(InputAction.CallbackContext _value)
    {
        moveVector.y = _value.ReadValue<float>();
    }

    void onVerticalMovementCanceled(InputAction.CallbackContext _value)
    {
        moveVector.y = 0;
    }
}
