using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSearchCardAction : MonoBehaviour, IInteractable
{
    public string GetDescription()
    {
        if (gameObject.tag == "CardSearch")
            return "Fouiller";
        else if (gameObject.tag == "CardSearch")
            return "Vous avez déjà fouillé cet objet";
        else
            return "La carte a été trouvée";
    }

    public void Interact()
    {
        GameObject player = GameObject.Find("Player");
        
    
        if (gameObject.tag == "CardSearch") {
            player.GetComponent<PlayerMovement>().enabled = false;
            OnTriggerEnter();
        } else {
            return;
        }
    }

    private void OnTriggerEnter()
    {
        Debug.Log("Searching...");
        gameObject.tag = "CardSearched";

        Invoke(nameof(UnfreezePlayer), 5f);
    }

    private void UnfreezePlayer() {
        Debug.Log("Stopped searching...");
        GameObject player = GameObject.Find("Player");

        GameObject[] historySearchObjects = GameObject.FindGameObjectsWithTag("CardSearch");

        int probsToFind = historySearchObjects.Length;

        DidHeFind(probsToFind);

        player.GetComponent<PlayerMovement>().enabled = true;
    }

    private void DidHeFind(int probs) {
        int random = Random.Range(0, probs);

        if (random == 0) {
            GameObject player = GameObject.Find("Player");
            Debug.Log("You found the card !");

            
        // Add HistorySearched tag to all historySearchObjects
        GameObject[] historySearchedObjects = GameObject.FindGameObjectsWithTag("CardSearched");

        foreach (GameObject historySearchedObject in historySearchedObjects) {
            historySearchedObject.tag = "NothingToSearch";
        }


        GameObject[] historySearchObjects = GameObject.FindGameObjectsWithTag("CardSearch");
        
        foreach (GameObject historySearchObject in historySearchObjects) {
            historySearchObject.tag = "NothingToSearch";
        }

        } else {
            Debug.Log("You found nothing...");
        }
    }
}
