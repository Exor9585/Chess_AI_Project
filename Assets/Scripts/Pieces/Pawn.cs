using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override List<Vector2Int> GetAvailableMoves(Piece[,] board)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int direction = isWhite ? 1 : -1;
        int x = position.x;
        int y = position.y;

        bool InBounds(int bx, int by) => bx >= 0 && bx < 8 && by >= 0 && by < 8;

        // Előre
        if (InBounds(x, y + direction) && board[x, y + direction] == null)
            moves.Add(new Vector2Int(x, y + direction));

        // Első dupla lépés
        if ((isWhite && y == 1) || (!isWhite && y == 6))
        {
            if (board[x, y + direction] == null && board[x, y + 2 * direction] == null)
                moves.Add(new Vector2Int(x, y + 2 * direction));
        }

        // Ütés átlósan
        int[] dx = { -1, 1 };
        foreach (int dxi in dx)
        {
            int tx = x + dxi;
            int ty = y + direction;
            if (InBounds(tx, ty))
            {
                Piece target = board[tx, ty];
                if (target != null && target.isWhite != isWhite)
                    moves.Add(new Vector2Int(tx, ty));
            }
        }

        return moves;
    }

}
