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
    public bool p1T;

    private Tile selectedTile = null; // Casilla actualmente seleccionada
    private List<Tile> highlightedTiles = new List<Tile>(); // Movimientos legales resaltados
    private Camera mainCamera;
    public float rotationSpeed = 90f;

    private void Awake()
    {
        p1T = true;
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
                    if (selectedTile == null && tile.identity != 0) // Seleccionar una pieza
                    {
                        if(!tile.p2 && p1T)
                        {
                            SelectTile(tile);
                        }
                        if (tile.p2 && !p1T)
                        {
                            SelectTile(tile);
                        }
                        
                    }
                    else if (tile.isHighlighted) // Mover pieza
                    {
                        MovePiece(tile);
                        if(p1T)
                        {
                            p1T = false;
                        }
                        else { p1T = true; }
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

    private void SelectTile(Tile tile)
    {
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
        targetTile.team = selectedTile.team;
        targetTile.identity = selectedTile.identity;
        targetTile.p2 = selectedTile.p2;
        

        // Limpiar la casilla original
        selectedTile.team = 0;
        selectedTile.identity = 0;
        selectedTile.p2 = false;
        // Actualizar las piezas visualmente
        UpdateAllTiles();

        
        

        // Limpiar selección
        DeselectTile();
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