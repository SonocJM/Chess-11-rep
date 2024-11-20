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
    public GameObject[,] tiles;
    private GameObject lastHoveredTile;
    private GameObject clickedTile;

    [Header("Materials")]
    public Material defaultMaterial;
    public Material highlightMaterial;
    public Material clickMaterial;

    [Header("Scripts")]
    public GenerateBoard gB;

    public bool mirror = false;
    private bool isHoverEnabled = true;

    private void Awake()
    {
        p1T = GameData.p1T;
        p2T = GameData.p2T;
        tiles = new GameObject[TilesX, TilesY];
        GenerateAllTiles();
        if (p1T == p2T)
        {
            //maneja el mirror match
            p2T += 3;
        }
        AssignPieces(p1T, false);
        AssignPieces(p2T, true);
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
        int[] row1 = new int[11];
        int[] row2 = new int[11];
        int[] row3 = new int[11];



        switch (team)
        {
            case 0:
                //no existe el equipo 0
                break;

            //rogue
            case 1 or 4:
                row1 = new int[11] { 1, 3, 2, 3, 5, 3, 6, 3, 2, 3, 1 };
                row2 = new int[11] { 0, 1, 1, 2, 1, 7, 1, 2, 1, 1, 0 };
                row3 = new int[11] { 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0 };

                break;

            case 2 or 5:
                row1 = new int[11] { 1, 4, 2, 4, 5, 4, 6, 4, 2, 4, 1 };
                row2 = new int[11] { 0, 1, 2, 1, 1, 8, 1, 1, 2, 1, 0 };
                row3 = new int[11] { 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0 };

                break;

            case 3 or 6:
                row1 = new int[11] { 1, 4, 3, 5, 4, 6, 4, 5, 3, 4, 1 };
                row2 = new int[11] { 0, 1, 1, 4, 3, 9, 3, 4, 1, 1, 0 };
                row3 = new int[11] { 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0 };

                break;
        }
        if (!p2)
        {

            for (int i = 0; i < 11; i++)
            {
                tiles[i, 0].GetComponent<Tile>().identity = row1[i];
                tiles[i, 0].GetComponent<Tile>().p2 = false;
                tiles[i, 0].GetComponent<Tile>().team = p1T;
            }
            for (int i = 0; i < 11; i++)
            {
                tiles[i, 1].GetComponent<Tile>().identity = row2[i];
                tiles[i, 1].GetComponent<Tile>().p2 = false;
                tiles[i, 1].GetComponent<Tile>().team = p1T;
            }
            for (int i = 0; i < 11; i++)
            {
                tiles[i, 2].GetComponent<Tile>().identity = row3[i];
                tiles[i, 2].GetComponent<Tile>().p2 = false;
                tiles[i, 2].GetComponent<Tile>().team = p1T;
            }


        }
        else
        {
            for (int i = 0; i < 11; i++)
            {
                tiles[i, 10].GetComponent<Tile>().identity = row1[i];
                tiles[i, 10].GetComponent<Tile>().p2 = true;
                tiles[i, 10].GetComponent<Tile>().team = p2T;
            }
            for (int i = 0; i < 11; i++)
            {
                tiles[i, 9].GetComponent<Tile>().identity = row2[i];
                tiles[i, 9].GetComponent<Tile>().p2 = true;
                tiles[i, 9].GetComponent<Tile>().team = p2T;
            }
            for (int i = 0; i < 11; i++)
            {
                tiles[i, 8].GetComponent<Tile>().identity = row3[i];
                tiles[i, 8].GetComponent<Tile>().p2 = true;
                tiles[i, 8].GetComponent<Tile>().team = p2T;

            }
        }

    }
    

}
