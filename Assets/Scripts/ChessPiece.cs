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
    protected Dictionary<Vector2, ChessPiece> Pieces;
    public Player Player;
    protected Player Opponent;
    protected bool PlayersTurn;
    protected int currentX, currentY, ChessPieceValue;
    protected List<LogMove> History;
    protected GameManager Gm;
    protected const float Scaling = 0.08f;

    public bool hasMoved = false, offset;

    /* Interaction Glove*/

    private int _touchingFingers;
    private bool _located = false;
    private Vector3 _firstLocation;
    private SenseGlove _sg;

    private void Start() {
    }

    protected virtual bool IsValidMove(Vector2 newPosition) {
        return true;
    }

    public int GetChessPieceValue() {
        return ChessPieceValue;
    }

    public void Init(Dictionary<Vector2, ChessPiece> pieces, Player player, Player opponent, bool playersTurn,
        List<LogMove> history, GameManager gm, Vector2 position, SenseGlove sg) {
        _touchingFingers = 0;
        Gm = gm;
        Pieces = pieces;
        Player = player;
        Opponent = opponent;
        PlayersTurn = playersTurn;
        History = history;
        currentX = (int)(transform.position.x/Scaling);
        currentY = (int)(transform.position.z/Scaling);
        print("Init "+currentY+" "+currentX+" "+name);
        _sg = sg; // Beispiel: Wenn das SenseGlove-Objekt diesem Skript angefügt ist.
    }

    protected void SwitchPlayersTurn() {
        /* true ist white und black ist false*/
        PlayersTurn = !PlayersTurn;
    }

    private IEnumerable<Vector2> PossibleMoves() {
        var positions = new List<Vector2>();
        for (var i = 0; i < 8; i++) {
            for (var j = 0; j < 8; j++) {
                var tmpPos = new Vector2(i, j);
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


    public virtual bool Move(Vector2 newPos) {
        Debug.Log("Move: " +name + ", "+ newPos );
        var newPosition = new Vector2(RoundMove(newPos.x / Scaling), RoundMove(newPos.y / Scaling));

        if (Pieces.ContainsKey(newPosition) && Pieces[newPosition].Player == Player) return false;
        if (PlayersTurn && Player.GetColor() != "Black" || !IsValidMove(newPosition)) return false;
        LogMove lm;
        var oldPos = new Vector2(currentX, currentY);
        if (Pieces.ContainsKey(newPosition)) {
            lm = new LogMove(this, oldPos, newPosition, true, Pieces[newPosition], null);
            Opponent.GetPiecesOfPlayer().Remove(Pieces[newPosition]);
            Pieces.Remove(newPosition);
        }
        else lm = new LogMove(this, oldPos, newPosition, false, null, null);

        Pieces.Remove(oldPos);
        currentX = (int)newPosition.x;
        currentY = (int)newPosition.y;

        Pieces.Add(new Vector2(currentX, currentY), this);
        transform.position = new Vector3(currentX* Scaling, 0.05f, currentY * Scaling) * Time.deltaTime;
        History.Add(lm);

        hasMoved = true;
        SwitchPlayersTurn();
        return true;
    }

    protected static int RoundMove(float value) {
        switch (value)
        {
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


    protected bool VerticalMove(Vector2 vec) {
        float limit = 0, init = 0;
        if (Math.Abs(vec.x - currentX) > 0) return false;
        else if (vec.y < currentY) {
            limit = currentY;
            init = vec.y + 1;
        }
        else if (vec.y > currentY) {
            init = currentY + 1;
            limit = vec.y;
        }

        for (var i = init; i <= limit; i++) {
            var nVec = new Vector2(vec.x, i);
            if (Math.Abs(i - limit) < 0 && Pieces[nVec].Player != Player) return true;
            if (Pieces[nVec] != null) return false;
        }

        return true;
    }

    protected bool HorizontalMove(Vector2 vec) {
        float limit = 0, init = 0;
        if (Math.Abs(vec.y - currentY) > 0) return false;
        if (vec.x < currentX) {
            limit = currentX;
            init = vec.x + 1;
        }
        else if (vec.x > currentX) {
            init = currentX + 1;
            limit = vec.x;
        }

        for (var i = init; i <= limit; i++) {
            var nVec = new Vector2(i, vec.y);
            if (Math.Abs(i - limit) < 0 && Pieces[nVec].Player != Player) return true;
            if (Pieces[nVec] != null) return false;
        }

        return true;
    }

    protected bool DiagonalMove(Vector2 vec) {
        if (Math.Abs((currentX - currentY) - (vec.x - vec.y)) > 0) return false;
        var limit = vec.x - vec.y;
        //left top
        if (currentX > vec.x && currentY > vec.y) {
            for (float i = 1; i < limit; i++)
            {
                var nVec = new Vector2(vec.x + i, vec.y + i);
                if (Pieces[nVec] != null) return false;
            }
        }
        //right bottom
        else if (currentX > vec.x && currentY < vec.y) {
            for (float i = 1; i < limit; i++)
            {
                var nVec = new Vector2(vec.x + i, vec.y - 1);
                if (Pieces[nVec] != null) return false;
            }
        }
        //left bottom
        else if (currentX < vec.x && currentY < vec.y) {
            for (float i = 1; i < limit; i++)
            {
                var nVec = new Vector2(vec.x - i, vec.y - i);
                if (Pieces[nVec] != null) return false;
            }
        }
        //right top
        else if (currentX < vec.x && currentY > vec.y) {
            for (float i = 1; i < limit; i++)
            {
                var nVec = new Vector2(vec.x - i, vec.y + i);
                if (Pieces[nVec] != null) return false;
            }
        }

        return true;
    }


    private void SnapPieceBack() {
        print("snap back <<<<<<<<<<<<<< X: " + currentX + ", y: " + currentY);
        transform.position = new Vector3(currentX*Scaling, Scaling, currentY*Scaling) * Time.deltaTime;
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
        if (Move(new Vector2(transform.position.x, transform.position.z))) return;
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
            
        currentX = (int)log.GetOldPos().x;
        currentY = (int)log.GetOldPos().y;
            
        Pieces.Add(log.GetOldPos(), this);
        transform.position = new Vector3(currentX,0.05f,currentY) * Time.deltaTime;
            
        SwitchPlayersTurn();
    }

}