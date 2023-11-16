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
            return "Cet objet est vide";
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

            // unfreeze player after 3 seconds
            Invoke(nameof(UnfreezePlayer), 3f);

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
    }

    private void UnfreezePlayer() {
        GameObject player = GameObject.Find("Player");

        gameObject.tag = "CardSearched";

        GameObject[] cardSearchObjects = GameObject.FindGameObjectsWithTag("CurrentlySearching");

        int probsToFind = cardSearchObjects.Length;

        DidHeFind(probsToFind);

        player.GetComponent<PlayerMovement>().enabled = true;
    }

    private void OnTriggerExit()
    {
        Invoke(nameof(UnfreezePlayer), 3f);
    }

    private void DidHeFind(int probs) {
        int random = Random.Range(1, probs);

        float probToFind = 0;

        if (probs > 10) {
            probToFind = probs / 1.5f;
        } else if (probs > 5) {
            probToFind = probs / 1.2f;
        } else {
            probToFind = probs;
        }
        // Get 40% of the time the card
        if (random < probToFind) {
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
