using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ProceduralGenerator;
using static MonsterScript;

public class CollisionManager : MonoBehaviour
{

    GameObject player;

    // ELEVATOR MANAGER //
    
    public bool isElevatorOpen = false;

    public bool canOpenElevator = false;

    private bool isPlayerFreeze = false;

    // ELEVATOR MANAGER //
    
    void Start()
    {
        PlayerSpawn();
        freezePlayer();
        isElevatorOpen = false;
        canOpenElevator = false;
     
    }

    private void Update()
    {
        PlayerSpawn();
    }

    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        // if (other.gameObject.tag == "closeElevator" && isElevatorOpen) {
        //     closeDoor(other.transform.parent.gameObject.name);
        //     isElevatorOpen = true;
        // }

        if ((other.gameObject.tag == "manageExitElevator") && !isElevatorOpen && Inventory.isCardCollected && Inventory.isSouvenirCollected) {
            openElevator(other.transform.parent.gameObject.name);
            isElevatorOpen = true;
            canOpenElevator = false;
        }

        if (other.gameObject.tag == "freezeExitElevator") {
            Invoke(nameof(freezePlayer), 0.3f);

            // EMPTY THE SCENE TO REGENERATE A NEW ONE
        }

        if ((other.gameObject.tag == "manageThomasElevator" && canOpenElevator)) {
            openElevator(other.transform.parent.gameObject.name);
            isElevatorOpen = true;
            canOpenElevator = false;
        }

        if ((other.gameObject.tag == "virusHitbox" || other.gameObject.tag == "yetiHitbox")) {
            AudioSource source = other.transform.parent.gameObject.GetComponent<AudioSource>();
            source.volume = Random.Range(1f, 3.5f);
            source.pitch = Random.Range(0.8f, 1.2f);
            source.Play();
        }
    }

    public void PlayerSpawn()
    {
        player = GameObject.Find("Player");
         if (actualEtage != null)
         {
            if (actualEtage == EtageType.Etage1 && player.transform.position== new Vector3(-20, 0.999f, -20))
            {
                player = GameObject.Find("Player");
                GameObject manageThomasElevator = GameObject.FindWithTag("ThomasFirstSpawn");

                if (manageThomasElevator != null)
                {
                    Vector3 newPosition = manageThomasElevator.transform.position;
                    newPosition.x += 2;
                    newPosition.y += 2;
                    player.transform.position = newPosition;
                    unfreezePlayer();
                }
                else
                {
                    Debug.LogError("Le GameObject avec le tag 'ThomasFirstSpawn' n'a pas �t� trouv�.");
                }
            }
            else if (actualEtage == EtageType.Etage1 && isPlayerKilled)
            {
                player = GameObject.Find("Player");
                GameObject manageThomasElevator = GameObject.FindWithTag("ThomasFirstSpawn");

                if (manageThomasElevator != null)
                {
                    Vector3 newPosition = manageThomasElevator.transform.position;
                    player.transform.position = newPosition;
                    newPosition.x += 2;
                    newPosition.y += 2;
                }
                else
                {
                    Debug.LogError("Le GameObject avec le tag 'ThomasFirstSpawn' n'a pas �t� trouv�.");
                }
                isPlayerKilled = false;
            }
        }
    }

    // Update is called once per frame
    void OnTriggerStay(Collider other)
    {        
        player = GameObject.Find("Player");
        if ((other.gameObject.tag == "freezeExitElevator" && canOpenElevator && isPlayerFreeze)) {
            GameObject manageThomasElevator = GameObject.FindWithTag("manageThomasElevator");
            if (manageThomasElevator != null) {
                player.transform.position = manageThomasElevator.transform.position;
            }
        }
        if ((other.gameObject.tag == "manageThomasElevator" && isPlayerFreeze)) {
            Invoke(nameof(unfreezePlayer), 1f);
            player.GetComponent<Inventory>().EmptyInventory();
            Inventory.isCardCollected = false;
            Inventory.isSouvenirCollected = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "manageExitElevator" && isElevatorOpen || other.gameObject.tag == "manageThomasElevator" && isElevatorOpen) {
            closeDoor(other.transform.parent.gameObject.name);
            isElevatorOpen = false;
        }
    }


    private void closeDoor(string elevator) {
        Animator animator = GameObject.Find(elevator).GetComponent<Animator>();
        animator.speed = 0.5f;
        animator.Play("CloseDoors");
    }

    private void changeCanOpenElevator() {
        canOpenElevator = true;
    }

    private void openElevator(string elevator) {
        GameObject.Find(elevator).GetComponent<Animator>().speed = 0.5f;
        GameObject.Find(elevator).GetComponent<Animator>().Play("OpenDoors");
    }

    private void freezePlayer() {
        GameObject.Find("Player").GetComponent<PlayerMovement>().enabled = false;

        isPlayerFreeze = true;

        Invoke(nameof(changeCanOpenElevator), 3.5f);
    }

    private void unfreezePlayer() {
        GameObject.Find("Player").GetComponent<PlayerMovement>().enabled = true;

        Invoke(nameof(unfreezeVariable), 50f);

        CancelInvoke(nameof(unfreezePlayer));
    }

    private void unfreezeVariable() {
        isPlayerFreeze = false;
    }
}
