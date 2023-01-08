using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour
{
    List<compartmentController> compartments;
    List<GameObject> modules;

    float totalStoredAir;
    float totalPowerGen, totalPowerCons, totalPowerStored;

    private void Awake()
    {
        modules.Add(gameObject);
        compartments.Add(GetComponent<compartmentController>());
    }

    private void FixedUpdate()
    {
        totalStoredAir = 0;
        totalPowerStored = 0;
        totalPowerGen = 0;
        totalPowerCons = 0;
        foreach (compartmentController comp in compartments)
        {
            totalStoredAir += comp.storedAir;
            if(comp.on)
            {
                totalPowerCons += comp.powerConsumption;
            }
            totalPowerGen += comp.powerGeneration;
            totalPowerStored += comp.powerStored;
        }
    }
    public void addModule(GameObject module)
    {
        modules.Add(module);
        compartments.Add(module.GetComponent<compartmentController>());
    }

    public void removeModule(GameObject module)
    {
        modules.Remove(module);
        compartments.Remove(module.GetComponent<compartmentController>());
    }
}