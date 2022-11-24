using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum moveType
{
    ShipGravity,
    Gravity,
    Mag,
    ZeroG
};

public class PlayerMovement : MonoBehaviour
{
    public float crouchHeight, walkSpeed, runSpeed, RCSThrust, RollThrust, sensitivity, maxRotation;
    public Vector3 velocity, movement, gravMovement, rotation;
    bool Grounded;
    public Rigidbody body, shipBody;
    private float rotY, rotX;
    private Camera cam;
    bool controlling;
    int gravityColliders, floorColliders;
    public LayerMask ignore, ignoreFloor;
    moveType move;
    GameObject ship, planet;

    private void Start()
    {
        cam = Camera.main;
        body = GetComponent<Rigidbody>();
        Grounded = false;
        move = moveType.ZeroG;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "ShipGravity")
        {
            if(!controlling)
            {
                gravityColliders++;
            }
            if(gravityColliders == 1)
            {
                body.constraints = RigidbodyConstraints.FreezeRotation;
                transform.parent = other.transform.parent.parent;
                transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, 0f);
                shipBody = other.transform.parent.parent.gameObject.GetComponent<Rigidbody>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(!controlling)
        {
            if(other.tag == "ShipGravity")
            {
                gravityColliders--;
            }
            if(gravityColliders == 0)
            {
                body.constraints = RigidbodyConstraints.None;
                transform.parent = null;
                try
                {
                    rotation += other.transform.parent.parent.gameObject.GetComponent<ShipMovement>().GetShipRotation();
                }
                catch { }
                shipBody = null;
                Grounded = false;
            }
        }
    }
    private void Update()
    {
        MovementController();
    }

    private void MovementController()
    {
        movement = GetMovement();
        switch (move)
        {
            case moveType.ShipGravity:
                if (ship.GetComponent<ShipMovement>().UnderThrust())
                {
                    ShipGrav();
                }
                else
                {
                    ZeroGrav();
                }
                break;

            case moveType.Gravity:
                Grav();
                break;

            case moveType.Mag:
                if(Grounded)
                {
                    Mag();
                }
                else
                {
                    ZeroGrav();
                }
                break;

            case moveType.ZeroG:
                ZeroGrav();
                break;
        }
    }


    private Vector3 GetMovement()
    {
        return new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("UpDown"), Input.GetAxis("Vertical"));
    }

    Vector3 GetRotation()
    {
        Vector3 rotate;
        rotate.x = -(Input.GetAxis("Mouse Y") * sensitivity);
        rotate.y = Input.GetAxis("Mouse X") * sensitivity;
        rotate.z = Input.GetAxis("Roll") * RollThrust;
        return rotate;
    }

    float ClampRotation(float angle)
    {
        return Mathf.Clamp(angle * Time.deltaTime, -maxRotation, maxRotation);
    }

    void Stabilise()
    {
        rotation.x = Mathf.MoveTowards(rotation.x, 0f, 0.2f);
        rotation.y = Mathf.MoveTowards(rotation.y, 0f, 0.2f);
        rotation.z = Mathf.MoveTowards(rotation.z, 0f, 0.2f);
    }

    void Mag()
    {

    }
    void Grav()
    {

    }

    void ShipGrav()
    {
        body.centerOfMass = transform.InverseTransformPoint(shipBody.transform.TransformPoint(shipBody.centerOfMass));
        rotation = new Vector3(0f, 0f, 0f);

        if(Grounded)
        {
            gravMovement = new Vector3(movement.x, 0f, movement.z);
            if(Input.GetKey(KeyCode.LeftShift))
            {
                body.velocity = transform.TransformDirection(gravMovement) * runSpeed;
            }
            else
            {
                body.velocity = transform.TransformDirection(gravMovement) * walkSpeed;
            }
        }
        else
        {
            body.AddForce(transform.TransformDirection(new Vector3(0f, -ship.GetComponent<ShipMovement>().GetAcceleration() * Time.deltaTime, 0f)), ForceMode.VelocityChange);
        }
    }

    void ZeroGrav()
    {
        movement = transform.TransformDirection(movement) * RCSThrust * Time.deltaTime;
        body.AddForce(movement, ForceMode.VelocityChange);

        Vector3 rot = GetRotation();
        rotation += new Vector3(ClampRotation(rot.x), ClampRotation(rot.y), ClampRotation(rot.z));

        if (Input.GetButton("Stabilise"))
        {
            Stabilise();
        }

        transform.Rotate(rotation * Time.deltaTime, Space.Self);
    }
}
