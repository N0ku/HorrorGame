using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDoorAction : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator myDoor = null;
    [SerializeField] private bool openTrigger = false;
    [SerializeField] private bool closeTrigger = true;

    public string GetDescription()
    {
         return "Porte";
    }

    public void Interact()
    {
        OnTriggerEnter();
    }

    private void OnTriggerEnter()
    {
        if (openTrigger)
        {
            Debug.Log("Open");
            myDoor.Play("doorOpen", 0, 0f);
            openTrigger = false;
            closeTrigger = true;
        }
        else if (closeTrigger)
        {
            Debug.Log("Close");
            myDoor.Play("doorClose", 0, 0f);
            closeTrigger = false;
            openTrigger = true;
        }
    }
}
