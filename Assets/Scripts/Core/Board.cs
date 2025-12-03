using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Board : MonoBehaviour
{

    public Transform chessBoard;       
    public float tileSize = 1f;        // 1 egység = 1 mező
    public GameObject chessPiecePrefab;
    public ChessManager chessManager;
    public GameState gameState;

    public bool whiteTurn = true;
    public Piece selectedPiece = null; //babu valasztas

    public Sprite whitePawnSprite;
    public Sprite blackPawnSprite;
    public Sprite whiteRookSprite;
    public Sprite blackRookSprite;
    public Sprite whiteKnightSprite;
    public Sprite blackKnightSprite;
    public Sprite whiteBishopSprite;
    public Sprite blackBishopSprite;
    public Sprite whiteQueenSprite;
    public Sprite blackQueenSprite;
    public Sprite whiteKingSprite;
    public Sprite blackKingSprite;


    // bábuk logikai pozícióinak nyilvántartása
    public Piece[,] squares = new Piece[8, 8];

    // világpozíció számítása
    public Vector3 GetWorldPosition(Vector2Int boardPos)
    {
        Vector3 bottomLeft = chessBoard.position - new Vector3(3.5f * tileSize, 3.5f * tileSize, 0);
        return bottomLeft + new Vector3(boardPos.x * tileSize, boardPos.y * tileSize, 0);
    }

    // babuk elhelyezése a logikai pozíciók szerint
    public void SetPieceAt(Vector2Int pos, Piece piece)
    {
        squares[pos.x, pos.y] = piece;
        if (piece != null)
        {
            piece.position = pos;

            
            Vector3 worldPos = GetWorldPosition(pos);
            worldPos.z = -2f;
            piece.transform.position = worldPos;

            piece.transform.parent = this.transform;
        }
    }

    public void SpawnPiece(Vector2Int pos, bool isWhite, Sprite sprite, string type)
    {
        GameObject obj = Instantiate(chessPiecePrefab);
        Piece piece = null;

        
        switch (type)
        {
            case "Pawn":
                piece = obj.AddComponent<Pawn>();
                break;
            case "Rook":
                piece = obj.AddComponent<Rook>();
                break;
            case "Knight":
                piece = obj.AddComponent<Knight>();
                break;
            case "Bishop":
                piece = obj.AddComponent<Bishop>();
                break;
            case "Queen":
                piece = obj.AddComponent<Queen>();
                break;
            case "King":
                piece = obj.AddComponent<King>();
                break;
            default:
                Debug.LogError("Unknown piece type: " + type);
                Destroy(obj);
                return;
                
        }

        piece.isWhite = isWhite;
        piece.type = type;
        piece.SetSprite(sprite);

        SetPieceAt(pos, piece);
    }

    public void SpawnAllPieces()
    {
        // Fehér gyalogok
        for (int x = 0; x < 8; x++)
            SpawnPiece(new Vector2Int(x, 1), true, whitePawnSprite, "Pawn");

        // Fekete gyalogok
        for (int x = 0; x < 8; x++)
            SpawnPiece(new Vector2Int(x, 6), false, blackPawnSprite, "Pawn");

        // Fehér bábúk
        SpawnPiece(new Vector2Int(0, 0), true, whiteRookSprite, "Rook");
        SpawnPiece(new Vector2Int(7, 0), true, whiteRookSprite, "Rook");
        SpawnPiece(new Vector2Int(1, 0), true, whiteKnightSprite, "Knight");
        SpawnPiece(new Vector2Int(6, 0), true, whiteKnightSprite, "Knight");
        SpawnPiece(new Vector2Int(2, 0), true, whiteBishopSprite, "Bishop");
        SpawnPiece(new Vector2Int(5, 0), true, whiteBishopSprite, "Bishop");
        SpawnPiece(new Vector2Int(3, 0), true, whiteQueenSprite, "Queen");
        SpawnPiece(new Vector2Int(4, 0), true, whiteKingSprite, "King");

        // Fekete bábúk
        SpawnPiece(new Vector2Int(0, 7), false, blackRookSprite, "Rook");
        SpawnPiece(new Vector2Int(7, 7), false, blackRookSprite, "Rook");
        SpawnPiece(new Vector2Int(1, 7), false, blackKnightSprite, "Knight");
        SpawnPiece(new Vector2Int(6, 7), false, blackKnightSprite, "Knight");
        SpawnPiece(new Vector2Int(2, 7), false, blackBishopSprite, "Bishop");
        SpawnPiece(new Vector2Int(5, 7), false, blackBishopSprite, "Bishop");
        SpawnPiece(new Vector2Int(3, 7), false, blackQueenSprite, "Queen");
        SpawnPiece(new Vector2Int(4, 7), false, blackKingSprite, "King");
    }


    public bool HasAnyLegalMove(bool isWhite)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece piece = squares[x, y];
                if (piece == null || piece.isWhite != isWhite) continue;

                Vector2Int from = new Vector2Int(x, y);
                
                for (int tx = 0; tx < 8; tx++)
                {
                    for (int ty = 0; ty < 8; ty++)
                    {
                        Vector2Int to = new Vector2Int(tx, ty);
                        if (MoveValidator.IsLegalMove(from, to, gameState))
                            return true;
                    }
                }
            }
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnAllPieces();
        gameState = CreateGameStateFromBoard();
        Debug.Log("GameState létrejött: " + (gameState != null));
        
        Vector2Int from = new Vector2Int(0, 1);
        Vector2Int to = new Vector2Int(0, 2);
        Debug.Log("IsLegalMove test: " + MoveValidator.IsLegalMove(from, to, gameState));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int boardPos = new Vector2Int(
                Mathf.FloorToInt(mouseWorldPos.x + 4),
                Mathf.FloorToInt(mouseWorldPos.y + 4)
            );

            // játék vége
            if (chessManager.IsCheckmate(whiteTurn))
            {
                Debug.Log((whiteTurn ? "Fehér" : "Fekete") + " sakk-matt! Játék vége.");
                return;
            }

            if (selectedPiece != null)
            {
                // csak a saját köröd bábuja léphet
                if (selectedPiece.isWhite == whiteTurn)
                {
                    bool moved = TryMovePiece(selectedPiece, boardPos);
                    if (moved)
                    {
                        // Körváltás
                        whiteTurn = !whiteTurn;

                        // sakk állapot ellenorzes
                        chessManager.CheckGameState();
                    }
                }
                selectedPiece = null;
            }
            else
            {
                // próbáljuk kiválasztani a bábut
                Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);
                if (hit != null)
                {
                    Piece piece = hit.GetComponent<Piece>();
                    if (piece != null && piece.isWhite == whiteTurn)
                    {
                        selectedPiece = piece;
                    }
                }
            }
        }

        if (chessManager.IsCheckmate(whiteTurn))
        {
            Debug.Log((whiteTurn ? "Fehér" : "Fekete") + " sakk-matt! Játék vége.");
            return;
        }

        if (chessManager.IsStalemate(whiteTurn, gameState))
        {
            Debug.Log("Patt! Döntetlen.");
            return;
        }

    }


    public bool TryMovePiece(Piece piece, Vector2Int newPos)
    {
        Vector2Int from = piece.position;

        
        if (!MoveValidator.IsLegalMove(from, newPos, gameState))
        {
            Debug.Log("Érvénytelen lépés: " + from + " → " + newPos);
            return false;
        }

        
        gameState.ApplyMove(from, newPos);

        
        ApplyGameStateToBoard(gameState);

        return true;
    }

    public void MovePiece(Piece piece, Vector2Int newPos)
    {
        // rokád ellenőrzése 
        if (piece is King king)
        {
            int deltaX = newPos.x - piece.position.x;
            if (Mathf.Abs(deltaX) == 2) // Rokád
            {
                if (deltaX > 0) // King-side
                {
                    Piece rook = squares[7, piece.position.y];
                    squares[7, piece.position.y] = null;
                    SetPieceAt(new Vector2Int(5, piece.position.y), rook);
                }
                else // Queen-side
                {
                    Piece rook = squares[0, piece.position.y];
                    squares[0, piece.position.y] = null;
                    SetPieceAt(new Vector2Int(3, piece.position.y), rook);
                }
            }
            king.hasMoved = true;
        }

       
        if (squares[newPos.x, newPos.y] != null)
            Destroy(squares[newPos.x, newPos.y].gameObject);

        
        squares[piece.position.x, piece.position.y] = null;

        
        SetPieceAt(newPos, piece);

        
        if (piece is Rook rookPiece)
            rookPiece.hasMoved = true;
    }

    //////
    public GameState CreateGameStateFromBoard()
    {
        GameState state = new GameState();

        
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Piece p = squares[x, y];
                if (p != null)
                {
                    // PieceData pd = new PieceData(true, p.isWhite, p.GetType().Name, x, y);
                    PieceData pd = new PieceData(true, p.isWhite, p.type, x, y)
                    {
                        hasMoved = p.hasMoved 
                    };
                    state.board[x, y] = pd;
                }
                else
                {
                    state.board[x, y] = new PieceData(false, false, null, x, y);
                }
            }
        }

        return state;
    }

    public void ApplyGameStateToBoard(GameState state)
    {
        
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                PieceData pd = state.board[x, y];
                Piece current = squares[x, y];

                if (!pd.exists)
                {
                    
                    if (current != null)
                    {
                        Destroy(current.gameObject);
                        squares[x, y] = null;
                    }
                }
                else
                {
                    
                    if (current == null)
                    {
                        
                        Sprite sprite = GetSpriteFor(pd.type, pd.isWhite);
                        SpawnPiece(new Vector2Int(x, y), pd.isWhite, sprite, pd.type);

                        
                        Piece spawned = squares[x, y];
                        if (spawned != null)
                            spawned.hasMoved = pd.hasMoved;
                    }
                    else
                    {
                        
                        current.isWhite = pd.isWhite;
                        current.type = pd.type;
                        current.position = new Vector2Int(x, y);
                        current.transform.position = GetWorldPosition(current.position) + new Vector3(0, 0, -2f);
                        current.SetSprite(GetSpriteFor(pd.type, pd.isWhite));

                        
                        current.hasMoved = pd.hasMoved;
                    }
                }
            }
        }
    }

    // visszaadja a megfelelő spriteot 
    public Sprite GetSpriteFor(string type, bool isWhite)
    {
        switch (type)
        {
            case "Pawn": return isWhite ? whitePawnSprite : blackPawnSprite;
            case "Rook": return isWhite ? whiteRookSprite : blackRookSprite;
            case "Knight": return isWhite ? whiteKnightSprite : blackKnightSprite;
            case "Bishop": return isWhite ? whiteBishopSprite : blackBishopSprite;
            case "Queen": return isWhite ? whiteQueenSprite : blackQueenSprite;
            case "King": return isWhite ? whiteKingSprite : blackKingSprite;
            default: return null;
        }
    }


}
