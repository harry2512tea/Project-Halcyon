using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingController : MonoBehaviour
{
    
    public bool dockingMode = false;
    public List<GameObject> dockingPorts = new List<GameObject>();
    public List<DockingPort> portControllers = new List<DockingPort>();
    public int activeDockingPort;
    Rigidbody body;
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
                if(dockingMode)
                {
                    body.isKinematic = true;
                    Debug.Log(other.GetComponent<DockingPort>().getID());
                    other.GetComponent<DockingPort>().setAvailable(false);
                    Vector3 newPosition = other.transform.localPosition + dockingPorts[activeDockingPort].transform.localPosition;
                    transform.parent = other.transform.parent;
                    transform.localPosition = newPosition;
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
        }
    }
}
