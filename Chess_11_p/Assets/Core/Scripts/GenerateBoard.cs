using Unity.VisualScripting;
using UnityEngine;

public class GenerateBoard : MonoBehaviour
{
    [Header("ints")]
    public int TilesX; 
    public int TilesY;
    public int p1T;
    public int p2T;

    [Header("GameObjects")]
    public GameObject tile; 
    private GameObject[,] tiles; 
    private GameObject lastHoveredTile; 
    private GameObject clickedTile; 

    [Header("Materials")]
    public Material defaultMaterial; 
    public Material highlightMaterial; 
    public Material clickMaterial;

    [Header("Scripts")]
    public GenerateBoard gB;


    private bool isHoverEnabled = true; 

    private void Awake()
    {
        tiles = new GameObject[TilesX, TilesY];
        GenerateAllTiles();
        AssignPieces(1, false);
        AssignPieces(1, true);
    }

    private void GenerateAllTiles()
    {
        for (int x = 0; x < TilesX; x++)
        {
            for (int y = 0; y < TilesY; y++)
            {
                GenerateSingleTile(x, y);
            }
        }
    }

    private void GenerateSingleTile(int x, int y)
    {
        Vector3 position = new Vector3(x, 0, y); 
        GameObject singleTile = Instantiate(tile, position, Quaternion.identity);
        singleTile.transform.SetParent(transform);
        singleTile.GetComponent<Renderer>().material = defaultMaterial;
        Tile tileS = singleTile.GetComponent<Tile>();
        tileS.board = gB;
        tileS.x = x;
        tileS.y = y;
        tiles[x, y] = singleTile; 
    }

    private void Update()
    {
        RaycastFromCamera();
        
    }
    

    private void RaycastFromCamera()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hoveredTile = hit.collider.gameObject;

            for (int x = 0; x < TilesX; x++)
            {
                for (int y = 0; y < TilesY; y++)
                {
                    if (tiles[x, y] == hoveredTile)
                    {
                        if (isHoverEnabled)
                        {
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

                        if (Input.GetMouseButtonDown(0)) 
                        {
                            HandleTileClick(hoveredTile);
                        }
                        return;
                    }
                }
            }

            
            if (lastHoveredTile != null)
            {
                lastHoveredTile.GetComponent<Renderer>().material = defaultMaterial;
                lastHoveredTile = null;
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

    private void HandleTileClick(GameObject clicked)
    {
        if (clickedTile == clicked)
        {
            clicked.GetComponent<Renderer>().material = defaultMaterial;
            clickedTile = null;
            isHoverEnabled = true;
        }
        else
        {
            if (clickedTile != null)
            {
                clickedTile.GetComponent<Renderer>().material = defaultMaterial;
            }

            clicked.GetComponent<Renderer>().material = clickMaterial;
            clickedTile = clicked;
            isHoverEnabled = false;
        }
    }

    private void AssignPieces(int team, bool p2)   
    {
        int[] row1;
        int[] row2;
        int[] row3;
        switch(team)
        {
            case 0:
                //no existe el equipo 0
                break;

                //rogue
            case 1:
                row1 = new int[11] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                row2 = new int[11] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                row3 = new int[11] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
                if (!p2)
                {
                    
                    for (int i = 0; i < 11; i++)
                    {
                        tiles[i,0].GetComponent<Tile>().identity = row1[i];
                    }
                    for (int i = 0; i < 11; i++)
                    {
                        tiles[i, 1].GetComponent<Tile>().identity = row2[i];
                    }
                    for (int i = 0; i < 11; i++)
                    {
                        tiles[i, 2].GetComponent<Tile>().identity = row3[i];
                    }


                }
                else
                {
                    for (int i = 0; i < 11; i++)
                    {
                        tiles[i, 10].GetComponent<Tile>().identity = row1[i];
                    }
                    for (int i = 0; i < 11; i++)
                    {
                        tiles[i, 9].GetComponent<Tile>().identity = row2[i];
                    }
                    for (int i = 0; i < 11; i++)
                    {
                        tiles[i, 8].GetComponent<Tile>().identity = row3[i];
                    }
                }
                break;
        }
    }
}
