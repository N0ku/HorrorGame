using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSearchHistoryAction : MonoBehaviour, IInteractable
{
    public string GetDescription()
    {
        if (gameObject.tag == "HistorySearch")
            return "Fouiller";
        else if (gameObject.tag == "HistorySearched")
            return "Vous avez déjà fouillé cet objet";
        else
            return "L'objet de vos souvenirs a été trouvé";
    }

    public void Interact()
    {
        GameObject player = GameObject.Find("Player");
        
    
        if (gameObject.tag == "HistorySearch") {
            player.GetComponent<PlayerMovement>().enabled = false;
            OnTriggerEnter();
        } else {
            return;
        }
    }

    private void OnTriggerEnter()
    {
        Debug.Log("Searching...");
        gameObject.tag = "HistorySearched";

        Invoke(nameof(UnfreezePlayer), 5f);
    }

    private void UnfreezePlayer() {
        Debug.Log("Stopped searching...");
        GameObject player = GameObject.Find("Player");

        GameObject[] historySearchObjects = GameObject.FindGameObjectsWithTag("HistorySearch");

        int probsToFind = historySearchObjects.Length;

        DidHeFind(probsToFind);

        player.GetComponent<PlayerMovement>().enabled = true;
    }

    private void DidHeFind(int probs) {
        int random = Random.Range(0, probs);

        if (random == 0) {
            GameObject player = GameObject.Find("Player");
            Debug.Log("You found something!");

            
        // Add HistorySearched tag to all historySearchObjects
        GameObject[] historySearchedObjects = GameObject.FindGameObjectsWithTag("HistorySearched");

        foreach (GameObject historySearchedObject in historySearchedObjects) {
            historySearchedObject.tag = "NothingToSearch";
        }


        GameObject[] historySearchObjects = GameObject.FindGameObjectsWithTag("HistorySearch");
        
        foreach (GameObject historySearchObject in historySearchObjects) {
            historySearchObject.tag = "NothingToSearch";
        }

        } else {
            Debug.Log("You found nothing...");
        }
    }
}
