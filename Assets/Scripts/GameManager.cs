using System.Collections.Generic;
using System.Linq;
using SGCore.SG;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour {
    
    [FormerlySerializedAs("_queenPrefab")] [SerializeField] private ChessPiece queenPrefab;
    [FormerlySerializedAs("_pawnPrefab")] [SerializeField] private ChessPiece pawnPrefab;
    
    private Player _playerWhite, _playerBlack;
    private bool _playersTurn = true;
    private bool _gameloop;
    private Dictionary<Vector2, Tile> _tiles;
    private Dictionary<Vector3, ChessPiece> _pieces;
    private List<ChessPiece> _piecesBlk, _piecesWht;
    private ChessPiece _selectedPiece;
    private List<LogMove> _history = new List<LogMove>();
    private int _queenCounter = 3;
    private SenseGlove _sg;
    protected const float Scaling = 0.1f;


    private void Start() {
        _pieces = new Dictionary<Vector3, ChessPiece>();
        _piecesBlk = new List<ChessPiece>();
        _piecesWht = new List<ChessPiece>();
        _playerWhite = new Player("White", _piecesWht);
        _playerBlack = new Player("Black", _piecesBlk);
        FindAndAssignChessPieces();
        _playersTurn = true;

    }

    public bool GetPlayersTurn() {
        return _playersTurn;
    }
    
    public void SwitchPlayersTurn() {
        /* true ist white und black ist false*/
        _playersTurn = !_playersTurn;
    }
    
    private void FindAndAssignChessPieces() {
        var chessPieces = FindObjectsOfType<ChessPiece>();

        foreach (var chessPiece in chessPieces) {
            var position1 = chessPiece.transform.position;
           // Debug.Log("Init " + chessPiece.name + ", pos: " + position1);
           //TODO muss angepasst werden wenn schwarze figuren hunzugefuegt werden
            chessPiece.Init(_pieces, _playerWhite, _playerBlack, _history, this, position1);
        
            _pieces[position1] = chessPiece;
        
            if (chessPiece.CompareTag("White")) {
                _playerWhite.GetPiecesOfPlayer().Add(chessPiece);
            }
            else if (chessPiece.CompareTag("Black")) {
                _playerBlack.GetPiecesOfPlayer().Add(chessPiece);
            }
        }
    }

    private void FindAndAssignGloves()
    {
        var sgR = GameObject.FindWithTag("RightGlove");
    }

    private List<ChessPiece> CreatePieceList() {
        var fields = typeof(GameManager).GetFields(System.Reflection.BindingFlags.Instance |
                                                   System.Reflection.BindingFlags.NonPublic |
                                                   System.Reflection.BindingFlags.Public);

        return (from field in fields where field.FieldType == typeof(GameObject) && (field.Name.StartsWith("white") || field.Name.StartsWith("black")) select field.GetValue(this)).OfType<ChessPiece>().ToList();
    }
    
    private void AssignPieces() {

        foreach (var p in CreatePieceList()) {
            var position = p.transform.position;
            var piece = Instantiate(p, position, Quaternion.identity);
            piece.name = $"{p.name}";
            print("here " + piece.name + position);
            _pieces.Add(piece.transform.position, piece);
            piece.Init(_pieces, p.CompareTag("White") ? _playerWhite : _playerBlack, p.CompareTag("White") ? _playerBlack : _playerWhite, _history,this, piece.transform.position);
            if(p.CompareTag("White")) _playerWhite.GetPiecesOfPlayer().Add(piece);
            else _playerBlack.GetPiecesOfPlayer().Add(piece);
        }
    }
     public void ChangePawnToQueen(Vector3 pos, Player myPlayer, Player op, GameManager gm) {
        var piece = Instantiate(queenPrefab, pos, Quaternion.identity);
        piece.name = $"Queen {_queenCounter}";
        _queenCounter++;
        piece.Init(_pieces, myPlayer,op, _history,gm, pos);
        myPlayer.GetPiecesOfPlayer().Add(piece);
        _pieces[pos] = piece;
    }
     
    public void ChangeQueenToPawn(Vector3 pos, Player myPlayer, Player op, GameManager gm) {
        var piece = Instantiate(pawnPrefab, pos, Quaternion.identity);
        _queenCounter--;
        piece.Init(_pieces, myPlayer,op, _history,gm, pos);
        myPlayer.GetPiecesOfPlayer().Add(piece);
        _pieces[pos] = piece;
    }
    
    /*
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
                SwitchScreen(false);
            }
            else if (!_playerBlack.RemainsKing()) {
                SwitchScreen(true);
            }
            else {
                /* Do the game stuff*/
                
            }
        }
        
    }

    private static void SwitchScreen(bool hasWon) {
        /* switch to losing screen*/
        /* switch to winning screen*/
        SceneManager.LoadScene(hasWon ? "Win-Screen" : "Lose-Screen");
    }

   
    
    
    
}