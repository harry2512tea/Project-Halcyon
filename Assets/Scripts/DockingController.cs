using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingController : MonoBehaviour
{
    
    public bool dockingMode = false;
    public List<GameObject> dockingPorts = new List<GameObject>();
    public List<DockingPort> portControllers = new List<DockingPort>();
    public int activeDockingPort;
    public float mass;
    bool canDock = true;
    Rigidbody body;
    DockingPort port;
    // Start is called before the first frame update
    void Awake()
    {
        
        body = GetComponent<Rigidbody>();
        if (dockingMode)
        {
            body.AddForce(new Vector3(-0.2f, 0.0f, 0.0f), ForceMode.VelocityChange);
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

        //debugging code
        activeDockingPort = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "DockingPort":
                if(dockingMode && canDock)
                {
                    canDock = false;
                    body.isKinematic = true;

                    float absOffset = Mathf.Abs(other.transform.localPosition.magnitude) + Mathf.Abs(dockingPorts[activeDockingPort].transform.localPosition.magnitude);

                    Vector3 offsetDir = other.transform.localPosition.normalized;

                    Vector3 newPosition = absOffset * offsetDir;
                    
                    //Time.timeScale = 0;
                    port = other.GetComponent<DockingPort>();
                    port.setAvailable(false);
                    
                    Destroy(body);
                    transform.parent = other.transform.parent;
                    transform.localPosition = newPosition;

                    port.attachedDoor.Dock(portControllers[activeDockingPort].attachedDoor);
                    port.stationComponent = gameObject;
                    portControllers[activeDockingPort].attachedDoor.Dock(port.attachedDoor);
                    portControllers[activeDockingPort].docked = other.gameObject;
                    portControllers[activeDockingPort].stationComponent = other.gameObject.transform.parent.parent.gameObject;
                    portControllers[activeDockingPort].isChild = true;
                    port.docked = dockingPorts[activeDockingPort];

                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(dockingMode)
        {
            //if(Input.GetKeyDown(KeyCode.W) && canDock)
            //{
            //    body.AddForce(new Vector3(-3.0f, 0.0f, 0.0f), ForceMode.Impulse);
            //}

            if (Input.GetKeyDown(KeyCode.U))
            {
                //Undock();
            }
        }
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
            Rigidbody temp = obj.GetComponent<DockingController>().addRigidBody();
            body.AddForce((transform.position - childPos).normalized * 0.3f, ForceMode.VelocityChange);
            portControllers[ID].docked.GetComponent<DockingPort>().attachedDoor.unDock();
            portControllers[ID].attachedDoor.unDock();

        }
        //StartCoroutine("Dockingcooldown");
        //if(transform.parent.parent == portControllers[ID].docked.transform.parent.parent)
        //{
        //    Vector3 parentPos = transform.parent.position;
        //    transform.parent = null;
        //    body = gameObject.AddComponent<Rigidbody>();
        //    body.useGravity = false;
        //    body.mass = mass;
        //    body.AddForce((transform.position - parentPos).normalized * 0.3f, ForceMode.VelocityChange);
        //    portControllers[ID].docked.GetComponent<DockingPort>().attachedDoor.unDock();
        //    portControllers[ID].attachedDoor.unDock();
        //}
        //else
        //{
        //    portControllers[ID].docked.transform.parent.parent = null;
        //    Vector3 parentPos = transform.position;
        //    body.AddForce((transform.parent.position - parentPos).normalized * 0.3f, ForceMode.VelocityChange);
        //    portControllers[ID].docked.transform.parent.parent.gameObject.GetComponent<DockingController>().addRigidBody();
        //    portControllers[ID].docked.GetComponent<DockingPort>().attachedDoor.unDock();
        //    portControllers[ID].attachedDoor.unDock();
        //}
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
