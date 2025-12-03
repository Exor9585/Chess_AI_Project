using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    

    public override List<Vector2Int> GetAvailableMoves(Piece[,] board)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        int x = position.x;
        int y = position.y;

        
        int[] dx = { 1, -1, 0, 0, 1, 1, -1, -1 };
        int[] dy = { 0, 0, 1, -1, 1, -1, 1, -1 };

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

        // rokád logika
        if (!hasMoved)
        {
            // kisrokád 
            if (CanCastle(board, true))
                moves.Add(new Vector2Int(x + 2, y));

            // nagyrokád 
            if (CanCastle(board, false))
                moves.Add(new Vector2Int(x - 2, y));
        }

        return moves;
    }

    bool CanCastle(Piece[,] board, bool kingSide)
    {
        int y = position.y;
        if (kingSide)
        {
            // bastya ellenorzes
            Piece rook = board[7, y];
            if (rook is Rook && !((Rook)rook).hasMoved)
            {
                
                if (board[5, y] == null && board[6, y] == null)
                    return true;
            }
        }
        else
        {
            
            Piece rook = board[0, y];
            if (rook is Rook && !((Rook)rook).hasMoved)
            {
                if (board[1, y] == null && board[2, y] == null && board[3, y] == null)
                    return true;
            }
        }

        return false;
    }


    public bool IsInCheck(Piece[,] board)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece p = board[x, y];
                if (p != null && p.isWhite != this.isWhite)
                {
                    List<Vector2Int> moves = p.GetAvailableMoves(board);
                    foreach (Vector2Int move in moves)
                    {
                        if (move == this.position)
                            return true; 
                    }
                }
            }
        }
        return false;

    }



}
