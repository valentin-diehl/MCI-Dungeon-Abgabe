using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using SGCore;
using SGCore.SG;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public abstract class ChessPiece : MonoBehaviour
{
    protected Dictionary<Vector3, ChessPiece> Pieces;
    public Player Player;
    protected Player Opponent;
    protected bool PlayersTurn;
    protected int ChessPieceValue;
    protected Vector3 CurrentPosition;
    protected List<LogMove> History;
    protected const float Scaling = 0.1f;
    protected GameManager Gm;

    public bool hasMoved = false, offset;

    /* Interaction Glove*/

    private int _touchingFingers;
    private bool _located;
    private Vector3 _firstLocation;

    private void Start() {
    }

    protected virtual bool IsValidMove(Vector3 newPosition) {
        return true;
    }

    public int GetChessPieceValue() {
        return ChessPieceValue;
    }

    public void Init(Dictionary<Vector3, ChessPiece> pieces, Player player, Player opponent, bool playersTurn,
        List<LogMove> history, GameManager gm, Vector3 position) {
        _touchingFingers = 0;
        Gm = gm;
        Pieces = pieces;
        Player = player;
        Opponent = opponent;
        PlayersTurn = playersTurn;
        History = history;
        CurrentPosition = position;
    }

    protected void SwitchPlayersTurn() {
        /* true ist white und black ist false*/
        PlayersTurn = !PlayersTurn;
    }
/*
    private IEnumerable<Vector3> PossibleMoves() {
        var positions = new List<Vector3>();
        for (var i = 0; i < Scaling*8; i+= Scaling) {
            for (var j = 0; j < Scaling*8; j+= Scaling) {
                var tmpPos = new Vector3(i,Scaling, j);
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
        foreach (var piece in Pieces.Values)
        {
            // Beispielhafter Materialwert (kann je nach Schachstück variieren)
            materialValue += piece.ChessPieceValue;
        }

        // Kontrolle über das Zentrum (eine starke Stellung)
        // Beispielhaft: Bonus für Figuren, die das Zentrum kontrollieren
        foreach (var piece in Pieces.Values)
        {
            if (isInCenter(piece.currentX, piece.currentY))
            {
                centerControlBonus += piece.ChessPieceValue / 2; // Einheiten im Zentrum werden belohnt
            }
        }

        // Gesamtbewertung: Materialwert + Bonus für Kontrolle über das Zentrum
        int evaluation = materialValue + centerControlBonus;

        return evaluation;
    }

// Hilfsmethode, um zu überprüfen, ob eine Position im Zentrum liegt
    private bool isInCenter(int x, int y) {
        return x is >= 2 and <= 5 && y is >= 2 and <= 5;
    }


    private IEnumerable<KiIvm> CreateKiIvmList() {
        return (from p in Pieces.Values
            let fname = p.ChessPieceValue switch
            {
                1 => "Pawn",
                3 => p.name.StartsWith(p.Player.GetColor() + "Knight") ? "Knight" : "Bishop",
                5 => "Rook",
                9 => "Queen",
                999 => "King",
                _ => ""
            }
            select new KiIvm(fname, p.Player.GetColor().Equals("Black") ? "Black" : "White", p.currentX, p.currentY,p.hasMoved)).ToList();
    }
    
    
    
    
    //pos ist die currently position, maximizingPlayer steht fuer player 1 und 2
    public int minimax(Vector2 pos, int depth, int alpha, int beta, bool maximizingPlayer) {
        if (depth == 0) {
            // Führe hier die Bewertung der Position durch und gebe den Wert zurück
            return EvaluatePosition();
        }

        if (maximizingPlayer)
        {
            var maxEval = int.MinValue;
            foreach (var eval in PossibleMoves().Select(tmPosition => minimax(tmPosition, depth - 1, alpha, beta, false)))
            {
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha) break;
            }

            return maxEval;
        }
        else
        {
            var minEval = int.MaxValue;
            foreach (var eval in PossibleMoves().Select(tmPosition => minimax(tmPosition, depth - 1, alpha, beta, true)))
            {
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);
            }

            return minEval;
        }
    }
*/

    public virtual bool Move(Vector3 newPos) {
        Debug.Log("Move: " +name + ", "+ newPos );
        var newPosition = new Vector3(RoundMove(newPos.x / Scaling)*Scaling,Scaling, RoundMove(newPos.z / Scaling)*Scaling);

        if (Pieces.ContainsKey(newPosition) && Pieces[newPosition].Player == Player) return false;
        if (PlayersTurn && Player.GetColor() != "Black" || !IsValidMove(newPosition)) return false;
        LogMove lm;
        if (Pieces.ContainsKey(newPosition)) {
            lm = new LogMove(this, CurrentPosition, newPosition, true, Pieces[newPosition], null);
            Opponent.GetPiecesOfPlayer().Remove(Pieces[newPosition]);
            Pieces.Remove(newPosition);
        }
        else lm = new LogMove(this, CurrentPosition, newPosition, false, null, null);

        Pieces.Remove(CurrentPosition);
        CurrentPosition.x = newPosition.x;
        CurrentPosition.z = newPosition.z;

        Pieces.Add(CurrentPosition, this);
        transform.position = newPosition;
        History.Add(lm);

        hasMoved = true;
        SwitchPlayersTurn();
        return true;
    }

    protected static int RoundMove(float value) {
        switch (value) {
            case < 0:
                return 0;
            case > Scaling * 8:
                return (int)(value / Scaling);
            default:
            {
                return (int)Math.Round(value / Scaling);
            }
        }
    }


    protected bool VerticalMove(Vector3 vec) {
        float limit = 0, init = 0;
        if (Math.Abs(vec.x - CurrentPosition.x) > 0) return false;
        if (vec.z < CurrentPosition.z) {
            limit = CurrentPosition.z;
            init = vec.y + Scaling;
        }
        else if (vec.z > CurrentPosition.z) {
            init = CurrentPosition.z + Scaling;
            limit = vec.z;
        }

        for (var i = init; i <= limit; i+=Scaling) {
            var nVec = new Vector3(vec.x,Scaling, i);
            if (Math.Abs(i - limit) < 0 && Pieces[nVec].Player != Player) return true;
            if (Pieces[nVec] != null) return false;
        }

        return true;
    }

    protected bool HorizontalMove(Vector3 vec) {
        float limit = 0, init = 0;
        if (Math.Abs(vec.z - CurrentPosition.z) > 0) return false;
        if (vec.x < CurrentPosition.x) {
            limit = CurrentPosition.x;
            init = vec.x + Scaling;
        }
        else if (vec.x > CurrentPosition.x) {
            init = CurrentPosition.x + Scaling;
            limit = vec.x;
        }

        for (var i = init; i <= limit; i+=Scaling) {
            var nVec = new Vector3(i,Scaling, vec.z);
            if (Math.Abs(i - limit) < 0 && Pieces[nVec].Player != Player) return true;
            if (Pieces[nVec] != null) return false;
        }

        return true;
    }

    protected bool DiagonalMove(Vector3 vec) {
        if (Math.Abs((CurrentPosition.x - CurrentPosition.z) - (vec.x - vec.z)) > 0) return false;
        var limit = vec.x - vec.z;
        //left top
        if (CurrentPosition.x > vec.x && CurrentPosition.z > vec.z) {
            for (var i = Scaling; i < limit; i+=Scaling) {
                var nVec = new Vector3(vec.x + i,Scaling, vec.z + i);
                if (Pieces[nVec] != null) return false;
            }
        }
        //right bottom
        else if (CurrentPosition.x > vec.x && CurrentPosition.z < vec.z) {
            for (var i = Scaling; i < limit; i+=Scaling) {
                var nVec = new Vector3(vec.x + i, vec.z - 1);
                if (Pieces[nVec] != null) return false;
            }
        }
        //left bottom
        else if (CurrentPosition.x < vec.x && CurrentPosition.z < vec.z) {
            for (var i = Scaling; i < limit; i+=Scaling) {
                var nVec = new Vector3(vec.x - i,Scaling, vec.z - i);
                if (Pieces[nVec] != null) return false;
            }
        }
        //right top
        else if (CurrentPosition.x < vec.x && CurrentPosition.z > vec.z) {
            for (var i = Scaling; i < limit; i+=Scaling) {
                var nVec = new Vector3(vec.x - i,Scaling, vec.z + i);
                if (Pieces[nVec] != null) return false;
            }
        }

        return true;
    }


    private void SnapPieceBack() {
        print("snap back <<<<<<<<<<<<<< X: " + CurrentPosition.x + ", 'y': " + CurrentPosition.z);
        transform.position = CurrentPosition;
    }

    private void OnTriggerEnter(Collider other) {
        if (!(other.CompareTag("Thumb") || other.CompareTag("Index") || other.CompareTag("Middle"))) return;
        print("ich bin hier in der enter " + _touchingFingers);
        print(other.ToString());
        
        if (other.CompareTag("Thumb")|| other.CompareTag("Index")||other.CompareTag("Middle")){//_sg.SendHaptics();
            _touchingFingers++;
        }
        
        
    }

    private void OnTriggerStay(Collider other) {
        if (!(other.CompareTag("Thumb") || other.CompareTag("Index") || other.CompareTag("Middle"))) return;
        if (_touchingFingers < 2) return;
        if (!_located) {
            _located = true;
            _firstLocation = transform.position;
        }
        

        var localCollisionPoint = other.transform.InverseTransformPoint(transform.position);

        var worldCollisionPoint = other.transform.TransformPoint(localCollisionPoint);

        //worldCollisionPoint.y = 0.0f;

        transform.position = worldCollisionPoint;
    }


    private void OnTriggerExit(Collider other) {
        if (!(other.CompareTag("Thumb") || other.CompareTag("Index") || other.CompareTag("Middle"))) return;
        if (_touchingFingers > 0) _touchingFingers--;
        print("ich bin hier in der exit nac loslassen " + _touchingFingers);
        if (_touchingFingers >= 2) return;
        if (_firstLocation == transform.position) return;
        transform.eulerAngles = new Vector3(0, 0, 0);
        if (Move(transform.position)) return;
        SnapPieceBack();
        _located = false;
    }
    
    /* einen Zug wieder rückgängig machen */
    private void UndoMove() {
        var log = History[^1];

        if (log.GetSpecialMove() != null) {
            switch (log.GetSpecialMove()) {
                case "TransformationToQueen":
                    Gm.ChangeQueenToPawn(log.GetOldPos(),Player,Opponent, Gm);
                    UndoMove();
                    break;
                case "Rochade":
                    UndoMoveHelper(log);
                    UndoMove();
                    break;
            }     
        } else UndoMoveHelper(log);
        
    }

    private void UndoMoveHelper(LogMove log) {
        
        History.Remove(History[^1]);
        var m = false;
            
        foreach (var lm in History.Where(lm => lm.GetSelf() == this)) {
            m = true;
        }

        hasMoved = m;
            
        Pieces.Remove(log.GetNewPos());
            
        if (log.GetCapturedPiece() != null) {
            Pieces.Add(log.GetNewPos(), log.GetCapturedPiece());
            log.GetCapturedPiece().Player.GetPiecesOfPlayer().Add(log.GetCapturedPiece());
            // TODO: geschlagenes Piece an position bewegen
        }
            
        CurrentPosition.x = log.GetOldPos().x;
        CurrentPosition.z = log.GetOldPos().z;
            
        Pieces.Add(log.GetOldPos(), this);
        transform.position = CurrentPosition;
            
        SwitchPlayersTurn();
    }

}