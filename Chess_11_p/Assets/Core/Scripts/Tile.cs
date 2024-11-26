using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("information")]
    public int team;
    public int identity;
    public bool p1DiedHere;
    public bool p2DiedHere;
    public bool isHighlighted = false;
    public bool portal = false;
    //secondary color
    public bool p2;
    public bool hasMoved = false;
    public int x;
    public int y;
    public List<Vector2Int> legalMoves = new List<Vector2Int>();
    [Header("References")]
    public GenerateBoard board;
    [Header("Actual Piece")]
    public GameObject currentPiece;
    public Vector3 OffsetPos;
    public Quaternion OffsetRotation;
    [Header("Pieces Models")]
    public Mesh[] PiecesModels;
    [Header("Pieces Materials")]
    public Material[] PiecesMaterials;
    [Header("Status Particles")]
    public GameObject currentStatusParticle;
    public GameObject[] statusParticlesPrefabs;
    [Header("Animation Particles")]
    public GameObject currentAnimationParticle;
    public GameObject[] AnimationParticlesPrefabs;
    

    
    // 0 = EMPTY
    // 1 = PAWN
    // 2 = ROOK
    // 3 = KNIGHT
    // 4 = BISHOP
    // 5 = QUEEN
    // 6 = KING
    // 7 = ROGUE
    // 8 = NECROMANCER
    // 9 = ELEMENTALIST
    // 10 = ZOMBIE

    private void Start()
    {
        p1DiedHere = false;
        p2DiedHere = false;
        LegalMovesAssign();
        SpawnPiece();
        ChangePiece();
        
    }
    

    
    private void SpawnPiece()
    {
        // Crear un nuevo GameObject y asignarlo a currentPiece
        currentPiece = new GameObject("Piece");
        currentPiece.transform.SetParent(transform); // Hacerlo hijo del objeto actual
        currentPiece.transform.localPosition = Vector3.zero; // Hereda la posición del padre
        currentPiece.transform.localRotation = Quaternion.identity; // Hereda la rotación del padre
        //currentPiece.transform.localScale = new Vector3(90f, 10f, 90f);
        currentPiece.AddComponent<MeshFilter>();     // Agregar el componente MeshFilter
        currentPiece.AddComponent<MeshRenderer>();   // Agregar el componente MeshRenderer

        
    }

    

    public void ChangePiece( )
    {
        //CAMBIAR EL Modelo
        MeshFilter meshFilter = currentPiece.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = currentPiece.GetComponent<MeshRenderer>();
        meshFilter.mesh = PiecesModels[identity];
        meshRenderer.material = PiecesMaterials[team];
        //arreglar rotacion
        if (!p2)
        {
            currentPiece.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            currentPiece.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void LegalMovesAssign()
    {
        legalMoves.Clear();
        Vector2Int pos = new Vector2Int(x, y);

        switch (identity)
        {
            case 0: // Casilla vacía
                break;

            case 1: // Peón
                int moveDirection = p2 ? -1 : 1; // Dirección según el equipo

                // Movimiento hacia adelante
                Vector2Int forwardMove = pos + new Vector2Int(0, moveDirection);
                if (IsValidMove(forwardMove) && board.tiles[forwardMove.x, forwardMove.y].GetComponent<Tile>().identity == 0)
                {
                    legalMoves.Add(forwardMove);

                    // Movimiento inicial de dos casillas
                    if (!hasMoved)
                    {
                        Vector2Int doubleForwardMove = pos + new Vector2Int(0, 2 * moveDirection);
                        if (IsValidMove(doubleForwardMove) && board.tiles[doubleForwardMove.x, doubleForwardMove.y].GetComponent<Tile>().identity == 0)
                        {
                            legalMoves.Add(doubleForwardMove);
                        }
                    }
                }

                // Capturas diagonales
                Vector2Int[] diagonalCaptures = {
                new Vector2Int(1, moveDirection),  // Diagonal derecha
                new Vector2Int(-1, moveDirection)  // Diagonal izquierda
            };

                foreach (Vector2Int captureMove in diagonalCaptures)
                {
                    Vector2Int capturePos = pos + captureMove;
                    if (IsValidMove(capturePos))
                    {
                        Tile targetTile = board.tiles[capturePos.x, capturePos.y].GetComponent<Tile>();
                        if (targetTile.identity != 0 && targetTile.p2 != p2)
                        {
                            legalMoves.Add(capturePos);
                        }
                    }
                }
                break;

            case 2: // Torre
                AddLinearMoves(pos, legalMoves, new Vector2Int(1, 0));  // Derecha
                AddLinearMoves(pos, legalMoves, new Vector2Int(-1, 0)); // Izquierda
                AddLinearMoves(pos, legalMoves, new Vector2Int(0, 1));  // Arriba
                AddLinearMoves(pos, legalMoves, new Vector2Int(0, -1)); // Abajo
                break;

            case 3: // Caballo
                Vector2Int[] knightMoves = {
                new Vector2Int(2, 1), new Vector2Int(2, -1),
                new Vector2Int(-2, 1), new Vector2Int(-2, -1),
                new Vector2Int(1, 2), new Vector2Int(1, -2),
                new Vector2Int(-1, 2), new Vector2Int(-1, -2)
            };

                foreach (Vector2Int move in knightMoves)
                {
                    AddMovesIfValid(pos + move, legalMoves);
                }
                break;

            case 4: // Alfil
                AddLinearMoves(pos, legalMoves, new Vector2Int(1, 1));  // Diagonal superior derecha
                AddLinearMoves(pos, legalMoves, new Vector2Int(-1, 1)); // Diagonal superior izquierda
                AddLinearMoves(pos, legalMoves, new Vector2Int(1, -1)); // Diagonal inferior derecha
                AddLinearMoves(pos, legalMoves, new Vector2Int(-1, -1)); // Diagonal inferior izquierda
                break;

            case 5: // Reina
                AddLinearMoves(pos, legalMoves, new Vector2Int(1, 0));  // Derecha
                AddLinearMoves(pos, legalMoves, new Vector2Int(-1, 0)); // Izquierda
                AddLinearMoves(pos, legalMoves, new Vector2Int(0, 1));  // Arriba
                AddLinearMoves(pos, legalMoves, new Vector2Int(0, -1)); // Abajo
                AddLinearMoves(pos, legalMoves, new Vector2Int(1, 1));  // Diagonal superior derecha
                AddLinearMoves(pos, legalMoves, new Vector2Int(-1, 1)); // Diagonal superior izquierda
                AddLinearMoves(pos, legalMoves, new Vector2Int(1, -1)); // Diagonal inferior derecha
                AddLinearMoves(pos, legalMoves, new Vector2Int(-1, -1)); // Diagonal inferior izquierda
                break;

            case 6: // Rey
                Vector2Int[] kingMoves = {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1),
                new Vector2Int(1, 1), new Vector2Int(-1, 1),
                new Vector2Int(1, -1), new Vector2Int(-1, -1)
            };

                foreach (Vector2Int move in kingMoves)
                {
                    AddMovesIfValid(pos + move, legalMoves);
                }
                break;

            case 7: // Rogue
                Vector2Int[] rogueMoves = {
                new Vector2Int(3, 0), new Vector2Int(-3, 0), // Movimiento horizontal
                new Vector2Int(0, 3), new Vector2Int(0, -3),  // Movimiento vertical
                new Vector2Int(2,2), new Vector2Int(-2, 2), 
                new Vector2Int(-2, -2),new Vector2Int(2, -2)
            };

                foreach (Vector2Int move in rogueMoves)
                {
                    AddMovesIfValid(pos + move, legalMoves);
                }

                // Capturas de Rogue (como peón)
                Vector2Int[] rogueCaptures = {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1)
            };

                foreach (Vector2Int captureMove in rogueCaptures)
                {
                    Vector2Int capturePos = pos + captureMove;
                    if (IsValidMove(capturePos))
                    {
                        Tile targetTile = board.tiles[capturePos.x, capturePos.y].GetComponent<Tile>();
                        if (targetTile.identity != 0 && targetTile.p2 != p2)
                        {
                            legalMoves.Add(capturePos);
                        }
                    }
                }
                break;

            case 8: // Necromancer
                int NecromoveDirection = p2 ? 1 : -1; // Dirección según el equipo
                int NecromoveDirectionBack = p2 ? -1 : 1;
                Vector2Int[] necroMoves = {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),  // Movimiento horizontal
                new Vector2Int(0, NecromoveDirection), new Vector2Int(-1, NecromoveDirection), new Vector2Int(1, NecromoveDirection) // Movimiento hacia atrás
            };

                foreach (Vector2Int move in necroMoves)
                {
                    AddMovesIfValid(pos + move, legalMoves);
                }

                // Capturas de Necromancer (solo hacia adelante)
                Vector2Int[] necroCaptures = {
                new Vector2Int(0, NecromoveDirectionBack), new Vector2Int(1, NecromoveDirectionBack), new Vector2Int(-1, NecromoveDirectionBack) // Captura hacia adelante
            };

                foreach (Vector2Int captureMove in necroCaptures)
                {
                    Vector2Int capturePos = pos + captureMove;
                    if (IsValidMove(capturePos))
                    {
                        Tile targetTile = board.tiles[capturePos.x, capturePos.y].GetComponent<Tile>();
                        if (targetTile.identity != 0 && targetTile.p2 != p2)
                        {
                            legalMoves.Add(capturePos);
                        }
                    }
                }
                break;

            case 9: // Elementalist


                Vector2Int[] wizCaptures = {
                new Vector2Int(4, 4), new Vector2Int(-4, 4),
                new Vector2Int(4, -4), new Vector2Int(-4, -4)
            };

                foreach (Vector2Int captureMove in wizCaptures)
                {
                    Vector2Int capturePos = pos + captureMove;
                    if (IsValidMove(capturePos))
                    {
                        Tile targetTile = board.tiles[capturePos.x, capturePos.y].GetComponent<Tile>();
                        if (targetTile.identity != 0 && targetTile.p2 != p2)
                        {
                            legalMoves.Add(capturePos);
                        }
                    }
                }
                break;
            case 10: // Zombie
                Vector2Int[] zombieMoves = {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1),
                new Vector2Int(1, 1), new Vector2Int(-1, 1),
                new Vector2Int(1, -1), new Vector2Int(-1, -1)
            };

                foreach (Vector2Int move in zombieMoves)
                {
                    AddMovesIfValid(pos + move, legalMoves);
                }
                break;
        }
    }

    // Método para añadir movimientos lineales (recto o diagonal) mientras no se encuentren otras piezas
    private void AddLinearMoves(Vector2Int startPos, List<Vector2Int> moves, Vector2Int direction)
    {
        Vector2Int currentPos = startPos + direction;

        while (IsValidMove(currentPos))
        {
            Tile tile = board.tiles[currentPos.x, currentPos.y].GetComponent<Tile>();

            if (tile.identity != 0) // Hay una pieza en la posición
            {
                if (tile.p2 != p2) // Es una pieza enemiga
                {
                    moves.Add(currentPos);
                }
                break; // Detiene el movimiento si hay una pieza
            }

            moves.Add(currentPos); // Posición vacía
            currentPos += direction; // Avanza a la siguiente casilla en la dirección
        }
    }

    // Método para añadir un movimiento si es válido (si está en el tablero y es capturable o vacío)
    private void AddMovesIfValid(Vector2Int pos, List<Vector2Int> moves)
    {
        if (IsValidMove(pos))
        {
            Tile tile = board.tiles[pos.x, pos.y].GetComponent<Tile>();

            // Añade el movimiento solo si la casilla está vacía o tiene una pieza enemiga
            if (tile.identity == 0 || tile.p2 != p2)
            {
                moves.Add(pos);
            }
        }
    }

    // Método auxiliar para verificar si una posición está dentro de los límites del tablero de 11x11
    private bool IsValidMove(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 11 && pos.y >= 0 && pos.y < 11;
    }
    private void AddLinearCaptures(Vector2Int startPos, List<Vector2Int> moves, Vector2Int direction)
    {
        Vector2Int currentPos = startPos + direction;

        // Recorre las casillas en la dirección de captura hasta que se encuentre una pieza enemiga o se salga del tablero
        if (IsValidMove(currentPos))
        {
            Tile tile = board.tiles[currentPos.x, currentPos.y].GetComponent<Tile>();

            if (tile.identity != 0 && tile.p2 != p2) // Si hay una pieza enemiga
            {
                moves.Add(currentPos); // Se añade la posición de captura
            }
        }
    }
    public void ParticleStatus(int team)
    {
        
        
        switch(team)
        {
            case 0:
                
                break;
            case 1:
                currentStatusParticle = Instantiate(statusParticlesPrefabs[0],transform);
                break;
            case 2:
                currentStatusParticle = Instantiate(statusParticlesPrefabs[1], transform);
                break;
        }

    }
    

    public void clearParticles(int index)
    {
        switch(index)
        {
            case 0:
                Destroy(currentStatusParticle);
                
                break;
            case 1:
                Destroy(currentAnimationParticle);
                
                break;
            case 2:
                
                
                break;
        }
    }
    public void SpawnWizAnimation(int index)
    {
       currentAnimationParticle = Instantiate(AnimationParticlesPrefabs[index], transform);
    }
    public void DeSpawnWizAnimation()
    {
        Destroy(currentAnimationParticle);
    }
}
