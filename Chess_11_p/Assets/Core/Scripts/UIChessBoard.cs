using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIChessBoard : MonoBehaviour
{
    [Header("p1")]
    public TextMeshProUGUI cooldownP1;
    public GameObject buttonP1;
    public GameObject portraitP1;
    [Header("p2")]
    public TextMeshProUGUI cooldownP2;
    public GameObject buttonP2;
    public GameObject portraitP2;
    [Header("general")]
    public GenerateBoard cB;
    public int p1Team;
    public int p2Team;
    public Image[] AbilityIcons;
    public Image[] CharacterIcons;
    
    
    public void UniversalIndex()
    {

    }
    public void UpdateUI()
    {
        UpdateCooldowns();
    }
    private void Start()
    {
        UpdateCooldowns();
        p1Team = cB.p1T;
        p2Team = cB.p2T;
    }

    private void UpdateCooldowns()
    {
        cooldownP1.text = cB.p1Cd.ToString();
        cooldownP2.text = cB.p2Cd.ToString();
    }
}
