using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fear : MonoBehaviour
{

    private MadnessState madnessState;
    private FlashlightManager FlashlightScript;
    private CameraShake CameraShakeScript;
    private FearMalus fearMalus;

    public static ArrayList fearMalusList = new ArrayList();
    public static ArrayList fearPermamentMalusList = new ArrayList();

    [SerializeField] private float fearTick = 0.35f;
    [SerializeField] private bool ShowDebugFear = false;
    public static float fear;

    [SerializeField]
    private GameObject FlashlightLight;
    void Start()
    {
        Debug.Log("Fear script is running");
        madnessState = MadnessState.Calm;
        FlashlightScript = FindObjectOfType<FlashlightManager>();
    }

    void FixedUpdate()
    {
        if (fear >= 100f)
        {
            KillPlayer();

            fear = 0f;

            if (madnessState != MadnessState.Terrified)
            {
                AddMadness();
            } 
            else if (madnessState == MadnessState.Terrified) 
            {
                SceneManager.LoadScene(5);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

        }
    }

    void Update()
    {
        AddFear();
        CheckFear();
        CheckMalus();

        if (ShowDebugFear)
        {
            Debug.Log("fear : " + fear);
        }
        // Debug.Log("madnessState : " + madnessState);

        for (int i = 0; i < fearMalusList.Count; i++)
        {
            // Debug.Log("fearMalusList[" + i + "] : " + fearMalusList[i]);
        }
        for (int i = 0; i < fearPermamentMalusList.Count; i++)
        {
            // Debug.Log("fearPermamentMalusList[" + i + "] : " + fearPermamentMalusList[i]);
        }

    }

    void AddFear()
    {
        if (FlashlightManager.flashlightIsOn == false || FlashlightManager.isUsable == false)
        {
            fear += fearTick * Time.deltaTime;
            // Debug.Log("increased : " + fear);
        }
    }

    void CheckFear()
    {
        if (fear >= 75f && fearMalusList.Contains(FearMalus.HorrorSounds) == false
            && fearPermamentMalusList.Contains(FearMalus.HorrorSounds) == false)
        {
            fearMalusList.Add(FearMalus.HorrorSounds);
            Debug.Log("HorrorSounds");
        }
        else if (fear >= 50f && fearMalusList.Contains(FearMalus.ScreanShake) == false
            && fearPermamentMalusList.Contains(FearMalus.ScreanShake) == false)
        {
            fearMalusList.Add(FearMalus.ScreanShake);
            Debug.Log("ScreanShake");
        }
        else if (fear >= 25f && fearMalusList.Contains(FearMalus.QuickFlashlight) == false
            && fearPermamentMalusList.Contains(FearMalus.QuickFlashlight) == false)
        {
            fearMalusList.Add(FearMalus.QuickFlashlight);
            Debug.Log("QuickFlashlight");
        }
    }

    void AddMadness()
    {
        if (madnessState == MadnessState.Calm)
        {
            madnessState = MadnessState.Scared;
            // add random malus in fearPermamentMalusList
            fearPermamentMalusList.Add(fearMalusList[UnityEngine.Random.Range(0, fearMalusList.Count)]);
        }
        else if (madnessState == MadnessState.Scared)
        {
            madnessState = MadnessState.Terrified;
            fearPermamentMalusList.Add(fearMalusList[UnityEngine.Random.Range(0, fearMalusList.Count)]);
        }
        else if (madnessState == MadnessState.Terrified)
        {
            fearPermamentMalusList.Add(fearMalusList[UnityEngine.Random.Range(0, fearMalusList.Count)]);
        }
        fearMalusList.Clear();
    }
    void FlashlightMalus()
    {
        if (FlashlightManager.flashlightIsOn == true && FlashlightManager.isUsable == true)
        {
            FlashlightScript.bugFlashlight(FlashlightLight);
            Debug.Log("QuickFlashlight");
        }
    }

    void ScreanShakeMalus()
    {
        CameraShake.shaketrue = true;
        Debug.Log("ScreenShake");
    }

    void CheckMalus()
    {
        if (fearMalusList.Contains(FearMalus.QuickFlashlight) || fearPermamentMalusList.Contains(FearMalus.QuickFlashlight))
        {
            int random = UnityEngine.Random.Range(5, 30);
            if (IsInvoking(nameof(FlashlightMalus)) == false)
                InvokeRepeating(nameof(FlashlightMalus), 3, random);
        }
        if (fearMalusList.Contains(FearMalus.ScreanShake) || fearPermamentMalusList.Contains(FearMalus.ScreanShake))
        {
            int random = UnityEngine.Random.Range(10, 20);
            if (IsInvoking(nameof(ScreanShakeMalus)) == false)
                InvokeRepeating(nameof(ScreanShakeMalus), 1, random);
        }
        if (fearMalusList.Contains(FearMalus.HorrorSounds) || fearPermamentMalusList.Contains(FearMalus.HorrorSounds))
        {
            // TODO
        }
    }

    private void KillPlayer()
    {
        GameObject playerEverything = GameObject.Find("Player");
        playerEverything.GetComponent<PlayerMovement>().enabled = false;
        TeleportPlayer();
    }

    private void TeleportPlayer()
    {
        GameObject playerEverything = GameObject.Find("Player");
        GameObject elevator = GameObject.FindWithTag("manageThomasElevator");

        if (elevator != null)
        {
            playerEverything.transform.position = elevator.transform.position;
            playerEverything.GetComponent<PlayerMovement>().enabled = true;
            elevator.transform.parent.GetComponent<Animator>().Play("OpenDoors");
            int nbOfDeaths = int.Parse(playerEverything.GetComponent<Inventory>().GetInventory()[2]);
            playerEverything.GetComponent<Inventory>().AddItem((nbOfDeaths + 1).ToString());

            if (Vector3.Distance(playerEverything.transform.position, elevator.transform.position) > 2f)
            {
                elevator.transform.parent.GetComponent<Animator>().Play("CloseDoors");
            }
        }
    }
}
