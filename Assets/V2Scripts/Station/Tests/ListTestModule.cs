using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListTestModule : MonoBehaviour
{
    public delegate void Test(TestEventData data);
    public static event Test OnDock;
    public static event Test OnUndock;

    [SerializeField]
    GameObject _targetObject;
    [SerializeField]
    ListTestMain _target;

    [SerializeField]
    List<ListTestItem> _items = new List<ListTestItem>();

    private void Awake()
    {
        _target = _targetObject.GetComponent<ListTestMain>();
    }

    public List<ListTestItem> getItems() { return _items; }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(OnDock != null)
            {
                TestEventData _data = new TestEventData();
                _data.Target = _target;
                _data.Caller = this;
                OnDock(_data);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (OnUndock != null)
            {
                TestEventData _data = new TestEventData();
                _data.Target = _target;
                _data.Caller = this;
                OnUndock(_data);
            }
        }
    }
}
