using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class compartmentController : MonoBehaviour
{
    public float storedAir, maxStoredAir;
    public float powerConsumption, powerGeneration, powerStored, maxPowerStored;
    public bool on;
    StationController mainController;
}
