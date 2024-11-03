using UnityEngine;

public class GenerateBoard : MonoBehaviour
{
    [Header("ints")]
    public int TilesX; 
    public int TilesY;
    [SerializeField]public int[,] FinalBoard;
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
        FinalBoard = new int[11, 11];
        for (int x = 0; x < TilesX; x++)
        {
            for (int y = 0; y < TilesY; y++)
            {
                FinalBoard[x,y] = 0;
            }
        }
        tiles = new GameObject[TilesX, TilesY];
        GenerateAllTiles();
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
        UpdateBoard();
    }
    private void UpdateBoard()
    {
        for(int i = 0; i > TilesX; i++)
        {
            for(int j = 0; j > TilesY; j++)
            {
                Tile tilE = tiles[i, j].GetComponent<Tile>();
                FinalBoard[i, j] = tilE.identity;
            }
        }
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

    private void AssignPiece(int team, bool p2)   
    {
        
    }
}
