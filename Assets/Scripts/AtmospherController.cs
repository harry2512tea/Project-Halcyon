using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmospherController : MonoBehaviour
{
    public List<DoorController> Doors;

    GameObject room;
    public compartmentController compartment;
    float pressure;
    public float roomSize;
    public float airInRoom;
    bool pressurising = false;
    bool depressurising = false;
    bool venting = false;

    float pressurisation_Rate = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    private void FixedUpdate()
    {
        pressure = airInRoom / roomSize;
        if (depressurising)
        {
            if (airInRoom - pressurisation_Rate < 0)
            {
                depressurising = false;
                airInRoom = 0;
            }
        }
        else if (pressurising)
        {
            if (airInRoom + pressurisation_Rate > roomSize)
            {
                pressurising = false;
                airInRoom = roomSize;
            }

        }
        else if (venting)
        {
            if (airInRoom - pressurisation_Rate < 0)
            {
                depressurising = false;
                airInRoom = 0;
            }
        }
    }
    public void pressurise()
    {

    }

    public void depressurise()
    {

    }

    public void vent()
    {
        if(!venting)
        {
            venting = true;
        }
    }

    public float getPressure()
    {
        return pressure;
    }
    public void stop()
    {
        depressurising = false;
        pressurising = false;
        venting = false;
    }
}
