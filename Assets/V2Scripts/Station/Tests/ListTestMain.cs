using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListTestMain : MonoBehaviour
{
    [SerializeField]
    List<ListTestModule> modules = new List<ListTestModule>();

    [SerializeField]
    List<ListTestItem> items = new List<ListTestItem>();

    private void OnEnable()
    {
        ListTestModule.OnDock += addModule;
        ListTestModule.OnUndock += removeModule;
    }

    private void OnDisable()
    {
        ListTestModule.OnDock -= addModule;
        ListTestModule.OnUndock -= removeModule;
    }

    void addModule(TestEventData _Data)
    {
        if(_Data.Target == this)
        {
            modules.Add(_Data.Caller);
            for(int i = 0; i < _Data.Caller.getItems().Count; i++)
            {
                items.Add(_Data.Caller.getItems()[i]);
            }
        }
    }

    void removeModule(TestEventData _Data)
    {
        if (_Data.Target == this)
        {
            modules.Remove(_Data.Caller);
            for (int i = 0; i < _Data.Caller.getItems().Count; i++)
            {
                items.Remove(_Data.Caller.getItems()[i]);
            }
        }
    }
}
