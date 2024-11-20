using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferHeroData : MonoBehaviour
{
    public SHeroe p1;
    public SHeroe p2;

    public void SaveData()
    {
        GameData.p1T = p1.IndiceHeroe + 1;
        GameData.p2T = p2.IndiceHeroe + 1;
    }
    
}
