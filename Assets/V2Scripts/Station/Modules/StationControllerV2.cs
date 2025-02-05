using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CompartmentControllerV2))]
public class StationControllerV2 : MonoBehaviour
{
    [SerializeField]
    List<CompartmentControllerV2> m_Compartments = new List<CompartmentControllerV2>();

    CompartmentControllerV2 m_Compartment;

    [SerializeField]
    List<GeneratorBase> m_generators = new List<GeneratorBase>();

    [SerializeField]
    List<SubSystemBase> m_subSystems = new List<SubSystemBase>();

    [SerializeField]
    List<DockingPortV2> m_dockingPorts = new List<DockingPortV2>();

    InputSystem_Actions input;

    private void Awake()
    {
        input = new InputSystem_Actions();
        m_Compartment = GetComponent<CompartmentControllerV2>();
        AddCompartments(m_Compartment);
        //m_dockingPorts.AddRange(m_Compartment.GetDockingPorts());
        //m_Compartments.Add(m_Compartment);
    }

    private void OnEnable()
    {
        input.Enable();

        CompartmentControllerV2.OnDock += OnDock;

        DockingPortV2.unDock += OnUndock;
        //CompartmentControllerV2.OnUndock += OnUndock;

        //input.Modules.Move.performed += onMovePerformed;
        //input.Modules.Move.canceled += onMoveCancelled;

        AddCompartments(m_Compartment);
    }

    private void OnDisable()
    {
        input.Disable();

        CompartmentControllerV2.OnDock -= OnDock;

        DockingPortV2.unDock -= OnUndock;

        m_Compartments.Clear();
        m_generators.Clear();
        m_subSystems.Clear();
        m_dockingPorts.Clear();
        //CompartmentControllerV2.OnUndock -= OnUndock;

        //input.Modules.Move.performed -= onMovePerformed;
        //input.Modules.Move.canceled -= onMoveCancelled;

    }

    void OnDock(DockingData _Data)
    {
        if(_Data.StationTarget == this)
        {
            AddCompartments(_Data.Caller);
        }
    }

    void OnUndock(DockingData _Data)
    {
        if (_Data.StationTarget == this)
        {
            if (_Data.Target == m_Compartment)
            {
                RemoveCompartments(_Data.Caller);
            }
            else if (_Data.Caller == m_Compartment)
            {
                RemoveCompartments(_Data.Target);
            }
            else
            {
                if(_Data.Caller.getChildCompartments().Contains(_Data.Target))
                {
                    RemoveCompartments(_Data.Target);
                }
                else
                {
                    RemoveCompartments(_Data.Caller);
                }
            }

        }
    }

    //recursive function to add every child compartment and ajoining module of a docking compartment to a list
    void AddCompartments(CompartmentControllerV2 _compartment)
    {
        m_Compartments.Add(_compartment);

        for(int gen = 0;  gen < _compartment.getGenerators().Count; gen++)
        { m_generators.Add(_compartment.getGenerators()[gen]); }

        for(int sys = 0; sys < _compartment.GetSubSystems().Count; sys++)
        { m_subSystems.Add(_compartment.GetSubSystems()[sys]); }

        for (int port = 0; port < _compartment.GetDockingPorts().Count; port++)
        { m_dockingPorts.Add(_compartment.GetDockingPorts()[port]); }

        if(_compartment.getChildCompartments().Count > 0)
        {
            for(int child = 0; child < _compartment.getChildCompartments().Count; child++)
            { AddCompartments(_compartment.getChildCompartments()[child]); }
        }
        else
        { return; }
    }

    //recursive function to remove every child compartment and ajoining module of an undocking compartment from a list
    void RemoveCompartments(CompartmentControllerV2 _compartment)
    {
        //remove the given compartment from the station list
        m_Compartments.Remove(_compartment);

        //remove any generators attached to the compartment from the station list
        for (int gen = 0; gen < _compartment.getGenerators().Count; gen++)
        { m_generators.Remove(_compartment.getGenerators()[gen]); }

        //remove any subsystems attached to the compartment from the station list
        for(int sys = 0; sys < _compartment.GetSubSystems().Count; sys++)
        { m_subSystems.Remove(_compartment.GetSubSystems()[sys]);}

        for (int port = 0; port < _compartment.GetDockingPorts().Count; port++)
        { m_dockingPorts.Remove(_compartment.GetDockingPorts()[port]); }

        //check if there are any compartments left to cycle through
        if (_compartment.getChildCompartments().Count > 0)
        {
            //cycle through every compartment docked to the current compartment being undocked and run this same function for them
            for (int child = 0; child < _compartment.getChildCompartments().Count; child++)
            { RemoveCompartments(_compartment.getChildCompartments()[child]); }
        }
        else
        { return; }
    }

}
