using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(RCS)), RequireComponent(typeof(SphereCollider))]
public class CompartmentControllerV2 : MonoBehaviour
{
    //when this compartment (un)docks from other compartments/ships
    public delegate void Docking(DockingData data);
    public static event Docking OnDock;
    public static event Docking OnUndock;

    //when another compartment/ship (un)docks from this compartment
    public delegate void CompartmentDocking(CompartmentDockingData data);
    public static event CompartmentDocking OnCompartmentDock;
    public static event CompartmentDocking OnCompartmentUndock;

    public delegate void Standing(seatingData data);
    public static event Standing OnCancelControlling;

    StationControllerV2 thisController;

    public StationControllerV2 mainController;

    //list of generators on this module e.g. solar panel, reactor, air generator
    [SerializeField]
    List<GeneratorBase> generators = new List<GeneratorBase>();

    //list of subsystems on this module e.g. RCS propulsion, Refinery, fabricator and base consumption
    [SerializeField]
    List<SubSystemBase> subSystems = new List<SubSystemBase>();

    RCS rcs;
    SphereCollider portDetector;

    //Compartments and/or ships docked to this compartment and/or ship.
    [SerializeField]
    List<CompartmentControllerV2> childCompartments = new List<CompartmentControllerV2>();

    //Docking ports of the module
    [SerializeField]
    List<DockingPortV2> dockingPorts = new List<DockingPortV2>();

    [SerializeField]
    List<DockingPortV2> availablePorts = new List<DockingPortV2>();

    [SerializeField]
    List<DockingPortV2> targetPorts = new List<DockingPortV2>();

    [SerializeField]
    Camera dockingCam;

    //Resources Stored
    float nitrogenFuel, airInTank;
    //Max resources
    [SerializeField]
    float maxNitrogenFuel, maxAirInTank;

    bool centralNode = true;

    bool docked = false;
    bool canDock = false;

    bool controlled = false;
    InteractionControllerV2 controlledBy;

    int Targetport = 0, Currentport = 0;

    InputSystem_Actions input;

    Rigidbody body;

    private void Awake()
    {

        dockingCam.enabled = false;
        thisController = GetComponent<StationControllerV2>();
        mainController = thisController;

        rcs = GetComponent<RCS>();
        portDetector = GetComponent<SphereCollider>();

        input = new InputSystem_Actions();

        body = GetComponent<Rigidbody>();

        Transform ports = transform.Find("DockingPorts");

        for (int i = 0; i < ports.childCount; i++)
        {
            dockingPorts.Add(ports.GetChild(i).GetComponent<DockingPortV2>());
        }
    }

    private void Update()
    {
        if (controlled)
        {
            docking();
        }
    }

    private void OnEnable()
    {
        input.Enable();

        CompartmentControllerV2.OnCompartmentDock += OnDockedTo;
        CompartmentControllerV2.OnDock += dockEvent;
        //CompartmentControllerV2.OnCompartmentUndock += OnUndockedFrom;

        //InteractionControllerV2.OnSeated += OnSeated;
        //InteractionControllerV2.OnStanding += OnStanding;
        DockingSystemPanel.OnSeated += OnSeated;

        DockingPortV2.unDock += UnDock;

        input.Modules.Cancel.performed += OnCancelDockingPerformed;
        input.Modules.TogglePort.performed += CyclePort;
        input.Modules.ToggleTarget.performed += CycleTarget;
    }

