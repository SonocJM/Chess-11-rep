using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    public GameObject _MPausa;  //Objeto que se activara y desactivara
    public GameObject _Movimientos;
    public int RMenu;
    public int RPersonajes;
    public bool MPausa = false; //Estado inicial del objeto
    public bool Movimientos = false;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (MPausa == false)
            {
                _MPausa.SetActive(true);
                MPausa = true;
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (MPausa == true)
            {
                ReanudarP();
            }
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (Movimientos == false)
            {
                _Movimientos.SetActive(true);
                Movimientos = true;
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Movimientos == true)
            {
                ReanudarM();
            }
        }
    }
    public void ReanudarP()
    {
        _MPausa.SetActive(false);
        MPausa = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void ReanudarM()
    {
        _Movimientos.SetActive(false);
        Movimientos = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void RMP()
    {
        SceneManager.LoadScene(RMenu);
    }
    public void RSP()
    {
        SceneManager.LoadScene(RPersonajes);
    }
}
