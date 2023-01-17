using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "OrbitInfo")]
public class PlayerOrbit : ScriptableObject
{
    public float orbitalPosition, inclination, periapsisArg, ascendingNode;
    public float eccentricity, semiMajAxis;
    public double orbitalPeriod;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
