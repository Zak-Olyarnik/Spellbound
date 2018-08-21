using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject instructionsMenu;
    [SerializeField] private GameObject creditsMenu;

    public void PlayClick()
    { SceneManager.LoadScene("Main"); }

    public void TutorialClick()
    {
        instructionsMenu.SetActive(true);
        startMenu.SetActive(false);
    }

    public void CreditsClick()
    {
        creditsMenu.SetActive(true);
        startMenu.SetActive(false);
    }

    public void BackClick()
    {
        startMenu.SetActive(true);
        creditsMenu.SetActive(false);
    }

    public void QuitClick()
    { Application.Quit(); }
}
