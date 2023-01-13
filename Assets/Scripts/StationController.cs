using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour
{
    public List<compartmentController> compartments = new List<compartmentController>();
    public List<GameObject> modules = new List<GameObject>();
    public int dockedModules;
    public float totalStoredAir, pressurisationRate;
    float totalPowerGen, totalPowerCons, totalPowerStored;

    private void Awake()
    {
        modules.Add(gameObject);
        compartments.Add(GetComponent<compartmentController>());
        dockedModules = 0;
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

        if (dockedModules < 1)
        {
            Debug.Log("Destroying station controller");
            Destroy(this);
        }
        
    }
    public void pressuriseModule(compartmentController controller)
    {
        if(totalStoredAir > 0)
        {
            for (int I = 0; I < compartments.Count; I++)
            {
                if (compartments[I].storedAir > 0)
                {
                    if (compartments[I].storedAir - pressurisationRate <= 0)
                    {
                        controller.volumeInRoom += compartments[I].storedAir;
                        compartments[I].storedAir = 0;
                    }
                    compartments[I].storedAir -= pressurisationRate;
                }
                else
                {
                }
            }
        }
    }

    public void depressuriseModule(compartmentController controller)
    {
    }

    public void addModule(GameObject module)
    {
        modules.Add(module);
        compartments.Add(module.GetComponent<compartmentController>());
        dockedModules++;
    }

    public void removeModule(GameObject module)
    {
        modules.Remove(module);
        compartments.Remove(module.GetComponent<compartmentController>());
        dockedModules--;
    }
}