using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public abstract class ChessPiece : MonoBehaviour {
    [SerializeField] protected Color _baseColor, _offsetColor;
    [SerializeField] protected MeshRenderer _renderer;
    protected Dictionary<Vector2, ChessPiece> _pieces;
    public Player player, playersTurn;
    protected int x, y, chessPieceValue;

    public bool hasMoved = false;

    void Start(){
        if (_renderer == null) {
            _renderer = GetComponent<MeshRenderer>();
        }
    }
    protected virtual bool IsValidMove(Vector2 newPosition){
        return true;
    }

    public void Init(bool isOffset, Dictionary<Vector2, ChessPiece> _pieces, Player player, Player playersTurn) {
        if (_renderer != null) {
            _renderer.material.color = isOffset ? _offsetColor : _baseColor;
        }
        this._pieces = _pieces;
        this.player = player;
        this.playersTurn = playersTurn;
    }
    
    public List<Vector2> possibleMoves(){
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
    
    
    public int evaluatePosition() {
        var materialValue = 0;
        var centerControlBonus = 0;

        // Materialbewertung
        foreach (var piece in _pieces.Values) {
            // Beispielhafter Materialwert (kann je nach Schachstück variieren)
            materialValue += piece.chessPieceValue;
        }

        // Kontrolle über das Zentrum (eine starke Stellung)
        // Beispielhaft: Bonus für Figuren, die das Zentrum kontrollieren
        foreach (var piece in _pieces.Values) {
            if (isInCenter(piece.x, piece.y)) {
                centerControlBonus += piece.chessPieceValue / 2; // Einheiten im Zentrum werden belohnt
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
            return evaluatePosition();
        }

        if (maximizingPlayer){
            int maxEval = int.MinValue;
            foreach (var tmPosition in possibleMoves()){
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
            foreach (var tmPosition in possibleMoves()){
                int eval = minimax(tmPosition, depth - 1, alpha, beta, true);
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval); 
            }
            return minEval;
        }
    }


    public bool Move(Vector2 newPosition) {
        Vector2 oldPos; 
        Vector2 newPos; 

        if(playersTurn != player || !IsValidMove(newPosition)) return false;
        
        oldPos = new Vector2(x, y); 
        _pieces.Remove(oldPos);
        x = (int)newPosition.x;
        y = (int)newPosition.y;

        newPos = new Vector2(x, y); 
        _pieces.Add(newPos, this);
        transform.position = new Vector3(x,0,y) * Time.deltaTime; 

        bool hasCapturedFigure = false; // muss noch angepasst werden
        ChessPiece capturedPiece = null // auch noch anpassen

        MovedObjekt movedObject = new MovedObjekt(oldPos, newPos, hasCapturedFigure, capturedPiece);

        GameManager.gameHistoryStack.add(movedObject); 

        hasMoved = true;
        return true;

    }


    public bool verticalMove(Vector2 vec) {
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
            if (i == limit && _pieces[nVec].player != player) return true;
            if (_pieces[nVec] != null) return false;
        }
        return true;
    }

    public bool horizontalMove(Vector2 vec) {
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
            if (i == limit && _pieces[nVec].player != player) return true;
            if (_pieces[nVec] != null) return false;
        }
        return true;
    }

    public bool diagonalMove(Vector2 vec) {
        if ((x - y) != (vec.x - vec.y)) return false;
        var limit = vec.x - vec.y;
        //nach links oben
        if (x > vec.x && y > vec.y) {
            for (float i = 1; i < limit; i++) {
                var nVec = new Vector2(vec.x + i, vec.y + i);
                if (_pieces[nVec] != null) return false;
            }
        }
        //nach rechts oben
        else if (x > vec.x && y < vec.y) {
            for (float i = 1; i < limit; i++) {
                var nVec = new Vector2(vec.x + i, vec.y - 1);
                if (_pieces[nVec] != null) return false;
            }
        }
        //nach links unten
        else if (x < vec.x && y < vec.y) {
            for (float i = 1; i < limit; i++) {
                var nVec = new Vector2(vec.x - i, vec.y - i);
                if (_pieces[nVec] != null) return false;
            }
        }
        //nach rechts unten
        else if (x < vec.x && y > vec.y)
        {
            for (float i = 1; i < limit; i++)
            {
                var nVec = new Vector2(vec.x - i, vec.y + i);
                if (_pieces[nVec] != null) return false;
            }
        }
        return true;
    }

}
