using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessManager : MonoBehaviour
{
    public Board board;

    

    public void CheckGameState()
    {
        bool isWhiteTurn = board.whiteTurn;

        if (IsKingInCheck(isWhiteTurn))
            Debug.Log((isWhiteTurn ? "Fehér" : "Fekete") + " király sakkban!");

        if (IsCheckmate(isWhiteTurn))
            Debug.Log((isWhiteTurn ? "Fehér" : "Fekete") + " sakk-matt! Játék vége.");
    }

    
    bool IsKingInCheck(bool isWhite)
    {
        var kingData = board.gameState.FindKingData(isWhite);
        if (!kingData.HasValue) return false; 

        return MoveValidator.IsKingInCheck(kingData.Value, board.gameState);
    }

    // Matt
    public bool IsCheckmate(bool isWhite)
    {
        if (!IsKingInCheck(isWhite)) return false;
        return !board.HasAnyLegalMove(isWhite);
    }

    public bool IsStalemate(bool isWhite, GameState state)
    {
        
        PieceData? king = state.FindKingData(isWhite);
        if (king.HasValue && MoveValidator.IsKingInCheck(king.Value, state))
            return false;

        
        if (board.HasAnyLegalMove(isWhite))
            return false;

        
        return true;
    }

}
