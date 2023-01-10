using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class compartmentController : MonoBehaviour
{
    public float storedAir, maxStoredAir, airQuality, totalAirQuality, roomVolume, volumeInRoom, pressure;
    public float powerConsumption, powerGeneration, powerStored, maxPowerStored;
    public float forceMultiplier;
    public bool on;
    public StationController mainController;
    List<DoorController> doors = new List<DoorController>();
    List<GameObject> physicsObjects = new List<GameObject>();
    List<Rigidbody> physicsBodies = new List<Rigidbody>();

    private void Awake()
    {
        storedAir = 0;
        airQuality = roomVolume;
        powerStored = 0;
        on = true;
    }

    private void FixedUpdate()
    {
        pressure = volumeInRoom / roomVolume;

        for(int I = 0; I < doors.Count; I++)
        {
            if (doors[I].open)
            {
                if (doors[I].linked)
                {
                    if(doors[I].attachedRoom.pressure < pressure)
                    {
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
