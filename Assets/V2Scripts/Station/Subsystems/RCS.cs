using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.InputSystem;

public class RCS : SubSystemBase
{
    public delegate void Thrust(ThrusterFiring data);
    public static event Thrust thrust;

    InputSystem_Actions input;
    bool active = false;

    Transform relativeMovement;

    Vector3 moveVector = Vector3.zero;
    Vector3 rotation = Vector3.zero;
    Vector2 mouseMove = Vector2.zero;

    bool stabilise = false;

    Vector3 prevMoveVector = Vector3.zero;
    Vector3 prevRotation = Vector3.zero;

    public float xSensitivity = 5f;
    public float ySensitivity = 5f;
    float rollBoost = 15.0f;

    [SerializeField]
    float thrusterForce;
    [SerializeField]
    float rotationThrusterForce;

    float nitrogenFuel, maxNitrogenFuel;

    Rigidbody body;

    ThrusterFiring data;

    private void Awake()
    {
        input = new InputSystem_Actions();
        body = GetComponent<Rigidbody>();
        data = new ThrusterFiring();
        relativeMovement = transform;
    }

    private void Update()
    {
        if (active)
        {
            Vector3 movement = relativeMovement.TransformDirection(moveVector);
            Vector3 rot = relativeMovement.TransformVector(rotation);

            //fireThrusters(movement, rot);

            body.AddForce(movement * thrusterForce * Time.deltaTime);
            body.AddTorque(rot * rotationThrusterForce * Time.deltaTime);

            if(moveVector != prevMoveVector || rotation != prevRotation)
            {
                rot = rot.normalized;
                //Debug.Log("Fire Thrusters");
                //Debug.Log(movement);
                fireThrusters(transform.InverseTransformDirection(movement), transform.InverseTransformVector(rot));
            }

            prevMoveVector = moveVector;
            prevRotation = rotation;

            if(moveVector == Vector3.zero && rotation == Vector3.zero && body.angularVelocity.magnitude > 0 && stabilise)
            {
                
                rot = transform.TransformVector(-body.angularVelocity);
                fireThrusters(movement, rot);
            }
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

    void fireThrusters(Vector3 _movement, Vector3 _rotation)
    {
        data.RCSCaller  = this;
        data.rotation = _rotation;
        data.movement = _movement;

        thrust(data);
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
        stabilise = true;
    }

    void onShiftCanceled(InputAction.CallbackContext _value)
    {
        body.angularDrag = 0.05f;
        stabilise = false;
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
