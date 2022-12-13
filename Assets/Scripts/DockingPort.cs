using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingPort : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject ThisShip;
    public int portID = -1;
    bool available = true;
    public DoorController attachedDoor;
    private void Awake()
    {
        ThisShip = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getID() { return portID; }
    public void setID(int _portID) { portID = _portID; }
    public void setAvailable(bool _available) { available = _available; }
    public bool getAvailable() { return available; }
}
