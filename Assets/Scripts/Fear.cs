using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fear : MonoBehaviour
{
    private enum MadnessState
    {
        Calm,
        Scared,
        Terrified,
    }

    private enum FearMalus
    {
        QuickFlashlight,
        ScreanShake,
        HorrorSounds,
    }

    private MadnessState madnessState;
    private FearMalus fearMalus;

    public static ArrayList fearMalusList = new ArrayList();
    public static ArrayList fearPermamentMalusList = new ArrayList();

    public static float fear;
    void Start()
    {
        Debug.Log("Fear script is running");
        madnessState = MadnessState.Calm;
    }

    void FixedUpdate()
    {
        if (fear >= 100f)
        {
            fear = 0f;

            AddMadness();

            transform.position = new Vector3(0, 2f, -2f);
            Debug.Log("you are dead");
        }
    }

    void Update()
    {
        AddFear();
        CheckFear();

        // Debug.Log("fear : " + fear);
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
            fear += 0.15f * Time.deltaTime;
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
            Debug.Log("End of the game");
        }
        fearMalusList.Clear();
    }
}

