using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingPortV2 : MonoBehaviour
{

    bool isDocked = false;

    [SerializeField]
    Transform parentModule;

    public Transform module
    {
        get { return parentModule; }
    }

    public bool docked
    {
        get { return isDocked; }
    }

    public Vector3 dockingCamPosition;

    private void OnEnable()
    {
        CompartmentControllerV2.OnDock += OnDock;
        CompartmentControllerV2.OnUndock += OnUnDock;
    }

    private void OnDisable()
    {
        CompartmentControllerV2.OnDock -= OnDock;
        CompartmentControllerV2.OnUndock -= OnUnDock;
    }

    void OnDock(DockingData data)
    { 
        if(data.TargetPort == this || data.CallerPort == this)
        {
            isDocked = true;
        }
    }

    void OnUnDock(DockingData data)
    {
        if (data.TargetPort == this || data.CallerPort == this)
        {
            isDocked = false;
        }
    }

}
