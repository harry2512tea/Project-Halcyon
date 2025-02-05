using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorControllerV2 : MonoBehaviour
{
    Animator animator;
    bool Open = false;
    bool isUndock = false;
    bool docked = false;
    //bool triggerEnabled = true;

    public bool Undock { get {  return isUndock; } }

    DoorControllerV2 DockedDoor;
    [SerializeField]
    CompartmentControllerV2 CompartmentController;

    [SerializeField]
    DockingPortV2 port;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        CompartmentControllerV2.OnDock += OnDock;
        DockingPortV2.unDock += OnUndock;
    }

    private void OnDisable()
    {
        CompartmentControllerV2.OnDock -= OnDock;
        DockingPortV2.unDock -= OnUndock;
    }

    void OnDock(DockingData data)
    {
        
        if (data.CallerPort == port)
        {
            DockedDoor = data.TargetPort.Door;
            docked = true;
            close();
        }
        else if (data.TargetPort == port)
        {
            DockedDoor = data.CallerPort.Door;
            docked = true;
            close();
        }
    }

    void OnUndock(DockingData data)
    {
        if (data.CallerPort == port || data.TargetPort == port)
        {
            docked = false;
            DockedDoor = null;
            close();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //Debug.Log("TriggerEnter Player");
            if (docked && !Open)
            {
                open();
                DockedDoor.open(); 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            close();
            //Debug.Log("TriggerExit Player");
            if (docked)
            {
                
                DockedDoor.close(); 
            }
        }
    }

    public void toggleOverride()
    {
        //Debug.Log("override");
        if(Open)
        {
            //Debug.Log("close");
            close();
            if (docked)
            { DockedDoor.close(); }
        }
        else
        {
            //Debug.Log("Open");
            open();
            if (docked)
            { DockedDoor.open(); }
        }

        
    }

    public void close()
    {
        Open = false;
        animator.SetTrigger("Close");
        animator.ResetTrigger("open");
    }

    public void open()
    {

        Open = true;
        animator.ResetTrigger("Close");
        animator.SetTrigger("Open");
        
    }

    public void undockToggle()
    {
        isUndock = !isUndock;
        if (docked)
        {
            if (isUndock && DockedDoor.Undock)
            {
                port.Undock();
            }
        }
        
    }

    public CompartmentControllerV2 getController() { return CompartmentController; }

    
}
