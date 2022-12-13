using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingController : MonoBehaviour
{
    
    public bool dockingMode = false;
    public List<GameObject> dockingPorts = new List<GameObject>();
    public List<DockingPort> portControllers = new List<DockingPort>();
    public int activeDockingPort;
    bool canDock = true;
    Rigidbody body;
    DockingPort port;
    // Start is called before the first frame update
    void Awake()
    {
        body = GetComponent<Rigidbody>();
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
                    portControllers[activeDockingPort].attachedDoor.Dock(port.attachedDoor);

                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(dockingMode)
        {
            if(Input.GetKeyDown(KeyCode.W))
            {
                body.AddForce(new Vector3(-3.0f, 0.0f, 0.0f), ForceMode.Impulse);
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                Undock();
            }
        }
    }

    void Undock()
    {
        Vector3 parentPos = transform.parent.position;
        StartCoroutine("Dockingcooldown");

        transform.parent = null;

        body = gameObject.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.AddForce((transform.position - parentPos).normalized, ForceMode.Impulse);
        portControllers[activeDockingPort].attachedDoor.unDock();
        port.attachedDoor.Dock(portControllers[activeDockingPort].attachedDoor);

    }

    IEnumerator Dockingcooldown()
    {
        canDock = false;
        yield return new WaitForSeconds(1.0f);
        canDock = true;
    }
}
