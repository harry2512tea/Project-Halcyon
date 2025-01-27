using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBase : MonoBehaviour
{
    public virtual void Interact(GameObject caller)
    { Debug.Log("Interacted"); }
}
