using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float sensitivity;
    public Transform CenterOfMass;
    bool controlled, Docked, MainDrive;
    Rigidbody body;
    Vector3 movement, rotation;
    public float maxRotation, RCSForce, rotationForce, maxThrust;
    float currentThrust, G;
    GameObject DockedObject;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        body.inertiaTensor = body.inertiaTensor;
        body.centerOfMass = body.centerOfMass;

        G = 9.81f;
    }

    private void Update()
    {
        body.angularVelocity = Vector3.zero;
        if(controlled)
        {
            DoMovement();
        }
        else if(body.velocity.magnitude < 0.05f)
        {
            body.velocity = Vector3.zero;
        }
        clampRotation();
        transform.Rotate(rotation * Time.deltaTime, Space.Self);

    }
    void DoMovement()
    {
        movement = GetMovement();
        movement = transform.TransformDirection(movement) * RCSForce * Time.deltaTime;
        body.AddForce(movement, ForceMode.VelocityChange);

        rotation += new Vector3(ClampRotation(GetRotation().x * rotationForce), ClampRotation(GetRotation().y * rotationForce), ClampRotation(GetRotation().z * rotationForce));
        if(Input.GetButton("Stabilise"))
        {
            Stabilise();
        }
    }


    Vector3 GetMovement()
    {
        return new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("UpDown"), Input.GetAxis("Vertical"));
    }

    Vector3 GetRotation()
    {
        Vector3 rotate;
        rotate.x = -(Input.GetAxis("Mouse Y") * sensitivity);
        rotate.y = Input.GetAxis("Mouse X") * sensitivity;
        rotate.z = Input.GetAxis("Rotation");
        return rotate;
    }

    float ClampRotation(float angle)
    {
        return Mathf.Clamp(angle * Time.deltaTime, -maxRotation, maxRotation);
    }
    void clampRotation()
    {
        rotation.x = Mathf.Clamp(rotation.x, -maxRotation, maxRotation);
        rotation.y = Mathf.Clamp(rotation.y, -maxRotation, maxRotation);
        rotation.z = Mathf.Clamp(rotation.z, -maxRotation, maxRotation);
    }

    void Stabilise()
    {
        rotation.x = Mathf.MoveTowards(rotation.x, 0f, 0.5f);
        rotation.y = Mathf.MoveTowards(rotation.y, 0f, 0.5f);
        rotation.z = Mathf.MoveTowards(rotation.z, 0f, 0.5f);
    }




    public void setControlled(bool controlledSetting)
    {
        controlled = controlledSetting;
    }
    public void SetVelocity(Vector3 newVel)
    {
        body.velocity = newVel;
    }
    public Vector3 GetVelocity()
    {
        return body.velocity;
    }
    public Vector3 GetShipRotation()
    {
        return rotation;
    }
    public bool UnderThrust()
    {
        return MainDrive;
    }

    public float GetAcceleration()
    {
        return currentThrust * G;
    }
}
