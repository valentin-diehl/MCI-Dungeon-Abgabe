using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private const int FieldSize = 8;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _cam;
    [SerializeField] private ChessPiece _rookPrefab;
    [SerializeField] private ChessPiece _bishopPrefab;
    [SerializeField] private ChessPiece _kingPrefab;
    [SerializeField] private ChessPiece _queenPrefab;
    [SerializeField] private ChessPiece _pawnPrefab;
    [SerializeField] private ChessPiece _knightPrefab;

    /* Jedes einzele Piece */
    [SerializeField] private GameObject  whiteRook1;
    [SerializeField] private GameObject  whiteRook2;
    [SerializeField] private GameObject  whiteKnight1;
    [SerializeField] private GameObject  whiteKnight2;
    [SerializeField] private GameObject  whiteBishop1;
    [SerializeField] private GameObject  whiteBishop2;
    [SerializeField] private GameObject  whiteQueen;
    [SerializeField] private GameObject  whiteKing;
    [SerializeField] private GameObject  whitePawn1;
    [SerializeField] private GameObject  whitePawn2;
    [SerializeField] private GameObject  whitePawn3;
    [SerializeField] private GameObject  whitePawn4;
    [SerializeField] private GameObject  whitePawn5;
    [SerializeField] private GameObject  whitePawn6;
    [SerializeField] private GameObject  whitePawn7;
    [SerializeField] private GameObject  whitePawn8;

    [SerializeField] private GameObject  blackRook1;
    [SerializeField] private GameObject  blackRook2;
    [SerializeField] private GameObject  blackKnight1;
    [SerializeField] private GameObject  blackKnight2;
    [SerializeField] private GameObject  blackBishop1;
    [SerializeField] private GameObject  blackBishop2;
    [SerializeField] private GameObject  blackQueen;
    [SerializeField] private GameObject  blackKing;
    [SerializeField] private GameObject  blackPawn1;
    [SerializeField] private GameObject  blackPawn2;
    [SerializeField] private GameObject  blackPawn3;
    [SerializeField] private GameObject  blackPawn4;
    [SerializeField] private GameObject  blackPawn5;
    [SerializeField] private GameObject  blackPawn6;
    [SerializeField] private GameObject  blackPawn7;
    [SerializeField] private GameObject  blackPawn8;

    private Player _playerWhite, _playerBlack;
    private bool _playersTurn = true, _gameloop = true;
    private Dictionary<Vector2, Tile> _tiles;
    private Dictionary<Vector2, ChessPiece> _pieces;
    private List<ChessPiece> _piecesBlk, _piecesWht;
    private ChessPiece _selectedPiece;
    private List<LogMove> history = new List<LogMove>();
    private int QueenCounter = 3;


    private void Start()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        _pieces = new Dictionary<Vector2, ChessPiece>();
        _piecesBlk = new List<ChessPiece>();
        _piecesWht = new List<ChessPiece>();
        _playerWhite = new Player("White", _piecesWht);
        _playerBlack = new Player("Black", _piecesBlk);

        //GenerateGrid();
        assignPieces();

    }

    private List<ChessPiece> createPieceList() {
        List<ChessPiece> chessPieceList = new List<ChessPiece>();
        var fields = typeof(GameManager).GetFields(System.Reflection.BindingFlags.Instance |
                                                   System.Reflection.BindingFlags.NonPublic |
                                                   System.Reflection.BindingFlags.Public);

        foreach (var field in fields)
        {
            if (field.FieldType == typeof(ChessPiece) && (field.Name.StartsWith("white") || field.Name.StartsWith("black"))) {
                ChessPiece piece = field.GetValue(this) as ChessPiece;
                if (piece != null) {
                    chessPieceList.Add(piece);
                }
            }
        }

        return chessPieceList;
    }
    
    private void assignPieces() {

        foreach (var p in createPieceList()) {
            var piece = Instantiate(p, p.transform.position, Quaternion.identity);
            piece.name = $"{p.name}";
            piece.Init(_pieces, p.CompareTag("white") ? _playerWhite : _playerBlack, p.CompareTag("white") ? _playerBlack : _playerWhite,_playersTurn, history,this);
            _pieces[new Vector2(p.transform.position.x, p.transform.position.z)] = piece;
            if(p.CompareTag("white")) _playerWhite.GetPiecesOfPlayer().Add(piece);
            else _playerBlack.GetPiecesOfPlayer().Add(piece);
        }
    }
    public void ChangePawnToQueen(Vector2 pos, Player myPlayer, Player op, bool off,GameManager gm) {
        var piece = Instantiate(_queenPrefab, pos, Quaternion.identity);
        piece.name = $"Queen {QueenCounter}";
        QueenCounter++;
        piece.Init(_pieces, myPlayer,op,_playersTurn, history,gm);
        myPlayer.GetPiecesOfPlayer().Add(piece);
        _pieces[pos] = piece;
    }
    private void GenerateGrid()
    {
        for (var x = 0; x < FieldSize; x++)
        {
            for (var y = 0; y < FieldSize; y++)
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                var isEvenRow = y % 2 == 0;
                var isEvenColumn = x % 2 == 0;
                var isOffset = isEvenRow ^ isEvenColumn;

                spawnedTile.Init(isOffset);
                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }
    }

    /*
private void GeneratePieces()
{
    for (var x = 0; x < FieldSize; x++)
    {
        for (var y = 0; y < FieldSize; y++)
        {
            var erg = (x == 0);
            switch (x)
            {
                case 0:
                case 7:
                    switch (y)
                    {
                        case 0:
                        case 7:
                            SpawnPiece(_rookPrefab, x, y, !erg);
                            break;
                        case 1:
                        case 6:
                            SpawnPiece(_knightPrefab, x, y, !erg);
                            break;
                        case 2:
                        case 5:
                            SpawnPiece(_bishopPrefab, x, y, !erg);
                            break;
                        case 3:
                            SpawnPiece(erg ? _queenPrefab : _kingPrefab, x, y, !erg);
                            break;
                        case 4:
                            SpawnPiece(erg ? _kingPrefab : _queenPrefab, x, y, !erg);
                            break;
                    }
                    break;
                case 1:
                case 6:
                    var erg2 = (x == 1);
                    SpawnPiece(_pawnPrefab, x, y, !erg2);
                    break;
            }
        }
    }
}


private void SpawnPiece(ChessPiece prefab, int x, int y, bool isOffset) {
    var piece = Instantiate(prefab, new Vector3(x, 0.5f, y), Quaternion.identity);

    // Ändere die Rotation des Prefabs basierend auf der Ausrichtung
    if (x is 0 or 7 && y is 1 or 6) {
        var rotation = 270f;
        if (x == 0) rotation = 90f;
        piece.transform.Rotate(Vector3.up, rotation);
    }

    piece.name = $"{prefab.name} {x} {y}";
    piece.Init(_pieces, x < 4 ? _playerWhite : _playerBlack, x < 4 ? _playerBlack : _playerWhite,_playersTurn, history,this);
    _pieces[new Vector2(x, y)] = piece;
    if(x < 4) _playerWhite.GetPiecesOfPlayer().Add(piece);
    else _playerBlack.GetPiecesOfPlayer().Add(piece);
}
*/

    public Tile GetTileAtPosition(Vector2 pos) {
        return _tiles.GetValueOrDefault(pos);
    } 

    private void GameLoop() {

        while (_gameloop) {
            /* Did white or black lose?*/
            if (!_playerWhite.RemainsKing()) {
                switchScreen(false);
            }
            else if (!_playerBlack.RemainsKing()) {
                switchScreen(true);
            }
            else {
                /* Do the game stuff*/
                
            }
        }
        
    }

    private void switchScreen(bool hasWon){
        if (hasWon) {
            /* switch to winning screen*/
            SceneManager.LoadScene("Win-Screen");
        }
        else {
            /* switch to losing screen*/
            SceneManager.LoadScene("Lose-Screen");
        }
    }

   
    
    
    
}