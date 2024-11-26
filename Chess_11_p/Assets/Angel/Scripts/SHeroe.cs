using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SHeroe : MonoBehaviour
{
    public Sprite[] Campeon;
    public Sprite[] Movimiento;
    public string[] Descripcion;
    public string[] NombreHero;
    public Image HeroeS;
    public Image HeroeS2;
    public Image Habilidad;
    public TextMeshProUGUI Lore;
    public TextMeshProUGUI CampeonNonbre;
    public int IndiceHeroe = 0;
    public int IndiceHabilidad = 0;
    public int IndiceLore = 0;
    public int IndiceCNombre = 0;

    void Start()
    {
        Hero();
    }
    public void Siguiente()
    {
        IndiceHeroe++;
        if (IndiceHeroe >= Campeon.Length)
            IndiceHeroe = 0 ;

        IndiceHabilidad++;
        if(IndiceHabilidad >= Movimiento.Length)
            IndiceHabilidad = 0 ;

        IndiceLore++;
        if(IndiceLore >= Descripcion.Length)
            IndiceLore = 0 ;

        IndiceCNombre++;
        if (IndiceCNombre >= NombreHero.Length)
            IndiceCNombre = 0;

        Hero();
    }
    public void Anterior()
    {
        IndiceHeroe--;
        if (IndiceHeroe < 0)
            IndiceHeroe = Campeon.Length - 1;

        IndiceHabilidad--;
        if(IndiceHabilidad < 0)
            IndiceHabilidad = Movimiento.Length - 1;

        IndiceLore--;
        if (IndiceLore < 0)
            IndiceLore = Descripcion.Length - 1;

        IndiceCNombre--;
        if (IndiceCNombre < 0)
            IndiceCNombre = NombreHero.Length - 1;

        Hero();
    }
    public void Hero()
    {
        HeroeS.sprite = Campeon[IndiceHeroe];
        HeroeS2.sprite = Campeon[IndiceHeroe];
        Habilidad.sprite = Movimiento[IndiceHabilidad];
        Lore.text = Descripcion[IndiceLore];
        CampeonNonbre.text = NombreHero[IndiceCNombre];
    }
}
