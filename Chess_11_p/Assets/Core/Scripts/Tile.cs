using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("information")]
    public int identity;
    //secondary color
    public bool p2;
    public bool hasMoved = false;
    public int x;
    public int y;
    public List<Vector2Int> legalMoves = new List<Vector2Int>();

    public GenerateBoard board;
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
        LegalMovesAssign(identity, x, y);
    }
    private void Update()
    {
        ChangePiece(identity, p2);
        
    }

    private void ChangePiece(int id, bool team)
    {
        switch(id)
        {
            case 0:
                //CAMBIAR EL SPRITE
                if(team)
                {
                    //jugador2
                }
                else
                {
                    //jugador1
                }

                break;
            
            case 1:

                break;
           
            case 2:

                break;
            
            case 3:

                break;
            
            case 4:

                break;

            case 5:

                break;

            case 6:

                break;
            
            case 7:

                break;
            
            case 8:

                break;
            
            case 9:

                break;
           
            case 10:

                break;
        }
    }

    private void LegalMovesAssign(int id, int x, int y)
    {
        
        Vector2Int pos = new Vector2Int(x, y);

        switch (id)
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

                // Verifica capturas en las casillas diagonales
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

                // Agrega cada movimiento posible del caballo si es válido
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
                    // Movimientos de torre
                AddLinearMoves(pos, legalMoves, new Vector2Int(1, 0));  // Derecha
                AddLinearMoves(pos, legalMoves, new Vector2Int(-1, 0)); // Izquierda
                AddLinearMoves(pos, legalMoves, new Vector2Int(0, 1));  // Arriba
                AddLinearMoves(pos, legalMoves, new Vector2Int(0, -1)); // Abajo
                                                                        // Movimientos de alfil
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

                // Agrega cada movimiento posible del rey si es válido
                foreach (Vector2Int move in kingMoves)
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

}
