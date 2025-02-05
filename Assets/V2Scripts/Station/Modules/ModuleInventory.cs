using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Resource
{
    Ice,
    Nitrates,
    DryIce,
    Regolith,
    Ferrite,
    Titanite,
    Oxygen,
    Nitrogen,
    Nitro,
    Helium,
    Iron,
    Titanium
}

public class ModuleInventory : MonoBehaviour
{
    //Resource Count
    [SerializeField]
    float MaxResources;

    //Raw Resources
    [SerializeField]
    float Ice, Nitrates, DryIce, Regolith, Ferrite, Titanite;

    //Refined Resources
    [SerializeField]
    float Oxygen, Nitrogen, Nitro, Helium, Iron, Titanium;
}
