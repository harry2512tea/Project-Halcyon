using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

//public class DockingData
//{
//    public CompartmentControllerV2 Caller;
//    public StationControllerV2 Target;
//}

public class DockingData
{
    public CompartmentControllerV2 Caller;
    public DockingPortV2 CallerPort;
    public DockingPortV2 TargetPort;
    public CompartmentControllerV2 Target;
    public StationControllerV2 StationTarget;
}

public class CompartmentDockingData
{
    public CompartmentControllerV2 Caller;
    public CompartmentControllerV2 Target;
}

public class TestEventData
{
    public ListTestMain Target;
    public ListTestModule Caller;
}

public class seatingData
{
    public GameObject Target;
    public InteractionControllerV2 Caller;
    public GameObject seatObject;
}

public class interactionData
{
    public InteractionControllerV2 Caller;
}
