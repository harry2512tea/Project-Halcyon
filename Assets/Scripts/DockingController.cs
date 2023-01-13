using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingController : MonoBehaviour
{
    public bool dockingMode = false;
    public List<GameObject> dockingPorts = new List<GameObject>();
    public List<DockingPort> portControllers = new List<DockingPort>();
    public DockingPanel panel;
    public int activeDockingPort;
    public float mass;
    public bool canDock = true;
    public bool docked;
    Rigidbody body;
    DockingPort port;
    public GameObject stationCore;
    public StationController stationMaster;
    Vector3 movement, rotationSpeed;
    public float Xsensitivity, Ysensitivity, rollThrust, RCSThrust, stabilisationForce;
    compartmentController thisCompartment;

    void Awake()
    {
        thisCompartment = GetComponent<compartmentController>();
        body = GetComponent<Rigidbody>();
        if(dockingMode)
        {
            body.AddForce(transform.TransformDirection(Vector3.right) * 10, ForceMode.Impulse);
        }
        int ID = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "DockingPort")
            {
                foreach (Transform port in child)
                {
                    if (port.gameObject.tag == "DockingPort" && port != child)
                    {
                        dockingPorts.Add(port.gameObject);
                        portControllers.Add(port.GetComponent<DockingPort>());
                        portControllers[ID].setID(ID);
                        ID++;
                    }
                }
                break;
            }
        }

        activeDockingPort = 0;
    }

    private void OnTriggerEnter(Collider other)
    {

        //Debug.Log("Enter Trigger");
        switch (other.tag)
        {
            case "DockingPort":
                if(dockingMode && canDock)
                {
                    
                    DockingPort port = other.gameObject.GetComponent<DockingPort>();
                    //Debug.Log("checking angles");

                    float pitchYaw = Vector3.Angle(transform.TransformDirection(portControllers[activeDockingPort].portAxis), other.transform.TransformDirection(port.alignmentVector));
                    float roll = Vector3.SignedAngle(other.transform.TransformDirection(Vector3.up), portControllers[activeDockingPort].transform.TransformDirection(Vector3.up), other.transform.TransformDirection(port.portAxis));
                    //Debug.Log("PitchYaw: " + pitchYaw);
                    //Debug.Log("Roll: " + roll);
                    if(roll < 4 && (pitchYaw < 4 || pitchYaw > 176))
                    {
                        //Debug.Log("Modules Aligned");
                        dock(other);
                    }

                }
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            case "DockingPort":
                if (dockingMode && canDock)
                {
                    DockingPort port = other.gameObject.GetComponent<DockingPort>();


                    float pitchYaw = Vector3.Angle(transform.TransformDirection(portControllers[activeDockingPort].portAxis), other.transform.TransformDirection(port.alignmentVector));
                    float roll = Vector3.SignedAngle(other.transform.TransformDirection(Vector3.up), portControllers[activeDockingPort].transform.TransformDirection(Vector3.up), other.transform.TransformDirection(port.portAxis));

                    if (roll < 4 && (pitchYaw < 4 || pitchYaw > 176))
                    {
                        //Debug.Log("Modules Aligned");
                        dock(other);
                    }
                }
                break;
        }
    }
    private void Update()
    {
        bool isDocked = false;
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            rotationSpeed += new Vector3(1.0f, 0.0f);
        }
        foreach (DockingPort port in portControllers)
        {
            if(port.docked)
            {
                isDocked = true;
            }
        }
        if(isDocked)
        {docked = true;}
        else
        {docked = false;}

        if(dockingMode && !docked)
        {
            doMovement();
            if (Input.GetKeyDown(KeyCode.R))
            {
                Camera prevCam = portControllers[activeDockingPort].portCam;
                activeDockingPort++;
                
                if(activeDockingPort == dockingPorts.Count)
                {
                    activeDockingPort = 0;
                }
                prevCam.enabled = false;
                portControllers[activeDockingPort].portCam.enabled = true;
            }
        }

        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
        
    }

    void doMovement()
    {
        getMovement();

        body.drag = 0.0f;
        body.constraints = RigidbodyConstraints.None;
        body.AddForce(portControllers[activeDockingPort].portCam.transform.TransformDirection(movement) * RCSThrust * Time.deltaTime, ForceMode.VelocityChange);
        rotationSpeed += (portControllers[activeDockingPort].portCam.transform.TransformVector(DoRotation()));
        

        if (Input.GetButton("Stabilise"))
        {
            Stabilise();
        }
    }

    void getMovement()
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

        return rotate;
    }

    void Stabilise()
    {
        rotationSpeed.x = Mathf.MoveTowards(rotationSpeed.x, 0f, stabilisationForce);
        rotationSpeed.y = Mathf.MoveTowards(rotationSpeed.y, 0f, stabilisationForce);
        rotationSpeed.z = Mathf.MoveTowards(rotationSpeed.z, 0f, stabilisationForce);
    }

    void dock(Collider other)
    {
        //Debug.Log("Docking");
        canDock = false;
        body.isKinematic = true;
        rotationSpeed = new Vector3(0.0f, 0.0f, 0.0f);
        float absOffset = Mathf.Abs(other.transform.localPosition.magnitude) + Mathf.Abs(dockingPorts[activeDockingPort].transform.localPosition.magnitude);

        Vector3 offsetDir = other.transform.localPosition.normalized;

        Vector3 newPosition = absOffset * offsetDir;

        port = other.GetComponent<DockingPort>();
        port.setAvailable(false);

        Destroy(body);
        transform.parent = other.transform.parent.parent;
        transform.localPosition = newPosition;
        

        port.attachedDoor.Dock(portControllers[activeDockingPort].attachedDoor);
        port.stationComponent = gameObject;
        portControllers[activeDockingPort].attachedDoor.Dock(port.attachedDoor);
        portControllers[activeDockingPort].docked = other.gameObject;
        portControllers[activeDockingPort].stationComponent = other.gameObject.transform.parent.parent.gameObject;
        portControllers[activeDockingPort].isChild = true;
        port.docked = dockingPorts[activeDockingPort];
        Vector3 dir = other.transform.TransformDirection(port.alignmentVector);
        Quaternion newRot = Quaternion.FromToRotation(portControllers[activeDockingPort].portAxis, dir);
        transform.rotation = newRot;
        transform.localEulerAngles = new Vector3(0.0f, transform.localEulerAngles.y, 0.0f);

        GameObject otherModule = other.transform.parent.parent.gameObject;
        DockingController otherController = other.transform.parent.parent.gameObject.GetComponent<DockingController>();
        if (otherController.stationMaster)
        {
            Debug.Log("station master exists");
            stationCore = otherController.stationCore;
            stationMaster = otherController.stationMaster;
        }
        else
        {
            Debug.Log("adding station master");
            Debug.Log(otherModule);
            stationCore = otherModule;
            stationMaster = otherModule.AddComponent<StationController>();
            Debug.Log(stationMaster);
            otherController.stationCore = stationCore;
            otherController.stationMaster = stationMaster;
        }
        otherModule.GetComponent<compartmentController>().mainController = stationMaster;
        stationMaster.addModule(gameObject);
        
    }
    public void Undock(int ID)
    {
        StartCoroutine("Dockingcooldown");
        if (portControllers[ID].isChild)
        {
            Vector3 parentPos = transform.parent.position;
            transform.parent = null;
            addRigidBody();
            body.AddForce((transform.position - parentPos).normalized * 0.3f, ForceMode.VelocityChange);
            portControllers[ID].docked.GetComponent<DockingPort>().attachedDoor.unDock();
            portControllers[ID].attachedDoor.unDock();
        }
        else
        {
            GameObject obj = portControllers[ID].stationComponent;
            Vector3 childPos = obj.transform.position;
            obj.transform.parent = null;
            DockingController temp = obj.GetComponent<DockingController>();
            body.AddForce((transform.position - childPos).normalized * 0.3f, ForceMode.VelocityChange);
            portControllers[ID].docked.GetComponent<DockingPort>().attachedDoor.unDock();
            portControllers[ID].attachedDoor.unDock();
            temp.addRigidBody();

        }
    }

    public void enableDockingMode()
    {
        dockingMode = true;
        Debug.Log(activeDockingPort);
        portControllers[activeDockingPort].portCam.enabled = true;
    }
    public void disableDockingMode()
    {
        dockingMode = false;
        portControllers[activeDockingPort].portCam.enabled = false;
    }

    IEnumerator Dockingcooldown()
    {
        yield return new WaitForSeconds(1.0f);
        canDock = true;
    }
    
    public Rigidbody addRigidBody()
    {
        body = gameObject.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.mass = mass;
        return body;
    }
}