using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIChessBoard : MonoBehaviour
{
    public TextMeshProUGUI cooldownP1;
    public TextMeshProUGUI cooldownP2;
    public GenerateBoard cB;
    

    public void UpdateUI()
    {
        UpdateCooldowns();
    }
    private void Start()
    {
        UpdateCooldowns();
    }

    private void UpdateCooldowns()
    {
        cooldownP1.text = cB.p1Cd.ToString();
        cooldownP2.text = cB.p2Cd.ToString();
    }
}
