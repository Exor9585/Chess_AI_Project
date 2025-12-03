using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class MoveValidator
{

    
    public static bool IsLegalMove(Vector2Int from, Vector2Int to, GameState state)
    {
        
        if (!InBounds(from) || !InBounds(to)) 
            return false;

        PieceData moving = state.board[from.x, from.y];
        if (!moving.exists) 
            return false; 

        
        PieceData target = state.board[to.x, to.y];
        if (target.exists && target.isWhite == moving.isWhite)
            return false;

        if (moving.type == "King")
        {
            
            if (IsKingCastlingMove(from, to, state))
                return true;
        }

        
        if (!IsDestinationReachableByPieceType(moving, from, to, state))
            return false;

        
        GameState copy = state.DeepCopy();
        copy.ApplyMove(from, to);

        PieceData? king = copy.FindKingData(moving.isWhite);
        if (!king.HasValue) return false; 
        if (IsKingInCheck(king.Value, copy))
            return false;

        return true;
    }

    static bool InBounds(Vector2Int p) => p.x >= 0 && p.x < 8 && p.y >= 0 && p.y < 8;


    static bool IsDestinationReachableByPieceType(PieceData piece, Vector2Int from, Vector2Int to, GameState state)
    {
        string t = piece.type;
        int dx = to.x - from.x;
        int dy = to.y - from.y;

        if (t == "Pawn")
        {
            int dir = piece.isWhite ? 1 : -1;
            
            if (dx == 0 && dy == dir && !state.board[to.x, to.y].exists) return true;
            
            if (dx == 0 && dy == 2 * dir)
            {
                int startRow = piece.isWhite ? 1 : 6;
                if (from.y == startRow && !state.board[from.x, from.y + dir].exists && !state.board[to.x, to.y].exists)
                    return true;
            }
            
            if ((dx == 1 || dx == -1) && dy == dir && state.board[to.x, to.y].exists && state.board[to.x, to.y].isWhite != piece.isWhite)
                return true;

            return false;
        }
        if (t == "Rook")
        {
            if (dx != 0 && dy != 0) return false;
            return IsPathClear(from, to, state);
        }
        if (t == "Bishop")
        {
            if (Mathf.Abs(dx) != Mathf.Abs(dy)) return false;
            return IsPathClear(from, to, state);
        }
        if (t == "Queen")
        {
            if (dx == 0 || dy == 0 || Mathf.Abs(dx) == Mathf.Abs(dy))
                return IsPathClear(from, to, state);
            return false;
        }
        if (t == "Knight")
        {
            int adx = Mathf.Abs(dx), ady = Mathf.Abs(dy);
            return (adx == 1 && ady == 2) || (adx == 2 && ady == 1);
        }
        if (t == "King")
        {
            if (Mathf.Abs(dx) <= 1 && Mathf.Abs(dy) <= 1) return true;
            
            return false;
        }

        return false; 
    }

    
    static bool IsPathClear(Vector2Int from, Vector2Int to, GameState state)
    {
        int dx = to.x - from.x;
        int dy = to.y - from.y;

        int stepX = dx == 0 ? 0 : (dx / Mathf.Abs(dx));
        int stepY = dy == 0 ? 0 : (dy / Mathf.Abs(dy));

        int cx = from.x + stepX;
        int cy = from.y + stepY;

        
        while (cx != to.x || cy != to.y)
        {
            if (state.board[cx, cy].exists) return false;
            cx += stepX;
            cy += stepY;
        }

        return true;
    }

    
    public static bool IsKingInCheck(PieceData king, GameState state)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                PieceData p = state.board[x, y];
                if (!p.exists || p.isWhite == king.isWhite) continue;
                if (IsDestinationReachableByPieceType(p, new Vector2Int(x, y), king.Pos, state))
                    return true;
            }
        }
        return false;
    }

    public static bool IsKingCastlingMove(Vector2Int from, Vector2Int to, GameState state)
    {
        int dx = to.x - from.x;
        int dy = to.y - from.y;
        if (dy != 0) return false;
        if (Mathf.Abs(dx) != 2) return false; 

        PieceData king = state.board[from.x, from.y];
        if (!king.exists || king.type != "King" || king.hasMoved) return false;

        int rookX = dx > 0 ? 7 : 0;
        PieceData rook = state.board[rookX, from.y];
        if (!rook.exists || rook.type != "Rook" || rook.isWhite != king.isWhite || rook.hasMoved) return false;

        
        int step = dx > 0 ? 1 : -1;
        for (int x = from.x + step; x != rookX; x += step)
        {
            if (state.board[x, from.y].exists) return false;
        }

        
        if (IsKingInCheck(king, state)) return false;

        
        Vector2Int middle = new Vector2Int(from.x + step, from.y);
        Vector2Int dest = to;

        
        GameState sim = state.DeepCopy();

        
        sim.board[from.x, from.y] = new PieceData(false, false, null, from.x, from.y); // üresíteni
        sim.board[middle.x, middle.y] = new PieceData(true, king.isWhite, "King", middle.x, middle.y);
        if (IsKingInCheck(sim.FindKingData(king.isWhite).Value, sim)) return false;

        
        sim = state.DeepCopy();
        sim.board[from.x, from.y] = new PieceData(false, false, null, from.x, from.y);
        sim.board[dest.x, dest.y] = new PieceData(true, king.isWhite, "King", dest.x, dest.y);
        if (IsKingInCheck(sim.FindKingData(king.isWhite).Value, sim)) return false;

        return true;
    }

}