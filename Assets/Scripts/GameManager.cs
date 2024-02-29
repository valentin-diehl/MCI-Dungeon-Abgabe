using System;
using System.Collections.Generic;
using System.Linq;
using SGCore.SG;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;

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
    private List<ChessPiece> _capturedPieces;
    private readonly List<LogMove> _history = new List<LogMove>();
    private int _queenCounter = 3;
    private SenseGlove _sg;
    private const float Scaling = 0.1f;
    private int durchlauf = 1;
    
    // Referenz auf die AudioSource-Komponente
    public AudioSource moveSound;

    // Methode, die aufgerufen wird, um den Sound abzuspielen
    public void PlayMoveSound() {
        moveSound.Play();
    }

    public void AddToHistory(LogMove l) {
        _history.Add(l);
    }

    public List<LogMove> GetHistory() {
        return _history;
    }
    


    private void Start() {
        _pieces = new Dictionary<Vector3, ChessPiece>();
        _piecesBlk = new List<ChessPiece>();
        _piecesWht = new List<ChessPiece>();
        _capturedPieces = new List<ChessPiece>();
        _playerWhite = new Player(PlayerColor.White, _piecesWht);
        _playerBlack = new Player(PlayerColor.Black, _piecesBlk);
        FindAndAssignChessPieces();
        _playersTurn = true;
        // Stellen Sie sicher, dass moveSound eine Referenz auf die AudioSource-Komponente hat
        if (moveSound == null) {
            moveSound = GetComponent<AudioSource>();
        }
    }

    public bool GetPlayersTurn() {
        return _playersTurn;
    }

    public List<ChessPiece> GetCapturedPieces() {
        return _capturedPieces;
    }
    
    public void SwitchPlayersTurn() {
        /* true ist white und black ist false*/
        //TODO: wieder wechseln
       // _playersTurn = !_playersTurn;
        //if (_playersTurn == false) // GoForNextMove();
          //  Machmal();
        _playersTurn = true;
    }

    private void Machmal() {
        if (durchlauf == 1) {
            _pieces[new Vector3(Scaling * 6, Scaling / 2, Scaling * 6)]
                .Move(new Vector3(Scaling * 4, Scaling / 2, Scaling * 6));
        }
        else if (durchlauf == 2) {
            _pieces[new Vector3(Scaling * 7, Scaling / 2, Scaling * 6)]
                .Move(new Vector3(Scaling * 5, Scaling / 2, Scaling * 5));
        }
        else if (durchlauf == 3) {
            _pieces[new Vector3(Scaling * 6, Scaling / 2, Scaling * 0)]
                .Move(new Vector3(Scaling * 5, Scaling / 2, Scaling * 0));
        }else if (durchlauf == 4) {
            _pieces[new Vector3(Scaling * 4, Scaling / 2, Scaling * 6)]
                .Move(new Vector3(Scaling * 3, Scaling / 2, Scaling * 6));
        }else if (durchlauf == 5) {
            _pieces[new Vector3(Scaling * 7, Scaling / 2, Scaling * 7)]
                .Move(new Vector3(Scaling * 7, Scaling / 2, Scaling * 6));
        }

        durchlauf++;
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
            chessPiece.Init(chessPiece.CompareTag("White") ?_playerWhite : _playerBlack,chessPiece.CompareTag("White") ?_playerBlack : _playerWhite , this, position1);
        
            _pieces[position1] = chessPiece;
        
            if (chessPiece.GetOwnPlayer().GetColor() == PlayerColor.White) {
                _playerWhite.GetPiecesOfPlayer().Add(chessPiece);
            }
            else if (chessPiece.GetOwnPlayer().GetColor() == PlayerColor.Black) {
                _playerBlack.GetPiecesOfPlayer().Add(chessPiece);
            }
        }
    }

    public Dictionary<Vector3, ChessPiece> GetPieces() {
        return _pieces;
    }

     public void ChangePawnToQueen(Vector3 pos, Player myPlayer, Player op, GameManager gm) {
        var piece = Instantiate(queenPrefab, pos, Quaternion.identity);
        var transform1 = piece.transform;
        transform1.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        transform1.position = pos;
        piece.name = $"Queen {_queenCounter}";
        _queenCounter++;
        piece.Init(myPlayer,op, gm, pos);
        myPlayer.GetPiecesOfPlayer().Add(piece);
        _pieces[pos] = piece;
    }
     
    public void ChangeQueenToPawn(Vector3 pos, Player myPlayer, Player op, GameManager gm) {
        var piece = Instantiate(pawnPrefab, pos, Quaternion.identity);
        var transform1 = piece.transform;
        transform1.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        transform1.position = pos;
        _queenCounter--;
        piece.Init(myPlayer,op,gm, pos);
        myPlayer.GetPiecesOfPlayer().Add(piece);
        _pieces[pos] = piece;
    } 

    public void GameLoop() {
        /* Did white or black lose?*/
        if (!_playerWhite.RemainsKing()) {
            SwitchScreen(false);
        }
        else if (!_playerBlack.RemainsKing()) {
            SwitchScreen(true);
        }
    }

    private static void SwitchScreen(bool hasWon) {
        /* switch to losing screen || switch to winning screen*/
        SceneManager.LoadScene(hasWon ? "Win-Screen" : "Lose-Screen");
    }

    private void GoForNextMove() {
        var moves = new Dictionary<ChessPiece, IEnumerable<Vector3>>();
        foreach (var piece in _playerBlack.GetPiecesOfPlayer()) moves.Add(piece, piece.PossibleMoves());

        var valueMoves = new List<NextMove>();
        var randomMoves = new List<NextMove>();
        foreach (var key in moves.Keys) {
            foreach(var positions in moves[key]){
                if (_pieces.ContainsKey(positions)) 
                    valueMoves.Add(new NextMove(positions,_pieces[positions].GetChessPieceValue(), key));
                
                else randomMoves.Add(new NextMove(positions,0,key));
            }
        }

        if (valueMoves.Count > 0) {
            var maxVal = valueMoves[0];
            foreach (var nm in valueMoves.Where(nm => nm.GetCapturedValue() > maxVal.GetCapturedValue())) {
                maxVal = nm;
            }
            maxVal.GetMyPiece().Move(maxVal.TargetGetPosition());
            
        }
        else {
            var random = new Random();
            if (randomMoves.Count <= 0) return;
            var randomIndex = random.Next(randomMoves.Count);
            var randomMove = randomMoves[randomIndex];
            randomMove.GetMyPiece().Move(randomMove.TargetGetPosition());
        }
    }
    
}