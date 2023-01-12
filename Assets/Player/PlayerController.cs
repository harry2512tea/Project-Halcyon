using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum movementType
{
    ZeroG,
    Grav,
    mag,
    controlling
}

public class PlayerController : MonoBehaviour
{
    movementType moveType = movementType.ZeroG;
    Vector3 rotationSpeed;
    Vector3 movement;
    Vector3 playerScale;
    Rigidbody body;
    public bool grounded;
    public bool magBoots = false;
    public bool controlling = false;
    public LayerMask ignore;
    CapsuleCollider col;
    Vector3 prevUp;
    bool lockmouse = true;
    Vector3 groundedHit;

    public float Xsensitivity, Ysensitivity, rollThrust, Thrust, walkSpeed, runSpeed, jumpForce, stabilisationForce;
    public GameObject cam;

    float rotX, rotY, totalRotY;

    //statistic collection
    List<float> positionCorrection = new List<float>();

    void Start()
    {
        body = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;
        playerScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    // Update is called once per frame
    private void Update()
    {
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (lockmouse)
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
    void FixedUpdate()
    {
        
        if (controlling)
        {
            moveType = movementType.controlling;
        }
        else
        {
            moveType = movementType.ZeroG;
            DoMovement();
            if(!cam.activeSelf)
            {
                cam.SetActive(true);
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

            case movementType.controlling:
                cam.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                if(cam.activeSelf)
                {
                    cam.SetActive(false);
                }
                break;
        }

        if (grounded && magBoots)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1.1f, ~ignore);
            Vector3 yPos = transform.parent.InverseTransformPoint(hit.point) + transform.parent.InverseTransformDirection(hit.normal);
            Vector3 newPos = new Vector3(transform.localPosition.x, yPos.y, transform.localPosition.z);
            //Debug.Log("current position: " + transform.localPosition);
            //Debug.Log("local hit point: " + transform.parent.InverseTransformPoint(hit.point));
            //Debug.Log("local up: " + transform.parent.InverseTransformDirection(hit.normal));
            //Debug.Log("new position: " + newPos);
            transform.localPosition = newPos;
            //Debug.Log("Final position: " + transform.localPosition.y);
            if(positionCorrection.Count < 1000)
            {
                positionCorrection.Add(transform.localPosition.y);
            }
            else
            {
                double val = 0;
                for(int i = 0; i < positionCorrection.Count; i++)
                {
                    val += positionCorrection[i];
                }
                val /= positionCorrection.Count;
                Debug.Log(val);
            }
        }
    }

    private void LateUpdate()
    {
        //if (grounded && magBoots)
        //{
        //    RaycastHit hit;
        //    Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1.1f, ~ignore);
        //    Vector3 yPos = transform.parent.InverseTransformPoint(hit.point) + transform.InverseTransformDirection(new Vector3(0.0f, 1.0f, 0.0f));
        //    Vector3 newPos = new Vector3(transform.localPosition.x, yPos.y, transform.localPosition.z);
        //    Debug.Log("current position: " + transform.localPosition);
        //    Debug.Log("local hit point: " + transform.parent.InverseTransformPoint(hit.point));
        //    Debug.Log("new position: " + newPos);
        //    transform.localPosition = newPos;
        //}
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
        if(transform.parent)
        {
            transform.parent = null;
            transform.localScale = playerScale;
        }
        if(prevUp != new Vector3(0.0f, 0.0f))
        {
            prevUp = new Vector3(0.0f, 0.0f);
        }

        body.drag = 0.0f;
        cam.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        body.constraints = RigidbodyConstraints.None;
        body.AddForce(transform.TransformDirection(movement) * Thrust * Time.fixedDeltaTime, ForceMode.VelocityChange);
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
            //Debug.Log(transform.localPosition);
            //Debug.Log("Global " + hit.normal);
            //Debug.Log("Local " + hit.collider.transform.InverseTransformDirection(hit.normal));
            //transform.localPosition = transform.InverseTransformPoint(hit.point) + new Vector3(transform.localPosition.x, col.height / 2, transform.localPosition.z);
            Vector3 up = hit.collider.transform.InverseTransformDirection(hit.normal);
            DoRotation();
            if (transform.parent != hit.collider.transform)
            {
                Debug.Log("new parent");
                transform.parent = hit.collider.transform;
                Vector3 floorScale = transform.parent.lossyScale;
                transform.localScale = new Vector3(playerScale.x / floorScale.x, playerScale.y / floorScale.y, playerScale.z / floorScale.z);
                body.drag = 5.0f;
                rotationSpeed = new Vector3(0.0f, 0.0f, 0.0f);

                body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

                
                if (up != prevUp)
                {
                    float Y = transform.localEulerAngles.y;
                    Debug.Log("setting rotation");
                    cam.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    transform.RotateAround(transform.position, hit.normal, Y);
                    prevUp = up;
                }
            }

            transform.RotateAround(transform.position, hit.normal, rotY);
            totalRotY += rotY;
            rotX = Mathf.Clamp(rotX, -80f, 80f);
            cam.transform.localEulerAngles = new Vector3(rotX, 0.0f, 0.0f);

            DoMovement();
            if(!Input.GetKey(KeyCode.LeftShift))
            {
                body.AddForce(transform.TransformDirection(movement) * walkSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
            else
            {
                body.AddForce(transform.TransformDirection(movement) * runSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }

            if (Input.GetAxis("UpDown") > 0.7f)
            {
                magBoots = false;
                transform.parent = null;
                body.drag = 0.0f;
                body.AddForce(new Vector3(0.0f, Input.GetAxis("UpDown") * jumpForce, 0.0f));
                transform.localScale = playerScale;
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
        bool ground = Physics.SphereCast(transform.position, col.radius, transform.TransformDirection(Vector3.down), out hit, 1.05f, ~ignore);
        if (transform.parent)
        {
            if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1.1f, ~ignore))
            {
                groundedHit = hit.point;
                //Debug.Log(transform.InverseTransformPoint(hit.point));
                grounded = true;
                //Debug.Log("1 " + hit.collider.name);
                //Debug.Log("Grounded 1");
            }
            else
            {
                //Debug.Log("not grounded 1");
                grounded = false;
            }
        }
        else
        {
            if (ground)
            {
                //Debug.Log("2 " + hit.collider.name);
                //Debug.Log("Grounded 2");
                grounded = true;
            }
            else
            {
                //Debug.Log("not grounded 2");
                grounded = false;
            }
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

    public void addRigidBody()
    {
        if(!body)
        {
            body = gameObject.AddComponent<Rigidbody>();
            body.useGravity = false;
        }
    }

    public void deleteRigidBody()
    {
        if (body)
        {
            Destroy(body);
        }
    }
}