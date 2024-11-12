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
    public Vector2[] legalMoves;

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

    private void Awake()
    {
        
    }
    private void Update()
    {
        ChangePiece(identity, p2);
        LegalMovesAssign(identity,x,y);
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
        legalMoves = new Vector2[4];
        Vector2 pos = new Vector2(x,y);
        switch(id)
        {
            //PAwn
            case 0:
                  break;

            case 1:
                Vector2 eatDiagonallyRight = new Vector2(1, 1);
                Vector2 eatDiagonallyLeft = new Vector2(-1, 1);
                if (y + 1 < 12)
                {
                    legalMoves[0] = pos + new Vector2(0, 1);
                    if (!hasMoved && y + 2 < 11)
                    {
                        legalMoves[1] = pos + new Vector2(0, 2);
                    }
                    if (x + 1 < 11 && y + 1 < 11)
                    {
                        //if (board.FinalBoard[x + 1, y + 1] != 0)
                        {
                            legalMoves[2] = pos + eatDiagonallyRight;
                        }
                    }
                    if (x - 1 > 0 && y + 1 < 11)
                    {
                        //if (board.FinalBoard[x - 1, y + 1] != 0)
                        {
                            legalMoves[3] = pos + eatDiagonallyLeft;
                        }
                    }
                }
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
}
