using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingPortV2 : MonoBehaviour
{

    public delegate void Docking(DockingData data);
    public static event Docking unDock;

    bool isDocked = false;

    [SerializeField]
    CompartmentControllerV2 thisCompartment;
    public CompartmentControllerV2 compartment {  get { return thisCompartment; } }

    [SerializeField]
    Transform parentModule;

    [SerializeField]
    DockingPortV2 dockedPort;

    BoxCollider trigger;

    [SerializeField]
    DoorControllerV2 DoorController;
    public DoorControllerV2 Door { get { return DoorController; } }


    public Transform module
    {
        get { return parentModule; }
    }

    public bool docked
    {
        get { return isDocked; }
    }

    public Vector3 dockingCamPosition;

    private void Awake()
    {
        trigger = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        CompartmentControllerV2.OnDock += OnDock;
        //CompartmentControllerV2.OnUndock += OnUnDock;
    }

    private void OnDisable()
    {
        CompartmentControllerV2.OnDock -= OnDock;
        //CompartmentControllerV2.OnUndock -= OnUnDock;
    }

    void OnDock(DockingData data)
    { 
        if(data.TargetPort == this || data.CallerPort == this)
        {
            isDocked = true;
            if(data.TargetPort != this)
            { dockedPort = data.TargetPort; }
            else
            { dockedPort = data.CallerPort; }
            trigger.isTrigger = false;
            trigger.enabled = false;
        }
    }

    void OnUnDock(DockingData data)
    {
        if (data.TargetPort == this || data.CallerPort == this)
        {
            isDocked = false;
        }
    }

    public void Undock()
    {
        DockingData data = new DockingData();
        data.CallerPort = this;
        data.TargetPort = dockedPort;
        data.Target = dockedPort.compartment;
        data.Caller = thisCompartment;
        data.StationTarget = thisCompartment.getStationController();

        trigger.enabled = true;
        trigger.isTrigger = true;

        unDock(data);

    }

}
