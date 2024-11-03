using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Materials")]
    public Material defaultMaterial;
    public Material highlightMaterial;

    private GameObject lastHoveredTile;

    void Update()
    {
        RaycastFromCamera();
    }

    private void RaycastFromCamera()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.CompareTag("Tile"))
            {
                GameObject hoveredTile = hit.collider.gameObject;

                if (hoveredTile != lastHoveredTile)
                {
                    if (lastHoveredTile != null)
                    {
                        lastHoveredTile.GetComponent<Renderer>().material = defaultMaterial;
                    }

                    hoveredTile.GetComponent<Renderer>().material = highlightMaterial;
                    lastHoveredTile = hoveredTile;
                }
            }
            else
            {
                if (lastHoveredTile != null)
                {
                    lastHoveredTile.GetComponent<Renderer>().material = defaultMaterial;
                    lastHoveredTile = null;
                }
            }
        }
        else
        {
            if (lastHoveredTile != null)
            {
                lastHoveredTile.GetComponent<Renderer>().material = defaultMaterial;
                lastHoveredTile = null;
            }
        }
    }



}
