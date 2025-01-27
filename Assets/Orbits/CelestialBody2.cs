using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CelestialBody2 : MonoBehaviour
{
    [SerializeField]
    bool isStationary = false;

    public bool stableOrbit = true;

    [SerializeField]
    GameObject orbitingBody;

    CelestialBody2 orbitingBodyController;

    [SerializeField]
    Transform ascNode, incl, pArg, position, modelPos, model, moons, velocityCalc;

    [SerializeField]
    float scaleDivision = 1;

    //radius in KM
    [SerializeField]
    double radius;

    //semiMajAxis in KM
    [SerializeField]
    float orbitalPosition, inclination, periapsisArg, ascendingNode;
    [SerializeField]
    double eccentricity, semiMajAxis, semiMinAxis, orbitalHeight, orbitalSpeed;

    [SerializeField]
    Vector3 orbitalVelocity;

    

    //mass in tons. multiply by to get kg 907.2
    [SerializeField]
    double orbitalPeriod, mass;

    //Day Length in hours;
    [SerializeField]
    float dayLength;

    const double G = 6.6743e-11;

    [SerializeField]
    double GM, u;

    double timeSincePeriapsis = 0.0;

    //orbital period in seconds
    double T;

    double meanMotion;

    float degreesPerSecond;

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
            semiMinAxis = b;
            //orbital period calculation in seconds
            //semiMajor Axis in meters
            double a = semiMajAxis * 1000;

            double aCubed = (a * a * a);

            double twoPi = Math.PI * 2;

            double orbitingBodyMass = orbitingBodyController.getMass() * 1000;

            GM = G * orbitingBodyMass;

            T = twoPi * Math.Sqrt(aCubed / GM);

            meanMotion = 2 * Mathf.PI / T;

            //orbital period in days
            double T_days = (double)(T / 86400);
            orbitalPeriod = T_days;
        }

        degreesPerSecond = 360 / (dayLength * 3600f);
        
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

            //Orbital Position Calculations. position in degrees
            orbitalPosition = (float)calculateTrueAnomaly(eccentricity, meanMotion * timeSincePeriapsis);

            //orbital height calculation in meters
            orbitalHeight = (semiMajAxis * 1000 * (1 - (eccentricity * eccentricity))) / (1 + eccentricity * Math.Cos(orbitalPosition));

            calculateOrbitalVelocity();

            //updating the position of the planet in the scene
            position.localEulerAngles = new Vector3(0.0f, (float)orbitalPosition, 0.0f);
            modelPos.localPosition = new Vector3(0.0f, 0.0f, (float)(orbitalHeight / 1000) / scaleDivision);
            modelPos.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

            moons.localPosition = new Vector3(0.0f, 0.0f, (float)(orbitalHeight / 1000) / scaleDivision);
            moons.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            
            
        }
        if (model.localEulerAngles.y + degreesPerSecond * Time.deltaTime < 360)
        {
            model.localEulerAngles += new Vector3(0.0f, degreesPerSecond, 0.0f) * Time.deltaTime;
        }
        else
        {
            float newY = (model.localEulerAngles.y + (degreesPerSecond * Time.deltaTime)) - 360;
            model.localEulerAngles += new Vector3(0.0f, newY, 0.0f);
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

    double calculateOrbitalVelocity()
    {
        //orbital speed in m/s
        u = G * (orbitingBodyController.getMass() * 1000);
        orbitalSpeed = Math.Sqrt(u * ((2/(orbitalHeight)) - (1/(semiMajAxis * 1000))));

        float nextPos = (float)calculateTrueAnomaly(eccentricity, meanMotion * (timeSincePeriapsis + 3600));

        velocityCalc.localEulerAngles = new Vector3(0.0f, nextPos, 0.0f);

        float nextHeight = (float)((float)(semiMajAxis * 1000 * (1 - (eccentricity * eccentricity))) / (1 + eccentricity * Math.Cos(nextPos)));

        Vector3 pos = velocityCalc.TransformPoint(Vector3.forward * ((nextHeight / 1000) / scaleDivision));

        Vector3 direction = (pos - position.localPosition);

        orbitalVelocity = direction.normalized * (float)orbitalSpeed;

        return 0;
    }

    double getMass() { return mass; }
    double getRadius() { return radius; }
}