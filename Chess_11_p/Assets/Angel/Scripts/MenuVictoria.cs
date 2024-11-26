using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class MenuVictoria : MonoBehaviour
{
    public Sprite[] Campeon;
    public string[] NombreC;
    public Image HeroeP;
    public TextMeshProUGUI NombreUI;
    
    public void SpawnPanel(bool p1, int equipoGanador)
    {
        int equipoIndice = equipoGanador;
        

        if (p1 == true)
        {
            HeroeP.sprite = Campeon[equipoGanador];
            NombreUI.text = NombreC[equipoGanador];
        }
        else
        {
            HeroeP.sprite = Campeon[equipoGanador];
            NombreUI.text = NombreC[equipoGanador];
        }

    }
    
    
}
