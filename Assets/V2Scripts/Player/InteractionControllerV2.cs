using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InteractionControllerV2 : MonoBehaviour
{
    public delegate void Seating(seatingData data);
    public static event Seating OnSeated;
    public static event Seating OnStanding;

    InputSystem_Actions input;

    [SerializeField]
    Camera cam;

    [SerializeField]
    float range;

    [SerializeField]
    LayerMask select;

    Ray ray = new Ray();
    RaycastHit hit;

    private void Awake()
    {
        input = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Interact.performed += interact;
    }

    private void OnDisable()
    {
        input.Disable();

        input.Player.Interact.performed -= interact;
    }

    void interact(InputAction.CallbackContext _value)
    {
        Debug.Log("Interact");
        ray.origin = cam.transform.position;
        ray.direction = cam.transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(ray, out hit, range, select))
        {
            //Debug.Log(hit.collider.gameObject.name);
            hit.collider.gameObject.GetComponent<InteractableBase>().Interact(gameObject);
        }
    }

}
