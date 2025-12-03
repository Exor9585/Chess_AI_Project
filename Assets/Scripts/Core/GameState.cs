using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct PieceData
{
    public bool exists;
    public bool isWhite;
    public string type;
    public int x;
    public int y;
    public bool hasMoved;

    public PieceData(bool exists, bool isWhite, string type, int x, int y)
    {
        this.exists = exists;
        this.isWhite = isWhite;
        this.type = type;
        this.x = x;
        this.y = y;
        this.hasMoved = false;
    }

    public Vector2Int Pos => new Vector2Int(x, y);
}


public class GameState
{
    public PieceData[,] board; // 8x8 logikai tábla

    public GameState()
    {
        board = new PieceData[8, 8];
        Clear();
    }

    public void Clear()
    {
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                board[i, j] = new PieceData(false, false, null, i, j);
    }

    public PieceData GetPiece(int x, int y)
    {
        return board[x, y];
    }

    public void SetPiece(int x, int y, PieceData data)
    {
        data.x = x;
        data.y = y;
        data.exists = true;
        board[x, y] = data;
    }

    public void RemovePiece(int x, int y)
    {
        board[x, y] = new PieceData(false, false, null, x, y);
    }

    
    public void ApplyMove(Vector2Int from, Vector2Int to)
    {
        PieceData moving = board[from.x, from.y];
        if (!moving.exists) return;

        
        PieceData moved = new PieceData(true, moving.isWhite, moving.type, to.x, to.y)
        {
            hasMoved = true
        };
        board[to.x, to.y] = moved;

        
        board[from.x, from.y] = new PieceData(false, false, null, from.x, from.y);

        
        if (moving.type == "King")
        {
            int dx = to.x - from.x;
            if (Mathf.Abs(dx) == 2)
            {
                int rookFromX = dx > 0 ? 7 : 0;
                int rookToX = dx > 0 ? to.x - 1 : to.x + 1;

                PieceData rook = board[rookFromX, from.y];
                if (rook.exists && rook.type == "Rook")
                {
                    board[rookToX, from.y] = new PieceData(true, rook.isWhite, "Rook", rookToX, from.y)
                    {
                        hasMoved = true
                    };
                    board[rookFromX, from.y] = new PieceData(false, false, null, rookFromX, from.y);
                }
            }
        }

        // promocio
        if (moving.type == "Pawn" && (to.y == 0 || to.y == 7))
        {
            PromotionManager pm = GameObject.FindObjectOfType<PromotionManager>();
            pm.PromotePawn(to, moving.isWhite);
        }
    }

    
    public GameState DeepCopy()
    {
        GameState copy = new GameState();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                PieceData pd = board[x, y];
                if (pd.exists)
                {
                    copy.board[x, y] = new PieceData(true, pd.isWhite, pd.type, pd.x, pd.y)
                    {
                        hasMoved = pd.hasMoved
                    };
                }
                else
                {
                    copy.board[x, y] = new PieceData(false, false, null, x, y);
                }
            }
        }
        return copy;
    }

    // megtalálja a király adatát egy színre
    public PieceData? FindKingData(bool isWhite)
    {
        for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
            {
                PieceData pd = board[x, y];
                if (pd.exists && pd.isWhite == isWhite && pd.type == "King")
                    return pd;
            }
        return null;
    }
}
