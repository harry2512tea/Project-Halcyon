using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingOverride : Interactable
{
    public DockingPort connectedPort;
    public DockingController controller;
    public override void Interact()
    {
        int portID = connectedPort.getID();
        controller.Undock(portID);
    }
}
