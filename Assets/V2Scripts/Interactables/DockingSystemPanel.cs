using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingSystemPanel : InteractableBase
{
    public delegate void Seating(seatingData data);
    public static event Seating OnSeated;

    [SerializeField]
    CompartmentControllerV2 controller;

    private void Awake()
    {
        //controller = GetComponent<CompartmentControllerV2>();
    }

    public override void Interact(GameObject caller)
    {
        //Debug.Log("Docking Panel");
        if (controller.canControl())
        {
            seatingData data = new seatingData();
            data.Caller = caller.GetComponent<InteractionControllerV2>();
            data.Target = controller.gameObject;
            data.seatObject = gameObject;
            OnSeated(data);
        }
    }
}
