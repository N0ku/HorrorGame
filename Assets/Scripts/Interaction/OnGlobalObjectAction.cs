using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGlobalObjectAction : MonoBehaviour, IInteractable
{
    public string GetDescription()
    {
        return "Objet";
    }

    public void Interact()
    {
        Debug.Log(gameObject.name);
    }

}
