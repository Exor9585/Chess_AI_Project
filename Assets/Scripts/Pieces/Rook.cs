using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    public bool hasMoved = false;

    public override List<Vector2Int> GetAvailableMoves(Piece[,] board)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int dir = 0; dir < 4; dir++)
        {
            int x = position.x;
            int y = position.y;
            while (true)
            {
                x += dx[dir];
                y += dy[dir];
                if (x < 0 || x >= 8 || y < 0 || y >= 8) break;
                if (board[x, y] == null)
                    moves.Add(new Vector2Int(x, y));
                else
                {
                    if (board[x, y].isWhite != isWhite)
                        moves.Add(new Vector2Int(x, y));
                    break;
                }
            }
        }

        return moves;
    }

}
