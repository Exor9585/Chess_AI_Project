using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    public override List<Vector2Int> GetAvailableMoves(Piece[,] board)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int x = position.x;
        int y = position.y;

        int[] dx = { 1, 2, 2, 1, -1, -2, -2, -1 };
        int[] dy = { 2, 1, -1, -2, -2, -1, 1, 2 };

        for (int i = 0; i < 8; i++)
        {
            int tx = x + dx[i];
            int ty = y + dy[i];

            if (tx >= 0 && tx < 8 && ty >= 0 && ty < 8)
            {
                Piece target = board[tx, ty];
                if (target == null || target.isWhite != isWhite)
                    moves.Add(new Vector2Int(tx, ty));
            }
        }

        return moves;
    }

}
