using System;
using System.Collections.Generic;
using SGCore.SG;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour {
    
    [FormerlySerializedAs("_queenPrefab")] [SerializeField] private ChessPiece queenPrefab;
    [FormerlySerializedAs("_pawnPrefab")] [SerializeField] private ChessPiece pawnPrefab;
    
    private Player _playerWhite, _playerBlack;
    private bool _playersTurn = true;
    private bool _gameLoop;
    private Dictionary<Vector2, Tile> _tiles;
    private Dictionary<Vector3, ChessPiece> _pieces;
    private List<ChessPiece> _piecesBlk, _piecesWht;
    private ChessPiece _selectedPiece;
    private readonly List<LogMove> _history = new List<LogMove>();
    private int _queenCounter = 3;
    private SenseGlove _sg;
    private const float Scaling = 0.1f;


    private void Start() {
        _pieces = new Dictionary<Vector3, ChessPiece>();
        _piecesBlk = new List<ChessPiece>();
        _piecesWht = new List<ChessPiece>();
        _playerWhite = new Player("White", _piecesWht);
        _playerBlack = new Player("Black", _piecesBlk);
        FindAndAssignChessPieces();
        _playersTurn = true;

        foreach (var k in _pieces.Keys) {
            print("KEY: " + k + " , Name:" + _pieces[k].name);
        }

    }

    public bool GetPlayersTurn() {
        return _playersTurn;
    }
    
    public void SwitchPlayersTurn() {
        /* true ist white und black ist false*/
        //TODO: wieder wechseln
        //_playersTurn = !_playersTurn;
        _playersTurn = true;
    }

    private static float RoundMove(float value) {
        return value switch {
            < 0 => 0,
            > 7 => 7,
            _ => (float)Math.Round(value)
        };
    }
    
    private void FindAndAssignChessPieces() {
        var chessPieces = FindObjectsOfType<ChessPiece>();

        foreach (var chessPiece in chessPieces) {
            
            var position1 = chessPiece.transform.position;
            var xVal = RoundMove(position1.x / Scaling) * Scaling;
            var zVal = RoundMove(position1.z / Scaling) * Scaling;
            position1 = new Vector3(xVal, Scaling / 2, zVal);
            //Debug.Log("Init " + chessPiece.name + ", pos: " + position1);
            chessPiece.Init(chessPiece.CompareTag("White") ?_playerWhite : _playerBlack,chessPiece.CompareTag("White") ?_playerBlack : _playerWhite , _history, this, position1);
        
            _pieces[position1] = chessPiece;
        
            if (chessPiece.CompareTag("White")) {
                _playerWhite.GetPiecesOfPlayer().Add(chessPiece);
            }
            else if (chessPiece.CompareTag("Black")) {
                _playerBlack.GetPiecesOfPlayer().Add(chessPiece);
            }
        }
    }

    public Dictionary<Vector3, ChessPiece> GetPieces() {
        return _pieces;
    }

     public void ChangePawnToQueen(Vector3 pos, Player myPlayer, Player op, GameManager gm) {
        var piece = Instantiate(queenPrefab, pos, Quaternion.identity);
        piece.name = $"Queen {_queenCounter}";
        _queenCounter++;
        piece.Init(myPlayer,op, _history,gm, pos);
        myPlayer.GetPiecesOfPlayer().Add(piece);
        _pieces[pos] = piece;
    }
     
    public void ChangeQueenToPawn(Vector3 pos, Player myPlayer, Player op, GameManager gm) {
        var piece = Instantiate(pawnPrefab, pos, Quaternion.identity);
        _queenCounter--;
        piece.Init(myPlayer,op, _history,gm, pos);
        myPlayer.GetPiecesOfPlayer().Add(piece);
        _pieces[pos] = piece;
    } 

    private void GameLoop() {
        while (_gameLoop) {
            /* Did white or black lose?*/
            if (!_playerWhite.RemainsKing()) {
                SwitchScreen(false);
            }
            else if (!_playerBlack.RemainsKing()) {
                SwitchScreen(true);
            }
        }
        
    }

    private static void SwitchScreen(bool hasWon) {
        /* switch to losing screen || switch to winning screen*/
        SceneManager.LoadScene(hasWon ? "Win-Screen" : "Lose-Screen");
    }

   
    
    
    
}