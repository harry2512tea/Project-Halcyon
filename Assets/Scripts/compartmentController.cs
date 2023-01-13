using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class compartmentController : MonoBehaviour
{
    public float storedAir, maxStoredAir, airQuality, totalAirQuality, roomVolume, volumeInRoom, pressure;
    public float powerConsumption, powerGeneration, powerStored, maxPowerStored, basePowerConsumption;
    public float forceMultiplier;
    public bool on;
    public StationController mainController;
    DockingController dockingCont;
    List<DoorController> doors = new List<DoorController>();
    List<GameObject> physicsObjects = new List<GameObject>();
    List<Rigidbody> physicsBodies = new List<Rigidbody>();

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.tag == "Door")
            {
                doors.Add(child.gameObject.GetComponent<DoorController>());
            }
        }

        dockingCont = GetComponent<DockingController>();
        storedAir = 0;
        airQuality = roomVolume;
        powerStored = 0;
        on = true;
    }

    private void FixedUpdate()
    {
        pressure = volumeInRoom / roomVolume;
        Debug.Log(gameObject.name + "attached doors count: " + doors.Count);
        for(int I = 0; I < doors.Count; I++)
        {
            if (doors[I].open)
            {
                Debug.Log(gameObject.name + "Open Door");
                if (doors[I].linked)
                {
                    if(doors[I].attachedRoom.pressure < pressure)
                    {
                        Debug.Log(gameObject.name + "Pressure difference");
                        float pressureDiff = pressure - doors[I].attachedRoom.pressure;
                        float totalVolume = roomVolume + doors[I].attachedRoom.roomVolume;
                        float totalAir = volumeInRoom + doors[I].attachedRoom.volumeInRoom;
                        float desiredPressure = totalAir / totalVolume;
                        volumeInRoom = Mathf.Round(roomVolume * desiredPressure);
                        doors[I].attachedRoom.volumeInRoom = Mathf.Round(doors[I].attachedRoom.roomVolume * desiredPressure);
                        if(pressureDiff > 0.2f)
                        {
                            for(int X = 0; X < physicsObjects.Count; X++)
                            {
                                physicsBodies[X].AddForce((doors[I].transform.position - physicsObjects[X].transform.position).normalized * pressureDiff * forceMultiplier, ForceMode.Impulse);
                            }
                        }
                    }
                }
                else
                {
                    volumeInRoom = 0;
                    for (int X = 0; X < physicsObjects.Count; X++)
                    {
                        physicsBodies[X].AddForce((doors[I].transform.position - physicsObjects[X].transform.position).normalized * forceMultiplier, ForceMode.Impulse);
                    }
                }
            }
        }
    }
}
