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
            return "Cet objet est vide";
        else if (Inventory.isSouvenirCollected)
            return "L'objet de vos souvenirs a été trouvé";
        else
            return "En train de fouiller...";
    }

    public void Interact()
    {
        GameObject player = GameObject.Find("Player");

        GameObject[] historySearchObjects = GameObject.FindGameObjectsWithTag("HistorySearch");
        
    
        if (gameObject.tag == "HistorySearch") {

            foreach (GameObject historySearchObject in historySearchObjects) {
                historySearchObject.tag = "CurrentlySearching";
            }

            player.GetComponent<PlayerMovement>().enabled = false;
            OnTriggerEnter();
        } else {
            return;
        }
    }

    private void OnTriggerEnter()
    {
        AudioSource audio = gameObject.transform.parent.GetComponent<AudioSource>();
        audio.volume = 0.5f;
        audio.Play();

        Invoke(nameof(UnfreezePlayer), 5f);
    }

    private void UnfreezePlayer() {
        GameObject player = GameObject.Find("Player");

        gameObject.tag = "HistorySearched";

        GameObject[] historySearchObjects = GameObject.FindGameObjectsWithTag("CurrentlySearching");

        int probsToFind = historySearchObjects.Length;

        DidHeFind(probsToFind);

        player.GetComponent<PlayerMovement>().enabled = true;
    }

    private void DidHeFind(int probs) {
        int random = Random.Range(1, probs);

        if (random == 1) {
            GameObject player = GameObject.Find("Player");
            player.GetComponent<Inventory>().AddItem("Souvenir");

            GameObject[] historySearchedObjects = GameObject.FindGameObjectsWithTag("HistorySearched");

            foreach (GameObject historySearchedObject in historySearchedObjects) {
                historySearchedObject.tag = "NothingToSearch";
            }


            GameObject[] historySearchObjects = GameObject.FindGameObjectsWithTag("CurrentlySearching");

            foreach (GameObject historySearchObject in historySearchObjects) {
                historySearchObject.tag = "NothingToSearch";
            }

        } else {
            GameObject[] historySearchObjects = GameObject.FindGameObjectsWithTag("CurrentlySearching");

            foreach (GameObject historySearchObject in historySearchObjects) {
                historySearchObject.tag = "HistorySearch";
            }
        }
    }
}
