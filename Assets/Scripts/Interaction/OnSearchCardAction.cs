using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSearchCardAction : MonoBehaviour, IInteractable
{
    public string GetDescription()
    {
        if (gameObject.tag == "CardSearch")
            return "Fouiller";
        else if (gameObject.tag == "CardSearched")
            return "Vous avez déjà fouillé cet objet";
        else
            return "La carte a été trouvée";
    }

    public void Interact()
    {
        GameObject player = GameObject.Find("Player");

        // Add sound Search.mp3
        AudioSource audio = gameObject.transform.parent.GetComponent<AudioSource>();
        audio.volume = 0.5f;
        audio.Play();
        
    
        if (gameObject.tag == "CardSearch") {
            player.GetComponent<PlayerMovement>().enabled = false;
            OnTriggerEnter();
        } else {
            return;
        }
    }

    private void OnTriggerEnter()
    {
        // Debug.Log("Searching...");
        gameObject.tag = "CardSearched";

        Invoke(nameof(UnfreezePlayer), 5f);
    }

    private void UnfreezePlayer() {
        // Debug.Log("Stopped searching...");
        GameObject player = GameObject.Find("Player");

        GameObject[] cardSearchObjects = GameObject.FindGameObjectsWithTag("CardSearch");

        int probsToFind = cardSearchObjects.Length;

        Debug.Log(probsToFind);

        DidHeFind(probsToFind);

        player.GetComponent<PlayerMovement>().enabled = true;
    }

    private void DidHeFind(int probs) {
        int random = Random.Range(1, probs);

        if (random == 1) {
            GameObject player = GameObject.Find("Player");
            Debug.Log("You found the card !");
            player.GetComponent<Inventory>().AddItem("Card");
            Debug.Log(Inventory.isCardCollected);

            
            // Add HistorySearched tag to all historySearchObjects
            GameObject[] cardSearchedObjects = GameObject.FindGameObjectsWithTag("CardSearched");

            foreach (GameObject cardSearchedObject in cardSearchedObjects) {
                cardSearchedObject.tag = "NothingToSearch";
            }


            GameObject[] cardSearchObjects = GameObject.FindGameObjectsWithTag("CardSearch");
            
            foreach (GameObject cardSearchObject in cardSearchObjects) {
                cardSearchObject.tag = "NothingToSearch";
            }

        } else {
            // Debug.Log("You found nothing...");
        }
    }
}
