using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CelestialBody2 : MonoBehaviour
{
    [SerializeField]
    bool isStationary = false;

    [SerializeField]
    GameObject orbitingBody;

    CelestialBody2 orbitingBodyController;

    [SerializeField]
    public Transform ascNode, incl, pArg, position, model, moons;

    [SerializeField]
    float scaleDivision = 1;

    //radius in KM
    [SerializeField]
    double radius;

    //semiMajAxis in KM
    [SerializeField]
    double eccentricity, semiMajAxis, orbitalHeight;
    [SerializeField]
    float orbitalPosition, inclination, periapsisArg, ascendingNode;

    //mass in tons. multiply by to get kg 907.2
    [SerializeField]
    double orbitalPeriod, mass;

    const double G = 6.6743e-11;

    [SerializeField]
    double u;

    double timeSincePeriapsis = 0.0;

    //orbital period in seconds
    double T;

    double meanMotion;

    private void Awake()
    {
        //setting the size of the object to a scale version.
        float tempR = (float)(radius / scaleDivision);
        model.localScale = new Vector3(tempR, tempR, tempR);

        //checking if the object is orbiting another body.
        if (!isStationary)
        {
            orbitingBodyController = orbitingBody.GetComponent<CelestialBody2>();
            //setting up the rotations for the celestial body
            ascNode.localRotation = Quaternion.Euler(0.0f, ascendingNode, 0.0f);
            incl.localRotation = Quaternion.Euler(inclination, 0.0f, 0.0f);
            pArg.localRotation = Quaternion.Euler(0.0f, periapsisArg, 0.0f);
            position.localRotation = Quaternion.Euler(0.0f, orbitalPosition, 0.0f);
            
            //semi-minor axis (km)
            double b = semiMajAxis * Math.Sqrt(1 - (eccentricity * eccentricity));

            //orbital period calculation in seconds
            //semiMajor Axis in meters
            double a = semiMajAxis * 1000;
            Debug.Log(a);
            double aCubed = (a * a * a);
            Debug.Log(aCubed);

            double twoPi = Math.PI * 2;

            double orbitingBodyMass = orbitingBodyController.getMass() * 1000;
            Debug.Log(orbitingBodyMass);

            double GM = G * orbitingBodyMass;

            T = twoPi * Math.Sqrt(aCubed / GM);

            Debug.Log((float)T);

            meanMotion = 2 * Mathf.PI / T;

            //orbital period in days
            double T_days = (double)(T / 86400);
            orbitalPeriod = T_days;
        }
    }

    private void Update()
    {
        if (!isStationary)
        {
            orbitalHeight = 0;
            timeSincePeriapsis += Time.deltaTime;
            if (timeSincePeriapsis > T)
            {
                timeSincePeriapsis -= T;
            }

            //Orbital Position Calculations
            orbitalPosition = (float)calculateTrueAnomaly(eccentricity, meanMotion * timeSincePeriapsis);

            //orbital height calculation in meters
            orbitalHeight = (semiMajAxis * 1000 * (1 - (eccentricity * eccentricity))) / (1 + eccentricity * Math.Cos(orbitalPosition));


            //updating the position of the planet in the scene
            position.localEulerAngles = new Vector3(0.0f, (float)orbitalPosition, 0.0f);
            model.localPosition = new Vector3(0.0f, 0.0f, (float)(orbitalHeight / 1000) / scaleDivision);
            model.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

            moons.localPosition = new Vector3(0.0f, 0.0f, (float)(orbitalHeight / 1000) / scaleDivision);
            moons.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }

    double calculateTrueAnomaly(double e, double M)
    {
        double E = M;

        for(int i = 0; i < 5; i++)
        {
            E -= E - e * Math.Sin(E) - M / (1 - e * Math.Cos(E));
        }

        double sinE = Math.Sin(E);
        double cosE = Math.Cos(E);
        double sqrtFactor = Math.Sqrt((1 + e) / (1 - e));

        double trueAnomalyRadians = 2 * Math.Atan2(sqrtFactor * sinE, cosE);

        return Mathf.Rad2Deg * trueAnomalyRadians;
    }

    double getMass() { return mass; }
}
