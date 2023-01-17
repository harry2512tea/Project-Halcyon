using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static PlayerOrbit;


public class CelestialBody : MonoBehaviour
{
    public bool isStationary = false;
    public PlayerOrbit orbitInfo;
    public GameObject orbitingBody;
    CelestialBody orbitingBodyController;
    public Transform ascNode, incl, pArg, position, model;
    public float scaleMultiplication;
    public float radius;
    public float orbitalPosition, inclination, periapsisArg, ascendingNode;
    public float eccentricity, semiMajAxis;
    public double orbitalPeriod;
    public double mass;
    double G = 0.000000000066743;

    private void Awake()
    {
        G = 6.6743e-11;
        if (!isStationary)
        {
            orbitingBodyController = orbitingBody.GetComponent<CelestialBody>();
            ascNode.localRotation = Quaternion.Euler(0.0f, ascendingNode, 0.0f);
            incl.localRotation = Quaternion.Euler(inclination, 0.0f, 0.0f);
            pArg.localRotation = Quaternion.Euler(0.0f, periapsisArg, 0.0f);
            position.localRotation = Quaternion.Euler(0.0f, orbitalPosition, 0.0f);

            double B = Mathf.Sqrt(semiMajAxis * 1000 * semiMajAxis * 1000 * (1 - (eccentricity * eccentricity)));
            double R = ((semiMajAxis * 1000 + B) / 2) / 149600000; // in AU
            double M = mass * 907.2; // mass in Kg.
            double V = (4 / 3 * Math.PI) * Math.Pow(radius * 1000, 3);
            double K = (4 * Math.PI) / (G * (M + (orbitingBodyController.mass * 907.2)));
            double density = M / V;
            Debug.Log(gameObject.name + "Radius: "  + R);
            //orbitalPeriod = Math.Sqrt((3 * Math.PI) / (G * density));
            //orbitalPeriod = Mathf.Pow((2 * Mathf.PI * Mathf.Pow(R * 1000, 3 / 2)) / (Mathf.Sqrt((float)G) * Mathf.Sqrt((float)orbitingBodyController.mass * 1000)), 3);
            //double temp = (4 * Math.PI * Math.PI) / (G * orbitingBodyController.mass * 907.2);
            //orbitalPeriod = Math.Sqrt(temp * Math.Pow(R * 1000, 3));
            orbitalPeriod = Math.Sqrt((R * R * R) / (orbitingBodyController.mass * 907.2/1.989e+30)); /// Math.Pow(149600000, 3)));
        }
        //mass /= 1000;
    }
    private void Update()
    {
        if(!isStationary)
        {
            double distance = (semiMajAxis * 1000 * (1 - (eccentricity * eccentricity))) / (1 + eccentricity * Mathf.Cos((orbitalPosition * Mathf.PI/180)));
            double temp = (float)G * (float)orbitingBodyController.mass * ((2 / (float)distance) - (1 / (semiMajAxis * 1000)));
            double V = Math.Sqrt(temp);
            float angularVel = (float)(V / distance);
            Debug.Log(gameObject.name + "Angular Vel: " + angularVel);
            if (orbitalPosition + angularVel* Time.deltaTime < 360)
            {
                orbitalPosition += angularVel * Time.deltaTime;
            }
            else
            {
                orbitalPosition += angularVel * Time.deltaTime;
                orbitalPosition -= 360;
            }
            position.localEulerAngles = new Vector3(0.0f, orbitalPosition, 0.0f);
            model.localPosition = new Vector3(0.0f, 0.0f, (float)distance/1000);
            //Debug.Log(distance);
            //Debug.Log(temp);
            //Debug.Log(V);
            //Debug.Log(angularVel);
        }
    }
}
