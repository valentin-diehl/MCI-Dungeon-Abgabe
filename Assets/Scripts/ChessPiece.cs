using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public abstract class ChessPiece : MonoBehaviour {
    [SerializeField] protected Color _baseColor, _offsetColor;
    [SerializeField] protected MeshRenderer _renderer;
    protected Dictionary<Vector2, ChessPiece> Pieces;
    public Player Player;
    private bool _playersTurn;
    protected int x, y, ChessPieceValue;

    public bool hasMoved = false;

    void Start(){
        if (_renderer == null) {
            _renderer = GetComponent<MeshRenderer>();
        }
    }
    protected virtual bool IsValidMove(Vector2 newPosition){
        return true;
    }

    public int GetChessPieceValue() {
        return ChessPieceValue;
    }

    public void Init(bool isOffset, Dictionary<Vector2, ChessPiece> pieces, Player player, bool playersTurn) {
        if (_renderer != null) {
            _renderer.material.color = isOffset ? _offsetColor : _baseColor;
        }
        this.Pieces = pieces;
        this.Player = player;
        this._playersTurn = playersTurn;
    }

    private List<Vector2> PossibleMoves(){
        List<Vector2> positions = new List<Vector2>();
        Vector2 tmpPos;
        for (var i = 0; i < 8; i++){
            for (var j = 0; j < 8; j++){
                tmpPos = new Vector2(i, j);
                if (IsValidMove((tmpPos))) {
                    positions.Add(tmpPos);
                }
            }
        }
        return positions;
    }


    private int EvaluatePosition() {
        var materialValue = 0;
        var centerControlBonus = 0;

        // Materialbewertung
        foreach (var piece in Pieces.Values) {
            // Beispielhafter Materialwert (kann je nach Schachstück variieren)
            materialValue += piece.ChessPieceValue;
        }

        // Kontrolle über das Zentrum (eine starke Stellung)
        // Beispielhaft: Bonus für Figuren, die das Zentrum kontrollieren
        foreach (var piece in Pieces.Values) {
            if (isInCenter(piece.x, piece.y)) {
                centerControlBonus += piece.ChessPieceValue / 2; // Einheiten im Zentrum werden belohnt
            }
        }

        // Gesamtbewertung: Materialwert + Bonus für Kontrolle über das Zentrum
        int evaluation = materialValue + centerControlBonus;

        return evaluation;
    }

// Hilfsmethode, um zu überprüfen, ob eine Position im Zentrum liegt
    private bool isInCenter(int x, int y) {
        return (x >= 2 && x <= 5) && (y >= 2 && y <= 5);
    }


    //pos ist die currently position, maximizingPlayer steht fuer player 1 und 2
    public int minimax(Vector2 pos, int depth, int alpha, int beta, bool maximizingPlayer)
    {
        if (depth == 0) {
            // Führe hier die Bewertung der Position durch und gebe den Wert zurück
            return EvaluatePosition();
        }

        if (maximizingPlayer){
            int maxEval = int.MinValue;
            foreach (var tmPosition in PossibleMoves()){
                int eval = minimax(tmPosition, depth - 1, alpha, beta, false);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha) break;
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (var tmPosition in PossibleMoves()){
                int eval = minimax(tmPosition, depth - 1, alpha, beta, true);
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval); 
            }
            return minEval;
        }
    }


    public bool Move(Vector2 newPosition) {
        if(!_playersTurn && Player.GetColor() != "White" || !IsValidMove(newPosition)) return false;
        if(_playersTurn && Player.GetColor() != "Black" || !IsValidMove(newPosition)) return false;
        
        Pieces.Remove(new Vector2(x, y));
        x = (int)newPosition.x;
        y = (int)newPosition.y;

        Pieces.Add(new Vector2(x, y), this);
        transform.position = new Vector3(x,0,y) * Time.deltaTime; 

        hasMoved = true;
        return true;

    }


    protected bool VerticalMove(Vector2 vec) {
        float limit = 0, init = 0;
        if (vec.x != x) return false;
        else if (vec.y < y) {
            limit = y;
            init = vec.y + 1;
        }
        else if (vec.y > y) {
            init = y + 1;
            limit = vec.y;
        }
        for (var i = init; i <= limit; i++) {
            var nVec = new Vector2(vec.x, i);
            if (i == limit && Pieces[nVec].Player != Player) return true;
            if (Pieces[nVec] != null) return false;
        }
        return true;
    }

    protected bool HorizontalMove(Vector2 vec) {
        float limit = 0, init = 0;
        if (vec.y != y) return false;
        if (vec.x < x) {
            limit = x;
            init = vec.x + 1;
        }
        else if (vec.x > x)
        {
            init = x + 1;
            limit = vec.x;
        }
        for (var i = init; i <= limit; i++)
        {
            var nVec = new Vector2(i, vec.y);
            if (i == limit && Pieces[nVec].Player != Player) return true;
            if (Pieces[nVec] != null) return false;
        }
        return true;
    }

    protected bool DiagonalMove(Vector2 vec) {
        if ((x - y) != (vec.x - vec.y)) return false;
        var limit = vec.x - vec.y;
        //nach links oben
        if (x > vec.x && y > vec.y) {
            for (float i = 1; i < limit; i++) {
                var nVec = new Vector2(vec.x + i, vec.y + i);
                if (Pieces[nVec] != null) return false;
            }
        }
        //nach rechts oben
        else if (x > vec.x && y < vec.y) {
            for (float i = 1; i < limit; i++) {
                var nVec = new Vector2(vec.x + i, vec.y - 1);
                if (Pieces[nVec] != null) return false;
            }
        }
        //nach links unten
        else if (x < vec.x && y < vec.y) {
            for (float i = 1; i < limit; i++) {
                var nVec = new Vector2(vec.x - i, vec.y - i);
                if (Pieces[nVec] != null) return false;
            }
        }
        //nach rechts unten
        else if (x < vec.x && y > vec.y)
        {
            for (float i = 1; i < limit; i++)
            {
                var nVec = new Vector2(vec.x - i, vec.y + i);
                if (Pieces[nVec] != null) return false;
            }
        }
        return true;
    }

}
