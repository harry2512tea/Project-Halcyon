using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCSThrusterController : MonoBehaviour
{
    [SerializeField]
    RCS controller;

    CompartmentControllerV2 compartment;

    [SerializeField]
    Vector3 movementVectorFiring, rotationVectorFiring;

    ParticleSystem particles;

    Vector3 absMoveVec, absRotVec;
    private void OnEnable()
    {
        RCS.thrust += thrust;
    }

    private void OnDisable()
    {
        RCS.thrust -= thrust;
    }

    private void Awake()
    {
        absMoveVec = new Vector3(Mathf.Abs(movementVectorFiring.x), Mathf.Abs(movementVectorFiring.y), Mathf.Abs(movementVectorFiring.z));
        absRotVec = new Vector3(Mathf.Abs(rotationVectorFiring.x), Mathf.Abs(rotationVectorFiring.y), Mathf.Abs(rotationVectorFiring.z));
        particles = GetComponent<ParticleSystem>();
        
    }

    void thrust(ThrusterFiring _data)
    {
        if (_data.RCSCaller == controller /*|| _data.StationCaller == compartment.getStationController()*/)
        {
            //Debug.Log("Thruster Pre: " + _data.movement);

            Vector3 move = Vector3.Scale(_data.movement, absMoveVec);
            Vector3 rot = Vector3.Scale(_data.rotation, absRotVec);

            //Debug.Log("Thruster move: " + move);
            //Debug.Log("Thruster rot: " + rot);

            if (shouldFire(move, movementVectorFiring) || shouldFire(rot, rotationVectorFiring))
            {
                particles.Play();
            }
            else
            {
                particles.Stop();
            }

        }   
    }

    bool shouldFire(Vector3 val1,  Vector3 val2)
    {
        if ((val1.x > 0.1 && val2.x > 0.1) || (val1.x < -0.1 && val2.x < -0.1)) { return true; }
        else if ((val1.y > 0.1 && val2.y > 0.1) || (val1.y < -0.1 && val2.y < -0.1)) { return true; }
        else if ((val1.z > 0.1 && val2.z > 0.1) || (val1.z < -0.1 && val2.z < -0.1)) { return true; }
        else
        { return false; }
    }
}
