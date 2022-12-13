using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour
{
    List<compartmentController> compartments;

    float totalAir;
    private void Update()
    {
        totalAir = 0;
        foreach (compartmentController comp in compartments)
        {
            totalAir += comp.GetComponent<AtmospherController>().airInRoom;
        }
    }
    public void addCompartment(compartmentController controller)
    {
        compartments.Add(controller);
    }
}

