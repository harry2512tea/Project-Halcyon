using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOverride : Interactable
{
    public GameObject attachedDoor;

    DoorController controller;

    public override void Interact(GameObject player)
    {
        controller.Interact();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = attachedDoor.GetComponent<DoorController>();
    }
}