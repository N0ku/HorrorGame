using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnElevatorAction : MonoBehaviour, IInteractable
{
    public string GetDescription()
    {
        if (Inventory.isCardCollected && Inventory.isSouvenirCollected)
            return "Allons y";
        else if (Inventory.isCardCollected && !Inventory.isSouvenirCollected)
            return "Je n'arrive pas à me souvenir...";
        else if (!Inventory.isCardCollected && Inventory.isSouvenirCollected)
            return "Où est ma carte d'ascenseur ?";
        else
            return "Il me manque la carte de l'ascenseur et un souvenir";
    }

    public void Interact()
    {
        OnTriggerEnter();
    }

    private void OnTriggerEnter()
    {
    
    }
}
