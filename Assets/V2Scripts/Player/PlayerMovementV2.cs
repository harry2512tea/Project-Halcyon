using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public enum moveState
{
    walking,
    floating,
    seated
}

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CharacterController))]
public class PlayerMovementV2 : MonoBehaviour
{

    InputSystem_Actions input;

    Rigidbody body;
    CharacterController controller;

    bool inGravity = false;
    bool seated = false;
    bool shiftPressed = false;

    public float xSensitivity = 100f;
    public float ySensitivity = 100f;

    Vector3 walkingVelocity = Vector3.zero;

    Vector3 moveVector = Vector3.zero;
    Vector3 rotation = Vector3.zero;
    Vector2 mouseMove = Vector2.zero;

    [SerializeField]
    float walkSpeed, runSpeed, jumpForce;

    [SerializeField]
    float thrusterForce, rotationThrusterForce, maxRotationAngle;

    [SerializeField]
    Camera cam;

    [SerializeField]
    moveState movestate = moveState.floating;

    moveState prevMoveState;

    float gravity;
    float rotX;

    int gravityColliders = 0;

    private void Awake()
    {
        input = new InputSystem_Actions();
        body = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
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
        input.Player.Move.performed += onMovePerformed;
        input.Player.Move.canceled += onMoveCanceled;
        input.Player.Vertical.performed += onVerticalMovementPerformed;
        input.Player.Vertical.canceled += onVerticalMovementCanceled;

        InteractionControllerV2.OnSeated += OnSeated;
        InteractionControllerV2.OnStanding += OnStanding;

        DockingSystemPanel.OnSeated += OnSeated;
        CompartmentControllerV2.OnCancelControlling += OnStanding;

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
        input.Player.Move.performed -= onMovePerformed;
        input.Player.Move.canceled -= onMoveCanceled;
        input.Player.Vertical.performed -= onVerticalMovementPerformed;
        input.Player.Vertical.canceled -= onVerticalMovementCanceled;

        InteractionControllerV2.OnSeated -= OnSeated;
        InteractionControllerV2.OnStanding -= OnStanding;

        DockingSystemPanel.OnSeated -= OnSeated;
        CompartmentControllerV2.OnCancelControlling -= OnStanding;
    }

    private void Update()
    {
        switch(movestate)
        {
            case moveState.walking:
                walking();
                break;
            case moveState.floating:
                floating();
                break;
            case moveState.seated:
                break;
            default:
                break;
        }
    }

    void OnSeated(seatingData data)
    {
        if (data.Caller.gameObject == gameObject)
        {
            prevMoveState = movestate;
            movestate = moveState.seated;
            body.isKinematic = true;
            transform.parent = data.Target.transform;
            transform.localPosition = data.seatObject.transform.localPosition + new Vector3(0.5f, 1.0f, 0.0f);
            transform.localEulerAngles = new Vector3(90.0f, -90.0f, 0.0f);
            cam.enabled = false;
        }
    }

    void OnStanding(seatingData data)
    {
        if (data.Target == gameObject)
        {
            movestate = prevMoveState;
            transform.parent = null;
            body.isKinematic = false;
            cam.enabled = true;
        }

    }

    //Switching between movement types
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Gravity")
        {
            if(gravityColliders == 0)
            {
                onEnterGravity(other);
            }
            gravityColliders++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Gravity")
        {
            if (gravityColliders == 1)
            {
                onExitGravity(other);
            }
            gravityColliders--;
        }
    }

    void onEnterGravity(Collider other)
    {
        
        body.isKinematic = true;
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        transform.parent = other.attachedRigidbody.transform;
        transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
        controller.enabled = true;
    }

    void onExitGravity(Collider other)
    {
        controller.enabled = false;
        transform.parent = null;
        body.isKinematic = false;
        body.velocity = other.attachedRigidbody.velocity;
        cam.transform.localEulerAngles = Vector3.zero;
    }

    //Movement Handling Functions
    void walking()
    {
        if (controller.isGrounded)
        {
            gravity = 0;
            if (shiftPressed)
            {
                walkingVelocity = moveVector * runSpeed;
            }
            else
            {
                walkingVelocity = moveVector * walkSpeed;
            }
            walkingVelocity.y = 0;
        }
        else
        {
            gravity += 9.81f * Time.deltaTime;
            walkingVelocity.y -= gravity * 2.0f;
        }

        controller.Move(transform.TransformDirection(walkingVelocity) * Time.deltaTime);
        transform.Rotate(new Vector3(0.0f, rotation.y * xSensitivity * Time.deltaTime, 0.0f), Space.Self);
        rotX = Mathf.Clamp(rotX, -maxRotationAngle, maxRotationAngle);
        cam.transform.localRotation = Quaternion.Euler(-rotX, 0f, 0f);
        //cam.transform.Rotate(new Vector3(rotation.x * ySensitivity, 0.0f, 0.0f) * Time.deltaTime, Space.Self);
        
    }

    void floating()
    {
        body.AddRelativeForce(moveVector * thrusterForce * Time.deltaTime);
        body.AddRelativeTorque(rotation * rotationThrusterForce * Time.deltaTime);
    }

    //Input Handling Functions
    void onMovePerformed(InputAction.CallbackContext _value)
    {
        //Debug.Log("Move Performed");
        //Vector2 temp = _value.ReadValue<Vector2>();
        //moveVector = new Vector3(temp.x, 0.0f, temp.y);
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
        if(movestate == moveState.walking)
        {
            rotX += temp.y * ySensitivity * 10 * Time.deltaTime;
        }
    }

    void onMouseStop(InputAction.CallbackContext _value)
    {
        rotation.x = 0;
        rotation.y = 0;
    }

    void onRollPerformed(InputAction.CallbackContext _value)
    { 
        rotation.z = -_value.ReadValue<float>();
    }

    void onRollCanceled(InputAction.CallbackContext _value)
    {
        rotation.z = 0;
    }

    void onShift(InputAction.CallbackContext _value)
    {
        shiftPressed = true;
        body.angularDrag = 0.95f;
    }

    void onShiftCanceled(InputAction.CallbackContext _value)
    {
        shiftPressed = false;
        body.angularDrag = 0.05f;
    }

    void onVerticalMovementPerformed(InputAction.CallbackContext _value)
    {

        

        if(movestate == moveState.floating)
        {
            //moveVector.y = _value.ReadValue<float>();
        }
        else
        {
            if (_value.ReadValue<float>() > 0)
            {
                walkingVelocity.y = _value.ReadValue<float>();
            }
        }
    }

    void onVerticalMovementCanceled(InputAction.CallbackContext _value)
    {  
        //moveVector.y = 0;
    }
}