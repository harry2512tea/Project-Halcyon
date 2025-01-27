using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingPanel : Interactable
{
    DockingController controller;
    public Camera playerCam;
    bool controlled;
    PlayerController playerControl;
    GameObject player;
    public Transform usePos;

    private void Start()
    {
        controlled = false;
        controller = transform.parent.parent.gameObject.GetComponent<DockingController>();
    }

    public override void Interact(GameObject _player)
    {
        playerControl = _player.GetComponent<PlayerController>();
        player = _player;
        enterDockingMode();
    }

    // Update is called once per frame
    void Update()
    {
        if(controlled)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                exitDockingMode();
            }
        }
    }

    void enterDockingMode()
    {
        controlled = true;
        controller.dockingMode = true;
        controller.enableDockingMode();
        playerControl.deleteRigidBody();
        player.transform.parent = usePos;
        player.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        player.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        playerControl.controlling = true;
        //player.transform.position = usePos.position;
        
    }

    void exitDockingMode()
    {
        controlled = false;
        controller.dockingMode = false;
        controller.disableDockingMode();
        player.transform.parent = null;
        playerControl.addRigidBody();
        playerControl.controlling = false;

    }
}
