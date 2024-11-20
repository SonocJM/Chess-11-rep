using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string SceneName;
    public void GoToScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void Salir()
    {
        Application.Quit();
    }
}
