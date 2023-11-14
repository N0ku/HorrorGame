using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    public static bool isPaused = false;

    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (pauseMenu.activeSelf) {
                ResumeGame();
                Cursor.lockState = CursorLockMode.Locked;
            } else {
                PauseGame();
                Cursor.lockState = CursorLockMode.None;
            }
        }
        
    }

    public void PauseGame() {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame() {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
    }

    public void ExitGame() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void SaveGame() {
        Debug.Log("Saving game...");
    }
}
