using System;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoPiezas : MonoBehaviour
{
    [Header("References")]
    public GenerateBoard board; // Referencia al script GenerateBoard
    public Material baseMaterial;
    public Material hoverMaterial;
    public Material clickMaterial;
    public GameObject chessBoard;
    public bool p1Turn;
    public bool usingAbility;

    private Tile selectedTile = null; // Casilla actualmente seleccionada
    private List<Tile> highlightedTiles = new List<Tile>(); // Movimientos legales resaltados
    private Camera mainCamera;
    public float rotationSpeed = 90f;
    public int abilityIndex;
    

    private void Awake()
    {
        usingAbility = false;
        p1Turn = true;
        
    }
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleHover();
        HandleClick();
    }
    public void StartSmoothRotateY180()
    {
        StartCoroutine(RotateY180Smoothly());
    }
    private System.Collections.IEnumerator RotateY180Smoothly()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 180f, 0f);

        float elapsedTime = 0f;
        float duration = 180f / rotationSpeed; // Calculate duration based on speed

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            chessBoard.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
            yield return null;
        }

        transform.rotation = endRotation; // Ensure exact final rotation
    }

    private void HandleHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null && !tile.isHighlighted) // Hover solo si no está resaltada
            {
                ResetAllTiles(); // Restablecer otras casillas al material base
                tile.GetComponent<Renderer>().material = hoverMaterial;
            }
        }
        else
        {
            ResetAllTiles();
        }
    }

    private void HandleClick()
    {
        if (Input.GetMouseButtonDown(0)) // Botón izquierdo
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null)
                {
                    if (tile.isHighlighted && usingAbility)
                    {
                        UseAbility(tile);
                        if (board.p1Cd > 0) { board.p1Cd--; }
                        if (board.p2Cd > 0) { board.p2Cd--; }
                        if (p1Turn)
                        {
                            board.p1Cd = board.cDrogue;
                            p1Turn = false;

                        }
                        else 
                        {
                            board.AssignCoolDowns(board.p1T,p1Turn);
                            p1Turn = true; 
                        }
                        
                        StartSmoothRotateY180();
                    }
                    if (selectedTile == null && tile.identity != 0) // Seleccionar una pieza
                    {
                        if(!tile.p2 && p1Turn)
                        {
                            SelectTile(tile);
                        }
                        if (tile.p2 && !p1Turn)
                        {
                            SelectTile(tile);
                        }
                        
                    }
                    
                    else if (tile.isHighlighted && !usingAbility) // Mover pieza
                    {
                        MovePiece(tile);
                        if(p1Turn)
                        {
                            p1Turn = false;
                        }
                        else { p1Turn = true; }
                        if (board.p1Cd > 0) { board.p1Cd--; }
                        if (board.p2Cd > 0) { board.p2Cd--; }
                        StartSmoothRotateY180();
                    }
                    
                    else
                    {
                        // Deseleccionar si se hace clic en un lugar no válido
                        DeselectTile();
                    }
                }
            }
        }
    }

    public void ChangeTurn()
    {

    }
    public void AbilityButton(bool p2)
    {
        ResetAllTiles();
        ClearHighlights();
        bool canCast = false;

        if (p1Turn != p2 )
        {
            usingAbility = true;

            int playerTeam;
            if (!p2)
            {
                playerTeam = board.p1T;
                if(board.p1Cd == 0)
                {
                    canCast = true;
                }
                
            }
            else
            {
                playerTeam = board.p2T;
                if (board.p2Cd == 0)
                {
                    canCast = true;
                }
            }

            if(canCast)
            {
                switch (playerTeam)
                {
                    //rogue
                    case 1 or 4:
                        for (int i = 0; i < 11; i++)
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                if (board.tiles[i, j].GetComponent<Tile>().p2 != p2 && board.tiles[i, j].GetComponent<Tile>().identity != 6)
                                {
                                    if (board.tiles[i, j].GetComponent<Tile>().identity != 0)
                                    {
                                        board.tiles[i, j].GetComponent<Renderer>().material = clickMaterial;
                                        board.tiles[i, j].GetComponent<Tile>().isHighlighted = true;
                                        highlightedTiles.Add(board.tiles[i, j].GetComponent<Tile>());
                                    }

                                }
                            }
                        }
                        abilityIndex = 1;

                        break;
                        //necromancer
                        //elementalist
                }
            }
            
        }
       
        

    }
    public void UseAbility(Tile targetTile)
    {
        usingAbility = false;
        switch(abilityIndex)
        {
            //rogue cast
            case 1:
                targetTile.identity = 0;
                UpdateAllTiles();
                DeselectTile();
                ClearHighlights();
                break;
        }
        
    }

    private void SelectTile(Tile tile)
    {
        usingAbility = false;
        selectedTile = tile;
        HighlightLegalMoves(tile); // Iluminar movimientos legales
    }

    private void DeselectTile()
    {
        selectedTile = null;
        ClearHighlights();
    }

    private void HighlightLegalMoves(Tile tile)
    {
        ClearHighlights(); // Limpiar cualquier resaltado previo
        List<Vector2Int> legalMoves = tile.legalMoves; // Obtener movimientos legales

        foreach (Vector2Int move in legalMoves)
        {
            Tile targetTile = board.tiles[move.x, move.y].GetComponent<Tile>(); // Usar board.tiles
            targetTile.GetComponent<Renderer>().material = clickMaterial;
            targetTile.isHighlighted = true;
            highlightedTiles.Add(targetTile);
        }
    }

    private void ClearHighlights()
    {
        foreach (Tile tile in highlightedTiles)
        {
            tile.GetComponent<Renderer>().material = baseMaterial;
            tile.isHighlighted = false;
        }
        highlightedTiles.Clear();
    }

    private void MovePiece(Tile targetTile)
    {
        // Transferir datos de la casilla seleccionada a la nueva
        if(targetTile.identity == 6) //detectar quien gano
        {
            if(p1Turn)
            {
                Debug.Log("gano jugador uno");
            }
            else
            {
                Debug.Log("gano jugador 2");
            }
        }
        targetTile.team = selectedTile.team;
        targetTile.identity = selectedTile.identity;
        targetTile.p2 = selectedTile.p2;
        //limitar al peon
        targetTile.hasMoved = true;
        //registrar casillas con piezas muertas para el necromancer
        if (targetTile.identity != 0 && targetTile.identity != 1)
        {
            if(!targetTile.p2DiedHere && !targetTile.p1DiedHere)
            {
                if (targetTile.p2)
                {
                    targetTile.p2DiedHere = true;
                }
                else
                {
                    targetTile.p1DiedHere = true;
                }
            }
        }


        // Limpiar la casilla original
        selectedTile.team = 0;
        selectedTile.identity = 0;
        selectedTile.p2 = false;
        // Actualizar las piezas visualmente
        UpdateAllTiles();

        
        

        // Limpiar selección
        DeselectTile();
        ClearHighlights();
    }

    private void UpdateAllTiles()
    {
        for(int i = 0; i < 11; i++ )
        {
            for (int j = 0; j < 11; j++)
            {
                board.tiles[i, j].GetComponent<Tile>().ChangePiece();
                board.tiles[i, j].GetComponent<Tile>().LegalMovesAssign();
            }
        }
    }

    private void ResetAllTiles()
    {
        foreach (GameObject tileObj in board.tiles) // Usar board.tiles
        {
            Tile tile = tileObj.GetComponent<Tile>();
            if (!tile.isHighlighted)
                tile.GetComponent<Renderer>().material = baseMaterial;
        }
    }
}