using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSearchCardAction : MonoBehaviour, IInteractable
{
    public string GetDescription()
    {
        if (gameObject.tag == "CardSearch")
            return "Fouiller";
        else if (Inventory.isCardCollected)
            return "La carte a été trouvée";
        else
            return "En train de fouiller...";
    }

    public void Interact()
    {
        GameObject player = GameObject.Find("Player");

        GameObject[] cardSearchObjects = GameObject.FindGameObjectsWithTag("CardSearch");
        
    
        if (gameObject.tag == "CardSearch") {

            foreach (GameObject cardSearchObject in cardSearchObjects) {
                cardSearchObject.tag = "CurrentlySearching";
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
        // Debug.Log("Stopped searching...");
        GameObject player = GameObject.Find("Player");

        gameObject.tag = "CardSearched";

        GameObject[] cardSearchObjects = GameObject.FindGameObjectsWithTag("CurrentlySearching");

        int probsToFind = cardSearchObjects.Length;

        DidHeFind(probsToFind);

        player.GetComponent<PlayerMovement>().enabled = true;
    }

    private void DidHeFind(int probs) {
        int random = Random.Range(1, probs);

        if (random == 1) {
            GameObject player = GameObject.Find("Player");
            player.GetComponent<Inventory>().AddItem("Card");

            GameObject[] cardSearchedObjects = GameObject.FindGameObjectsWithTag("CardSearched");

            foreach (GameObject cardSearchedObject in cardSearchedObjects) {
                cardSearchedObject.tag = "NothingToSearch";
            }


            GameObject[] cardSearchObjects = GameObject.FindGameObjectsWithTag("CurrentlySearching");
            
            foreach (GameObject cardSearchObject in cardSearchObjects) {
                cardSearchObject.tag = "NothingToSearch";
            }

        } else {
            // Debug.Log("You found nothing...");
            GameObject[] cardSearchObjects = GameObject.FindGameObjectsWithTag("CurrentlySearching");

            foreach (GameObject cardSearchObject in cardSearchObjects) {
                cardSearchObject.tag = "CardSearch";
            }
        }
    }
}
