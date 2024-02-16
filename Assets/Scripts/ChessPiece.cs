using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SGCore.SG;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public abstract class ChessPiece : MonoBehaviour {
    protected Dictionary<Vector2, ChessPiece> Pieces;
    public Player Player, Opponent;
    protected bool PlayersTurn;
    protected int x, y, ChessPieceValue;
    protected List<LogMove> History;
    protected GameManager gm;

    public bool hasMoved = false, offset;

    /* Interaction Glove*/

    private int _touchingFingers = 0;
    private bool _located = false;
    private Vector3 _firstLocation;
    private SenseGlove _sg;

    private void Start(){

    }
    protected virtual bool IsValidMove(Vector2 newPosition){
        return true;
    }

    public int GetChessPieceValue() {
        return ChessPieceValue;
    }

    public void Init(Dictionary<Vector2, ChessPiece> pieces, Player player, Player opponent, bool playersTurn, List<LogMove> history, GameManager gm, Vector2 position) {

        this.gm = gm;
        Pieces = pieces;
        Player = player;
        Opponent = opponent;
        PlayersTurn = playersTurn;
        History = history;
        x = (int)position.x;
        y = (int)position.y;
    }
    
    protected void SwitchPlayersTurn() {
        /* true ist white und black ist false*/
        PlayersTurn = !PlayersTurn;
    }

    private List<Vector2> PossibleMoves(){
        var positions = new List<Vector2>();
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


    public virtual bool Move(Vector2 newPos) {
        if (Pieces == null) {
            Debug.LogError("Dictionary _pieces or Pieces is not initialized.");
            return false;
        }
        var newPosition = new Vector2(roundMove(newPos.x) , roundMove(newPos.y));
        
        if (Pieces.ContainsKey(newPosition) && Pieces[newPosition].Player == Player) return false;
        if(PlayersTurn && Player.GetColor() != "Black" || !IsValidMove(newPosition)) return false;
        LogMove lm;
        var oldPos = new Vector2(x, y);
        if (Pieces.ContainsKey(newPosition)) {
            lm = new LogMove(this,oldPos, newPosition, true, Pieces[newPosition],null);
            Opponent.GetPiecesOfPlayer().Remove(Pieces[newPosition]);
            Pieces.Remove(newPosition);
        }
        else lm = new LogMove(this,oldPos, newPosition, false,null,null);
        
        Pieces.Remove(oldPos);
        x = (int)newPosition.x;
        y = (int)newPosition.y;

        Pieces.Add(new Vector2(x, y), this);
        transform.position = new Vector3(x,0,y) * Time.deltaTime; 
        History.Add(lm);
        
        hasMoved = true;
        SwitchPlayersTurn();
        return true;
    }
    
    protected int roundMove(float value){
        
        if(value < 0){
            return 0; 
        }else if(value > 7){
            return 7;
        }else if(value % 1 > 0.7){
            return (int)value + 1; 
        }else{
            return (int)value; 
        }
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
    
    
    private void SnapPieceBack() {
        transform.position = new Vector3(x, 0 ,y) * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other) {
        print("ich bin hier in der enter " + _touchingFingers);
        print(other.ToString());
        if(other.CompareTag("Glove")) _touchingFingers++;
    }

    private void OnTriggerStay(Collider other) {
    
        print("ich bin hier im stay " + _touchingFingers);
        if (_touchingFingers >= 2) {
            /* var loc = sg.GetGloveLocation();  Plan B */
            if (!_located) {
                _located = true;
                _firstLocation = transform.position;
            }
        
            /*/ Mausposition in Weltkoordinaten umwandeln*/
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    
            /*/ Z-Komponente der Mausposition auf 0 setzen, um in der Ebene zu bleiben -------- vorher wurde z auf 0 gesetzt*/
            mousePos.y = 0f;

            /*/ Objekt an die Mausposition setzen*/
            transform.position = mousePos;
        }
    }


    private void OnTriggerExit(Collider other) {
    
        if (_touchingFingers > 0) _touchingFingers--;
        print("ich bin hier in der exit nac loslassen " + _touchingFingers);
        if (_touchingFingers >= 2) return;
        if (_firstLocation == transform.position) return;
        float difX = _firstLocation.x - transform.position.x,
            difZ = _firstLocation.z - transform.position.z;

        if (!(Math.Abs(difX) > 0.6f) && !(Math.Abs(difZ) > 0.6f)) return;
        if (!Move(new Vector2(transform.position.x, transform.position.z))) {
            SnapPieceBack();
            _located = false;;
        }

    }


}
