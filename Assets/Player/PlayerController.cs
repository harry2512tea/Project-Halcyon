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


    public float Xsensitivity, Ysensitivity, rollThrust, Thrust, walkSpeed, runSpeed, jumpForce, stabilisationForce;
    public GameObject cam;

    bool wasGrounded;

    float rotX, rotY, totalRotY;
    // Start is called before the first frame update
    void Start()
    {
        //cam = Camera.main.gameObject;
        body = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        DoMovement();
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!magBoots)
            {
                magBoots = true;
                //moveType = movementType.mag;
            }
            else
            {
                //moveType = movementType.ZeroG;
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
        Debug.Log("ZeroG");
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
            body.drag = 5.0f;
            rotationSpeed = new Vector3(0.0f, 0.0f, 0.0f);
            
            body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            Vector3 up = hit.normal;
            //Debug.Log(hit.normal);
            Debug.Log(transform.InverseTransformVector(transform.localEulerAngles));
            DoRotation();
            if (up != prevUp)
            {
                
                transform.localRotation = Quaternion.FromToRotation(Vector3.up, up);
                //transform.RotateAround(transform.position, up, rotY);
                transform.RotateAround(transform.position, up, totalRotY);
                prevUp = up;
            }

            //body.AddForce(transform.TransformDirection(Vector3.down) * Time.deltaTime, ForceMode.VelocityChange);

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
                body.AddForce(new Vector3(0.0f, Input.GetAxis("UpDown") * jumpForce, 0.0f));
            }
        }
        else
        {
            body.drag = 0.0f;
        }
    }

    RaycastHit magGrounded()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, col.radius, transform.TransformDirection(Vector3.down), out hit, (col.height / 2) + 0.05f, ~ignore))
        {
            if(grounded)
            {
                wasGrounded = true;
            }
            else
            {
                wasGrounded = false; ;
            }
            grounded = true;
        }
        else
        {
            if (grounded)
            {
                wasGrounded = true;
            }
            else
            {
                wasGrounded = false; ;
            }
            grounded = false;

        }
        return hit;
    }
    RaycastHit Grounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, (col.height/2) + 0.1f, ~ignore))
        {
            if(grounded)
                {
                    wasGrounded = true;
                }
            else
                {
                    wasGrounded = false;
                }
            grounded = true;
        }
        else
        {
            grounded = false;
            wasGrounded = false;
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