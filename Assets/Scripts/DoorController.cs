using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool open = false;
    public compartmentController attachedRoom;

    public DoorController linkedDoor;
    public bool linked = false;
    public GameObject leftDoor, rightDoor;
    public Vector3 leftOpenPosition, leftClosedPosition, rightOpenPosition, rightClosedPosition;
    public float movementSpeed;

    // Update is called once per frame
    void Update()
    {
        if (open == true)
        {
            leftDoor.transform.localPosition = Vector3.Lerp(leftDoor.transform.localPosition, leftOpenPosition, movementSpeed * Time.deltaTime);
            rightDoor.transform.localPosition = Vector3.Lerp(rightDoor.transform.localPosition, rightOpenPosition, movementSpeed * Time.deltaTime);
        }
        else
        {
            leftDoor.transform.localPosition = Vector3.Lerp(leftDoor.transform.localPosition, leftClosedPosition, movementSpeed * Time.deltaTime);
            rightDoor.transform.localPosition = Vector3.Lerp(rightDoor.transform.localPosition, rightClosedPosition, movementSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(linked && other.tag == "Player" && !open)
        {
            Open();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (linked && other.tag == "Player" && open)
        {
            Close();
        }
    }

    private void FixedUpdate()
    {
    }

    public void Open()
    {
        if(linked)
        {
            if(linkedDoor.open == false)
            {
                linkedDoor.open = true;
            }
        }
        open = true;
    }
    public void Close()
    {
        if (linked)
        {
            if (linkedDoor.open == true)
            {
                linkedDoor.open = false;
            }
        }
        open = false;
    }
    public void Dock(DoorController door)
    {
        linked = true;
        linkedDoor = door;
    }

    public void unDock()
    {
        linked = false;
        linkedDoor = null;
        open = false;
    }

    public void Interact()
    {
        
        if(open)
        {
            Close();
        }
        else
        {
            Open();
        }

        
    }
}
