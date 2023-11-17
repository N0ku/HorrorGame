using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ProceduralGenerator;

public class GestionnaireDeNiveau : MonoBehaviour
{

    static public void NiveauTermine()
    {

        Debug.Log("NTM JVEUX MANGER");
        Debug.Log("NTM JVEUX MANGER");
        Debug.Log("NTM JVEUX MANGER");
        Debug.Log("NTM JVEUX MANGER");
        Debug.Log("NTM JVEUX MANGER");
        Debug.Log("NTM JVEUX MANGER");
        Debug.Log("NTM JVEUX MANGER");
        Debug.Log("NTM JVEUX MANGER");
        Debug.Log("NTM JVEUX MANGER");
        Debug.Log("NTM JVEUX MANGER");                                                                      
        actualEtage = EtageType.Etage2;
        Debug.Log(actualEtage+"Step1");
        Scene nouvelleScene = SceneManager.CreateScene(SceneManager.GetActiveScene().name +"2");
        SceneManager.LoadScene(nouvelleScene.name, LoadSceneMode.Additive);
        SceneManager.UnloadScene(SceneManager.GetActiveScene().name);
        actualEtage = EtageType.Etage2;
        Debug.Log(actualEtage + "Step2");
        SceneManager.SetActiveScene(nouvelleScene);
        Debug.Log(actualEtage+"Step3");
        actualEtage = EtageType.Etage2;


        Debug.Log("NTM G FAIM");
        Debug.Log("NTM G FAIM");
        Debug.Log("NTM G FAIM");
        Debug.Log("NTM G FAIM");
        Debug.Log("NTM G FAIM");
        Debug.Log("NTM G FAIM");
        Debug.Log("NTM G FAIM");
        Debug.Log("NTM G FAIM");
        Debug.Log("NTM G FAIM");
        Debug.Log("NTM G FAIM");
    }

}
