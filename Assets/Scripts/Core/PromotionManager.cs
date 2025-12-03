using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionManager : MonoBehaviour
{
    public Board board; 

    
    public void PromotePawn(Vector2Int pos, bool isWhite)
    {
       
        if (board.squares[pos.x, pos.y] != null)
        {
            Destroy(board.squares[pos.x, pos.y].gameObject);
            board.squares[pos.x, pos.y] = null;
        }

        
        Sprite sprite = board.GetSpriteFor("Queen", isWhite);
        board.SpawnPiece(pos, isWhite, sprite, "Queen");

        
        board.gameState.board[pos.x, pos.y] = new PieceData(true, isWhite, "Queen", pos.x, pos.y)
        {
            hasMoved = true
        };
    }
}
