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
    

    [Header("Materials")]
    public Material defaultMaterial;
    public Material highlightMaterial;
    public Material clickMaterial;
    public Material wizardPortal;

    [Header("Scripts")]
    public GenerateBoard gB;
    public MovimientoPiezas mP;

    public bool mirror = false;
    public int cDrogue;
    public int cDnecro;
    public int cDelem;
    public int p1Cd;
    public int p2Cd;

    private void Awake()
    {
        p1T = GameData.p1T;
        p2T = GameData.p2T;
        tiles = new GameObject[TilesX, TilesY];
        GenerateAllTiles();
        AssignCoolDowns(p1T, true);
        AssignCoolDowns(p2T, false);
        if (p1T == p2T)
        {
            //maneja el mirror match
            p2T += 3;
        }
        AssignPieces(p1T, false);
        AssignPieces(p2T, true);
    }
    
    public void AssignCoolDowns(int T, bool p1)
    {
        if (p1)
        {
            switch (T)
            {
                case 1 or 4:
                    p1Cd = cDrogue;
                    break;
                case 2 or 5:
                    p1Cd = cDnecro;
                    break;
                case 3 or 6:
                    p1Cd = cDelem;
                    break;
            }
        }
        else
        {
            switch (T)
            {
                case 1 or 4:
                    p2Cd = cDrogue;
                    break;
                case 2 or 5:
                    p2Cd = cDnecro;
                    break;
                case 3 or 6:
                    p2Cd = cDelem;
                    break;
            }
        }

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
                
                //row1 = new int[11] { 1, 3, 2, 3, 5, 3, 6, 3, 2, 3, 1 };
                //row2 = new int[11] { 0, 1, 1, 2, 1, 7, 1, 2, 1, 1, 0 };
                //row3 = new int[11] { 0, 3, 3, 5, 3, 6, 3, 6, 5, 3, 0 };

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
