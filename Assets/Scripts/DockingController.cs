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
                    Debug.Log(other.transform.localPosition);
                    Debug.Log(dockingPorts[activeDockingPort].transform.localPosition);
                    body.isKinematic = true;

                    float absOffset = Mathf.Abs(other.transform.localPosition.magnitude) + Mathf.Abs(dockingPorts[activeDockingPort].transform.localPosition.magnitude);

                    Vector3 offsetDir = other.transform.localPosition.normalized;

                    Vector3 newPosition = absOffset * offsetDir;
                    
                    Debug.Log(other.GetComponent<DockingPort>().getID());
                    //Time.timeScale = 0;
                    other.GetComponent<DockingPort>().setAvailable(false);
                    
                    Destroy(body);
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
