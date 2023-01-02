using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingPort : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject ThisShip;
    public int portID = -1;
    bool available = true;
    public bool isChild = false;
    public GameObject docked, stationComponent;
    public DoorController attachedDoor;
    public Vector3 alignmentVector;
    public Camera portCam;
    private void Awake()
    {
        portCam.enabled = false;
        ThisShip = transform.parent.gameObject;
    }

    public int getID() { return portID; }
    public void setID(int _portID) { portID = _portID; }
    public void setAvailable(bool _available) { available = _available; }
    public bool getAvailable() { return available; }
}
