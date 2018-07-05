using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayClick()
    { SceneManager.LoadScene("Main"); }

    public void QuitClick()
    { Application.Quit(); }
}
