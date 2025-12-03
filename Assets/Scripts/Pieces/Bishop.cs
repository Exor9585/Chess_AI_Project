using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public override List<Vector2Int> GetAvailableMoves(Piece[,] board)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int x = position.x;
        int y = position.y;

        
        int[] dx = { 1, 1, -1, -1 };
        int[] dy = { 1, -1, 1, -1 };

        for (int dir = 0; dir < 4; dir++)
        {
            int tx = x;
            int ty = y;

            while (true)
            {
                tx += dx[dir];
                ty += dy[dir];

                if (tx < 0 || tx >= 8 || ty < 0 || ty >= 8)
                    break;

                Piece target = board[tx, ty];

                if (target == null)
                {
                    moves.Add(new Vector2Int(tx, ty));
                }
                else
                {
                    if (target.isWhite != isWhite)
                        moves.Add(new Vector2Int(tx, ty));
                    break; 
                }
            }
        }

        return moves;
    }

}
