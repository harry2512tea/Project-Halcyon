using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum movementType
{
    ZeroG,
    Grav,
    mag
}

public class PlayerController : MonoBehaviour
{
    movementType moveType = movementType.ZeroG;
    Vector3 rotationSpeed;
    Vector3 movement;
    Rigidbody body;
    bool grounded;
    public LayerMask ignore;
    
    
    public float sensitivity, rollThrust, Thrust, movementSpeed;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        DoMovement();
        switch(moveType)
        {
            case movementType.ZeroG:
                zeroG();
                break;
            case movementType.Grav:
                Grav();
                break;
            case movementType.mag:
                Mag();
                break;
        }
    }

    void DoMovement()
    {
        movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("UpDown"), Input.GetAxis("Vertical"));
    }

    Vector3 DoRotation()
    {
        Vector3 rotate;
        rotate.x = -(Input.GetAxis("Mouse Y") * sensitivity);
        rotate.y = Input.GetAxis("Mouse X") * sensitivity;
        rotate.z = Input.GetAxis("Roll") * rollThrust;

        return rotate;
    }

    void zeroG()
    {
        body.AddForce(movement * Thrust * Time.deltaTime, ForceMode.VelocityChange);
        rotationSpeed += DoRotation();
        transform.Rotate(rotationSpeed * Time.deltaTime);

        if(Input.GetButton("Stabilise"))
        {
            Stabilise();
        }
    }

    void Grav()
    {
        Grounded();
        if(!grounded)
        {
        }
        else
        {
            body.velocity = movement * movementSpeed;
        }
    }

    void Mag()
    {

    }

    void Grounded()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), 0.5f, ~ignore))
        {
            grounded = true;
        }
        grounded = false;
    }
    void Stabilise()
    {
        rotationSpeed.x = Mathf.MoveTowards(rotationSpeed.x, 0f, 0.2f);
        rotationSpeed.y = Mathf.MoveTowards(rotationSpeed.y, 0f, 0.2f);
        rotationSpeed.z = Mathf.MoveTowards(rotationSpeed.z, 0f, 0.2f);
    }
}
