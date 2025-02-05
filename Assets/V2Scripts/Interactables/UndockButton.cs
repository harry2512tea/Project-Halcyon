using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndockButton : InteractableBase
{
    [SerializeField]
    DoorControllerV2 controller;

    public override void Interact(GameObject caller)
    {
        controller.undockToggle();
    }
}
