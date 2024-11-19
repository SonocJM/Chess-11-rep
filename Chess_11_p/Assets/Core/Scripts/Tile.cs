using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("information")]
    public int team;
    public int identity;
    public bool isHighlighted = false;
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
        LegalMovesAssign();
        SpawnPiece(p2);
        ChangePiece();
    }
    public void UpdatePiece()
    {
        if (identity == 0 && currentPiece != null)
        {
            Destroy(currentPiece); // Borrar pieza si no hay identidad
        }
        else if (identity != 0 && currentPiece == null)
        {
            currentPiece = new GameObject("Piece");
            currentPiece.transform.SetParent(transform);
            currentPiece.transform.localPosition = Vector3.zero;
            currentPiece.transform.localScale = new Vector3(90f, 10f, 90f);
            currentPiece.AddComponent<MeshRenderer>();
        }
    }

    public List<Vector2Int> GetLegalMoves()
    {
        // L�gica para calcular movimientos legales
        return new List<Vector2Int>(); // Reemplazar con l�gica real
    }
    private void SpawnPiece(bool p2)
    {
        // Crear un nuevo GameObject y asignarlo a currentPiece
        currentPiece = new GameObject("Piece");
        currentPiece.transform.SetParent(transform); // Hacerlo hijo del objeto actual
        currentPiece.transform.localPosition = Vector3.zero; // Hereda la posici�n del padre
        currentPiece.transform.localRotation = Quaternion.identity; // Hereda la rotaci�n del padre
        currentPiece.transform.localScale = new Vector3(90f, 10f, 90f);
        currentPiece.AddComponent<MeshFilter>();     // Agregar el componente MeshFilter
        currentPiece.AddComponent<MeshRenderer>();   // Agregar el componente MeshRenderer

        // Aplicar rotaci�n si es necesario
        if (p2)
        {
            currentPiece.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    

    public void ChangePiece( )
    {
        //CAMBIAR EL Modelo
        MeshFilter meshFilter = currentPiece.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = currentPiece.GetComponent<MeshRenderer>();
        meshFilter.mesh = PiecesModels[identity];
        meshRenderer.material = PiecesMaterials[team];
    }

    public void LegalMovesAssign()
    {
        Vector2Int pos = new Vector2Int(x, y);

        switch (identity)
        {
            case 0: // Casilla vac�a
                break;

            case 1: // Pe�n
                int moveDirection = p2 ? -1 : 1; // Direcci�n seg�n el equipo

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
                new Vector2Int(0, 3), new Vector2Int(0, -3)  // Movimiento vertical
            };

                foreach (Vector2Int move in rogueMoves)
                {
                    AddMovesIfValid(pos + move, legalMoves);
                }

                // Capturas de Rogue (como pe�n)
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
                Vector2Int[] necroMoves = {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),  // Movimiento horizontal
                new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(1, -1) // Movimiento hacia atr�s
            };

                foreach (Vector2Int move in necroMoves)
                {
                    AddMovesIfValid(pos + move, legalMoves);
                }

                // Capturas de Necromancer (solo hacia adelante)
                Vector2Int[] necroCaptures = {
                new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 1) // Captura hacia adelante
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
                Vector2Int[] elementalMoves = {
                new Vector2Int(1, 0), new Vector2Int(-1, 0), // Movimiento horizontal y vertical
                new Vector2Int(0, 1), new Vector2Int(0, -1)
            };

                foreach (Vector2Int move in elementalMoves)
                {
                    AddMovesIfValid(pos + move, legalMoves);
                }

                // Capturas del Elementalist (como un alfil, en diagonal)
                Vector2Int[] elementalCaptures = {
                new Vector2Int(1, 1), new Vector2Int(-1, 1),
                new Vector2Int(1, -1), new Vector2Int(-1, -1)
            };

                foreach (Vector2Int captureMove in elementalCaptures)
                {
                    AddLinearCaptures(pos, legalMoves, captureMove);
                }
                break;
        }
    }

    // M�todo para a�adir movimientos lineales (recto o diagonal) mientras no se encuentren otras piezas
    private void AddLinearMoves(Vector2Int startPos, List<Vector2Int> moves, Vector2Int direction)
    {
        Vector2Int currentPos = startPos + direction;

        while (IsValidMove(currentPos))
        {
            Tile tile = board.tiles[currentPos.x, currentPos.y].GetComponent<Tile>();

            if (tile.identity != 0) // Hay una pieza en la posici�n
            {
                if (tile.p2 != p2) // Es una pieza enemiga
                {
                    moves.Add(currentPos);
                }
                break; // Detiene el movimiento si hay una pieza
            }

            moves.Add(currentPos); // Posici�n vac�a
            currentPos += direction; // Avanza a la siguiente casilla en la direcci�n
        }
    }

    // M�todo para a�adir un movimiento si es v�lido (si est� en el tablero y es capturable o vac�o)
    private void AddMovesIfValid(Vector2Int pos, List<Vector2Int> moves)
    {
        if (IsValidMove(pos))
        {
            Tile tile = board.tiles[pos.x, pos.y].GetComponent<Tile>();

            // A�ade el movimiento solo si la casilla est� vac�a o tiene una pieza enemiga
            if (tile.identity == 0 || tile.p2 != p2)
            {
                moves.Add(pos);
            }
        }
    }

    // M�todo auxiliar para verificar si una posici�n est� dentro de los l�mites del tablero de 11x11
    private bool IsValidMove(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < 11 && pos.y >= 0 && pos.y < 11;
    }
    private void AddLinearCaptures(Vector2Int startPos, List<Vector2Int> moves, Vector2Int direction)
    {
        Vector2Int currentPos = startPos + direction;

        // Recorre las casillas en la direcci�n de captura hasta que se encuentre una pieza enemiga o se salga del tablero
        if (IsValidMove(currentPos))
        {
            Tile tile = board.tiles[currentPos.x, currentPos.y].GetComponent<Tile>();

            if (tile.identity != 0 && tile.p2 != p2) // Si hay una pieza enemiga
            {
                moves.Add(currentPos); // Se a�ade la posici�n de captura
            }
        }
    }


}
