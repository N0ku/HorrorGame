using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        isElevatorOpen = false;
        canOpenElevator = false;
        isPlayerFreeze = false;
    }

    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        // if (other.gameObject.tag == "closeElevator" && isElevatorOpen) {
        //     closeDoor(other.transform.parent.gameObject.name);
        //     isElevatorOpen = true;
        // }

        if ((other.gameObject.tag == "manageElevator") && !isElevatorOpen) {
            openElevator(other.transform.parent.gameObject.name);
            isElevatorOpen = true;
            canOpenElevator = false;
        }

        if (other.gameObject.tag == "openElevator") {
            Invoke(nameof(freezePlayer), 1f);
        }

        if ((other.gameObject.tag == "openElevator" && canOpenElevator)) {
            openElevator(other.transform.parent.gameObject.name);
            isElevatorOpen = true;
            canOpenElevator = false;
        }
    }

    // Update is called once per frame
    void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.tag == "openElevator" && canOpenElevator && isPlayerFreeze)) {
            Invoke(nameof(unfreezePlayer), 5f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "manageElevator" && isElevatorOpen) {
            closeDoor(other.transform.parent.gameObject.name);
            isElevatorOpen = false;
        }
    }


    private void closeDoor(string elevator) {
        Animator animator = GameObject.Find(elevator).GetComponent<Animator>();
        animator.speed = 0.5f;
        animator.Play("CloseDoors");

        Invoke("changeCanOpenElevator", 10f);
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
