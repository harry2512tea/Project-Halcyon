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
    Vector3 camRot;
    Rigidbody body;
    public bool grounded;
    public bool magBoots = false;
    public LayerMask ignore;
    CapsuleCollider col;
    Vector3 prevUp;
    bool lockmouse = true;

    public float Xsensitivity, Ysensitivity, rollThrust, Thrust, walkSpeed, runSpeed, jumpForce, stabilisationForce;
    public GameObject cam;

    float rotX, rotY, totalRotY;
    void Start()
    {
        //cam = Camera.main.gameObject;
        body = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
        //transform.localScale = new Vector3(1.0f, 0.5f, 1.0f);
        DoMovement();
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!magBoots)
            {
                magBoots = true;
            }
            else
            {
                magBoots = false;
            }
        }
        Grounded();
        switch (moveType)
        {
            case movementType.ZeroG:
                
                if (grounded && magBoots)
                {
                    Mag();
                }
                else
                {
                    zeroG();
                }

                break;
            case movementType.Grav:
                Grav();
                break;
            case movementType.mag:
                Mag();
                break;
        }
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(lockmouse)
            {
                lockmouse = false;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                lockmouse = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    void DoMovement()
    {
        body.angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("UpDown"), Input.GetAxis("Vertical"));
    }

    Vector3 DoRotation()
    {
        Vector3 rotate;
        rotate.x = -(Input.GetAxis("Mouse Y") * Xsensitivity);
        rotate.y = Input.GetAxis("Mouse X") * Ysensitivity;
        rotate.z = Input.GetAxis("Roll") * rollThrust;
        rotY = Input.GetAxis("Mouse X") * Ysensitivity;
        rotX += -Input.GetAxis("Mouse Y") * Xsensitivity;
        
        return rotate;
    }

    void zeroG()
    {
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        body.drag = 0.0f;
        cam.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        body.constraints = RigidbodyConstraints.None;
        body.AddForce(transform.TransformDirection(movement) * Thrust * Time.deltaTime, ForceMode.VelocityChange);
        rotationSpeed += DoRotation();
        transform.Rotate(rotationSpeed * Time.deltaTime);

        if (Input.GetButton("Stabilise"))
        {
            Stabilise();
        }
    }

    

    void Grav()
    {
        Grounded();
        if (!grounded)
        {
        }
        else
        {
            body.velocity = movement * walkSpeed;
        }
    }

    void Mag()
    {
        RaycastHit hit = magGrounded();
        if (grounded)
        {
            transform.parent = hit.collider.transform;
            body.drag = 5.0f;
            rotationSpeed = new Vector3(0.0f, 0.0f, 0.0f);
            
            body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            Vector3 up = hit.normal;

            DoRotation();
            if (up != prevUp)
            {
                cam.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                transform.localRotation = Quaternion.FromToRotation(Vector3.up, up);
                transform.RotateAround(transform.position, up, totalRotY);
                prevUp = up;
            }

            transform.RotateAround(transform.position, up, rotY);
            totalRotY += rotY;
            rotX = Mathf.Clamp(rotX, -80f, 80f);
            cam.transform.localEulerAngles = new Vector3(rotX, 0.0f, 0.0f);



            DoMovement();
            if(!Input.GetKey(KeyCode.LeftShift))
            {
                body.AddForce(transform.TransformDirection(movement) * walkSpeed * Time.deltaTime, ForceMode.VelocityChange);
            }
            else
            {
                body.AddForce(transform.TransformDirection(movement) * runSpeed * Time.deltaTime, ForceMode.VelocityChange);
            }
            if (Input.GetAxis("UpDown") > 0.7f)
            {
                magBoots = false;
                transform.parent = null;
                body.drag = 0.0f;
                body.AddForce(new Vector3(0.0f, Input.GetAxis("UpDown") * jumpForce, 0.0f));
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
        else
        {
            transform.parent = null;
            body.drag = 0.0f;
        }
    }

    RaycastHit magGrounded()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, col.radius, transform.TransformDirection(Vector3.down), out hit, (col.height / 2) + 0.05f, ~ignore))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        return hit;
    }
    RaycastHit Grounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, (col.height/2) + 0.1f, ~ignore))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        return hit;
    }
    void Stabilise()
    {
        rotationSpeed.x = Mathf.MoveTowards(rotationSpeed.x, 0f, stabilisationForce);
        rotationSpeed.y = Mathf.MoveTowards(rotationSpeed.y, 0f, stabilisationForce);
        rotationSpeed.z = Mathf.MoveTowards(rotationSpeed.z, 0f, stabilisationForce);
    }
}