using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{

    [Header("References")]
    AudioClip WelcomeMusic_FX;

    // Start is called before the first frame update
    void Start()
    {
        WelcomeMusic_FX = Resources.Load<AudioClip>("WelcomeMusic");
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = WelcomeMusic_FX;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame() {
        Application.Quit();
    }
}