    private void OnDisable()
    {
        input.Disable();

        CompartmentControllerV2.OnCompartmentDock -= OnDockedTo;
        CompartmentControllerV2.OnDock -= dockEvent;
        //CompartmentControllerV2.OnCompartmentUndock -= OnUndockedFrom;

        DockingSystemPanel.OnSeated -= OnSeated;

        DockingPortV2.unDock -= UnDock;

        input.Modules.Cancel.performed -= OnCancelDockingPerformed;
        input.Modules.TogglePort.performed -= CyclePort;
        input.Modules.ToggleTarget.performed -= CycleTarget;

        //InteractionControllerV2.OnSeated += OnSeated;
        //InteractionControllerV2.OnStanding += OnStanding;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("triggerEnter");
        switch (other.tag)
        {
            case "Module":
                //CompartmentControllerV2 temp = other.GetComponent<CompartmentControllerV2>();
                //for(int I = 0; I < dockingPorts.Count; I++)
                //{
                //    if (!dockingPorts[I].docked)
                //    {
                //        temp.AddTargetPort(dockingPorts[I]);
                //    }
                //}

                if (controlled)
                {
                    CompartmentControllerV2 temp = other.GetComponent<CompartmentControllerV2>();
                    for (int I = 0; I < temp.GetDockingPorts().Count; I++)
                    {
                        if (!temp.GetDockingPorts()[I].docked)
                        { targetPorts.Add(temp.GetDockingPorts()[I]); }
                    }
                }
                break;
            case "DockingPort":
                if (canDock && other.gameObject == targetPorts[Targetport].gameObject)
                {
                    Dock();
                }
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("Trigger Stay");
        //Debug.Log(other.name);

        switch (other.tag)
        {
            case "DockingPort":
                if (canDock && other.gameObject == targetPorts[Targetport].gameObject)
                {
                    Dock();
                }
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Module")
        {
            CompartmentControllerV2 temp = other.GetComponent<CompartmentControllerV2>();
            for (int I = 0; I < dockingPorts.Count; I++)
            {
                if (!dockingPorts[I].docked)
                {
                    temp.RemoveTargetPort(dockingPorts[I]);
                }
            }
        }

    }

    void OnSeated(seatingData data)
    {
        if (data.Target == gameObject)
        {
            controlled = true;
            controlledBy = data.Caller;
            rcs.activate();
            portDetector.enabled = true;
            Targetport = 0;
            Currentport = 0;
            rcs.UpdateReferenceFrame(dockingPorts[Currentport].transform);
            canDock = false;

            UpdateAvailablePorts();

            dockingCam.transform.parent = dockingPorts[Currentport].transform;
            dockingCam.transform.localPosition = Vector3.zero;
            dockingCam.transform.localEulerAngles = Vector3.zero;
            dockingCam.enabled = true;
        }
    }

    void OnStanding(seatingData data)
    {
        if (controlled && data.Caller == controlledBy)
        {
            controlled = false;
            controlledBy = null;
            rcs.deactivate();
            rcs.UpdateReferenceFrame(transform);
            portDetector.enabled = false;

            UpdateAvailablePorts();

            dockingCam.transform.parent = transform;
            dockingCam.transform.localPosition = Vector3.zero;
            dockingCam.transform.localEulerAngles = Vector3.zero;
            dockingCam.enabled = true;
        }
    }

    //called when another compartment docks to this one
    void OnDockedTo(CompartmentDockingData _data)
    {
        if (_data.Target == this)
        {
            childCompartments.Add(_data.Caller);
            docked = true;
        }
    }

    void dockEvent(DockingData _data)
    {
        if(_data.Target == this)
        {
            childCompartments.Add(_data.Caller);
        }
    }

    void Dock()
    {
        //Debug.Log("Dock");
        DockingData data = new DockingData();
        data.Caller = this;
        data.CallerPort = dockingPorts[Currentport];
        data.TargetPort = targetPorts[Targetport];
        data.Target = data.TargetPort.module.gameObject.GetComponent<CompartmentControllerV2>();
        data.StationTarget = data.Target.getStationController();


        body.isKinematic = true;
        transform.parent = data.Target.transform;
        //body.velocity = Vector3.zero;
        transform.rotation *= Quaternion.FromToRotation(data.CallerPort.transform.up, data.TargetPort.transform.up);
        transform.rotation *= Quaternion.FromToRotation(data.CallerPort.transform.forward, -data.TargetPort.transform.forward);

        //transform.localPosition = data.TargetPort.transform.localPosition + -data.CallerPort.transform.localPosition;
        float absOffset = Mathf.Abs(data.TargetPort.transform.localPosition.magnitude) + Mathf.Abs(data.CallerPort.transform.localPosition.magnitude);
        Vector3 offsetDir = data.TargetPort.transform.localPosition.normalized;

        Vector3 offset = offsetDir * absOffset;

        transform.localPosition = offset;

        centralNode = false;
        thisController.enabled = false;
        OnDock(data);

        portDetector.enabled = false;

        docked = true;
        canDock = false;
        
    }

    void UnDock(DockingData _data)
    {
        if (_data.Target == this)
        {
            undockTarget(_data);
        }
        else if(_data.Caller == this)
        {
            undockCaller(_data);
        }

        if (childCompartments.Count == 0)
        {
            docked = false;
        }
    }

    void undockTarget(DockingData _data)
    {
        if (centralNode)
        {
            childCompartments.Remove(_data.Caller);
        }
        else
        {
            if (childCompartments.Contains(_data.Caller))
            {
                childCompartments.Remove(_data.Caller);
            }
            else
            {
                thisController.enabled = true;
                mainController = thisController;
                transform.parent = null;
                body.isKinematic = false;
                Vector3 direction = transform.position - _data.Caller.transform.position;
            }
        }

    }

    void undockCaller(DockingData _data)
    {
        if (centralNode)
        {
            childCompartments.Remove(_data.Target);
        }
        else
        {
            if (childCompartments.Contains(_data.Target))
            {
                childCompartments.Remove(_data.Target);

            }
            else
            {
                thisController.enabled = true;
                mainController = thisController;
                transform.parent = null;
                body.isKinematic = false;
            }
        }
    }

    public bool canControl()
    {
        return !docked;
    }

    void docking()
    { 
        
        if(targetPorts.Count > 0 && !docked)
        {
            Transform current = dockingPorts[Currentport].transform;
            Transform target = targetPorts[Targetport].transform;

            float pitchYaw = Vector3.Angle(current.TransformDirection(Vector3.forward), target.TransformDirection(Vector3.back));
            float roll = Vector3.SignedAngle(current.TransformDirection(Vector3.up), target.TransformDirection(Vector3.up), current.TransformDirection(Vector3.forward));

            //Debug.Log("pitchYaw = " + pitchYaw);
            //Debug.Log("roll = " +  roll);

            if(roll < 3 && (pitchYaw < 4 || pitchYaw > 176))
            {
                canDock = true;
            }
            else
            {
                canDock = false;
            }
        }
    }

    void OnCancelDockingPerformed(InputAction.CallbackContext _value)
    {
        if (controlled)
        {
            Debug.Log("cancelling");
            controlled = false;

            seatingData data = new seatingData();
            data.Target = controlledBy.gameObject;


            OnCancelControlling(data);

            controlled = false;
            controlledBy = null;
            rcs.deactivate();
            rcs.UpdateReferenceFrame(transform);
            portDetector.enabled = false;
        }
    }

    public void AddTargetPort(DockingPortV2 port)
    { targetPorts.Add(port); }

    public void RemoveTargetPort(DockingPortV2 port)
    { targetPorts.Remove(port); }

    public List<GeneratorBase> getGenerators() { return generators; }
    public List<SubSystemBase> GetSubSystems() { return subSystems; }
    public List<CompartmentControllerV2> getChildCompartments() { return childCompartments; }
    public StationControllerV2 getStationController() { return mainController; }
    public List<DockingPortV2> GetDockingPorts() { return dockingPorts; }

    void CycleTarget(InputAction.CallbackContext _value)
    {
        //Debug.Log("Current Target: " + Targetport);
        if (Targetport + 1 < targetPorts.Count)
        {
            Targetport++;
        }
        else
        {
            Targetport = 0; 
        }
        //Debug.Log("Current Target: " + Targetport);
    }
    void CyclePort(InputAction.CallbackContext _value)
    {
        //Debug.Log("Current Port: " + Currentport);
        if (Currentport + 1 < dockingPorts.Count)
        {
            Currentport++;
            rcs.UpdateReferenceFrame(dockingPorts[Currentport].transform);
        }
        else
        {
            Currentport = 0;
            rcs.UpdateReferenceFrame(dockingPorts[Currentport].transform);
        }

        dockingCam.transform.parent = dockingPorts[Currentport].transform;
        dockingCam.transform.localPosition = Vector3.zero;
        dockingCam.transform.localEulerAngles = Vector3.zero;

        //Debug.Log("Current Port: " + Currentport);
    }
    void UpdateAvailablePorts()
    { 
        availablePorts.Clear();
        for(int i = 0; i < dockingPorts.Count; i++)
        {
            if(!dockingPorts[i].docked)
            {
                availablePorts.Add(dockingPorts[i]);
            }
        }
    }
}
