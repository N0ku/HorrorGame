using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    
    public static bool isCardCollected = false;

    public static bool isSouvenirCollected = false;

    public string[] inventorySlots;

    // Start is called before the first frame update
    void Start()
    {
        inventorySlots = new string[2];
        inventorySlots.SetValue("Empty", 0);
        inventorySlots.SetValue("Empty", 1);
        
    }

    public void AddItem(string item) {
        if (item == "Card") {
            isCardCollected = true;
            inventorySlots.SetValue("Card", 0);
        } else if (item == "Souvenir") {
            isSouvenirCollected = true;
            inventorySlots.SetValue("Souvenir", 1);
        }
        Debug.Log(inventorySlots[0]);
    }

    public void EmptyInventory() {
        inventorySlots.SetValue("Empty", 0);
        inventorySlots.SetValue("Empty", 1);
    }
}
