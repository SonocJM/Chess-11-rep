using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    public string SceneName;
    public TransferHeroData thd;
    public void GoToScene(string name)
    {
        thd.SaveData();
        SceneManager.LoadScene(name);
       
    }
}
