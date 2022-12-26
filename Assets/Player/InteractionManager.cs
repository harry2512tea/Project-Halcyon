using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public Camera cam;
    public float range;
    public LayerMask ignore;
    bool detected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit = interactionDetection();
        if (Input.GetKeyDown(KeyCode.F) && detected)
        {
            hit.collider.gameObject.GetComponent<Interactable>().Interact();
        }
    }

    RaycastHit interactionDetection()
    {
        RaycastHit hit;

        if(Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, range, ~ignore))
        {
            detected = true;
        }
        else
        {
            detected = false;
        }
        return hit;
    }
}
