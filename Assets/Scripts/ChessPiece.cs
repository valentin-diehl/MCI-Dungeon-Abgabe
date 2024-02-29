using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class ChessPiece : MonoBehaviour {
    private Player _player;
    private Player _opponent;
    protected int ChessPieceValue;
    protected Vector3 CurrentPosition;
    protected const float Scaling = 0.1f;
    protected GameManager Gm;

    public bool hasMoved;

    /* Interaction Glove*/

    private int _touchingFingers;
    private bool _located;

    protected abstract bool IsValidMove(Vector3 newPos);

    public int GetChessPieceValue() {
        return ChessPieceValue;
    }
    
    public void Init(Player player, Player opponent, GameManager gm, Vector3 position) {
        _touchingFingers = 0;
        Gm = gm;
        _player = player;
        _opponent = opponent;
        CurrentPosition = position;
        hasMoved = false;
    }

    public Player GetOwnPlayer() {
        return _player;
    }
    protected Player GetOpponentPlayer() {
        return _opponent;
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

*/

    public virtual bool Move(Vector3 newPos) {
        var xVal = RoundMove(newPos.x / Scaling) * Scaling;
        var zVal = RoundMove(newPos.z / Scaling) * Scaling;
        
        var newPosition = new Vector3(xVal,Scaling/2, zVal);
        if (!IsValidMove(newPosition)) return false;
        LogMove lm;
        if (Gm.GetPieces().ContainsKey(newPosition)) {
            CapturePiece(newPosition);
            lm = new LogMove(this, CurrentPosition, newPosition,Gm.GetPieces()[newPosition], null);
        }
        else lm = new LogMove(this, CurrentPosition, newPosition,null, null);

        Gm.GetPieces().Remove(CurrentPosition);
        CurrentPosition = newPosition;

        Gm.GetPieces()[CurrentPosition] = this;
        transform.position = CurrentPosition;
        Gm.AddToHistory(lm);

        hasMoved = true;
        Gm.SwitchPlayersTurn();
        RefreshChessPieces();
        return true;
    }

    protected static float RoundMove(float value) {
        return value switch {
            < 0 => 0,
            > 7 => 7,
            _ => (float)Math.Round(value)
        };
    }

    public virtual IEnumerable<Vector3> PossibleMoves() {
        var erg = new List<Vector3>();
        for (var i = 0; i < 8; i++) {
           for(var j = 0; j < 8; j++) {
               var pos = new Vector3(Scaling * i, Scaling / 2, Scaling * j);
               if(IsValidMove(pos)) erg.Add(pos);
           } 
        }
        return erg;
    }

    protected bool HorizontalMove(Vector3 targetPosition) {
        if (Math.Abs(CurrentPosition.x - targetPosition.x) > 0) return false; // Sicherstellen, dass die Bewegung auf der gleichen X-Achse stattfindet
        //hinzugefügt
        if (Gm.GetPieces().ContainsKey(targetPosition) && Gm.GetPieces()[targetPosition].GetOwnPlayer() == GetOwnPlayer()) return false;
        
        var direction = targetPosition.z > CurrentPosition.z ? 1 : -1;
        var steps = Mathf.Abs(targetPosition.z - CurrentPosition.z) / Scaling;

        for (var i = 1; i < steps; i++) {
            var nextStep = new Vector3(CurrentPosition.x, CurrentPosition.y, CurrentPosition.z + i * Scaling * direction);
            if (Gm.GetPieces().ContainsKey(nextStep)) return false; // Eine Figur blockiert den Weg
        }

        return true; // Der Weg ist frei und der Zug ist gültig
    }

    

    protected bool VerticalMove(Vector3 targetPosition) {
        if (Mathf.Abs(CurrentPosition.z - targetPosition.z) > 0) return false; 
        if (Gm.GetPieces().ContainsKey(targetPosition) && Gm.GetPieces()[targetPosition].GetOwnPlayer() == GetOwnPlayer()) return false;

        var direction = targetPosition.x > CurrentPosition.x ? 1 : -1;

        var steps = (int)Mathf.Abs((targetPosition.x - CurrentPosition.x) / Scaling);
        for (var i = 1; i < steps; i++) {
            var nextStep = new Vector3(CurrentPosition.x + i * Scaling * direction, CurrentPosition.y, CurrentPosition.z);
            if (Gm.GetPieces().ContainsKey(nextStep)) return false;
        }
        return true;
    }

    protected void CapturePiece(Vector3 vec) {
        var c = Gm.GetPieces()[vec];
        c.GetOwnPlayer().GetPiecesOfPlayer().Remove(c);
        c.transform.position = new Vector3(-0.2f, 2.37f, 12.17f);
        Gm.GetCapturedPieces().Add(Gm.GetPieces()[vec]);
    }


    protected bool DiagonalMove(Vector3 targetPosition) {
        // Überprüfen, ob die Bewegung tatsächlich diagonal ist
        var xDiff = Mathf.Abs(CurrentPosition.x - targetPosition.x);
        var zDiff = Mathf.Abs(CurrentPosition.z - targetPosition.z);
    
        if (Math.Abs(xDiff - zDiff) > 0.03f || xDiff == 0) {
            // Keine gültige diagonale Bewegung
            return false;
        }
    
        if (Gm.GetPieces().ContainsKey(targetPosition)) {
            // Ziel von eigener Figur besetzt
            if (Gm.GetPieces()[targetPosition].GetOwnPlayer() == GetOwnPlayer()) {
                return false;
            }
        }

        var xDirection = targetPosition.x > CurrentPosition.x ? 1 : -1;
        var zDirection = targetPosition.z > CurrentPosition.z ? 1 : -1;
    
        var steps = (int)(xDiff / Scaling); // Anzahl der Schritte bis zum Ziel
        var nextStep = CurrentPosition;

        for (var i = 1; i < steps; i++) {
            nextStep.x += Scaling * xDirection;
            nextStep.z += Scaling * zDirection;
        
            if (Gm.GetPieces().ContainsKey(nextStep)) {
                // Ein Stück blockiert den Weg
                return false;
            }
        }
        // Die Bewegung ist gültig
        return true;
    }



    protected void RefreshChessPieces() {
        foreach (var p in Gm.GetPieces().Values) {
            p.SnapPieceBack();
        }
    }


    private void SnapPieceBack() {
        //Debug.Log("neue Pos: " + transform.position + ", alte pos: " + CurrentPosition);
        var transform1 = transform;
        transform1.eulerAngles = new Vector3(0, 0, 0);
        transform1.position = CurrentPosition;
    }

    private void OnTriggerEnter(Collider other) {
        if (!(other.CompareTag("Thumb") || other.CompareTag("Index") || other.CompareTag("Middle"))) return;
            _touchingFingers++;
    }

    private void OnTriggerStay(Collider other) {
        if (!(other.CompareTag("Thumb") || other.CompareTag("Index") || other.CompareTag("Middle"))) return;
        if (_touchingFingers < 2) return;
        if (_located) return;
        _located = true;
    }


    private void OnTriggerExit(Collider other) {
        if (!(other.CompareTag("Thumb") || other.CompareTag("Index") || other.CompareTag("Middle"))) return;
        if (_touchingFingers > 0) _touchingFingers--;
        if (!_located) return;
        if (_touchingFingers > 0) return;
       
        _located = false;
        var mov = Move(transform.position);
        if (!mov) {
            RefreshChessPieces();
            return;
        }
        //Debug.Log("Gaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaandalf");
        Gm.PlayMoveSound();
        Gm.GameLoop();
    }
    
    private void UndoMove() {
        var log = Gm.GetHistory()[^1];

        if (log.GetSpecialMove() != null) {
            switch (log.GetSpecialMove()) {
                case "TransformationToQueen":
                    Gm.ChangeQueenToPawn(log.GetOldPos(),_player,_opponent, Gm);
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
        
        Gm.GetHistory().Remove(Gm.GetHistory()[^1]);
        var m = false;
            
        foreach (var lm in Gm.GetHistory().Where(lm => lm.GetSelf() == this)) {
            m = true;
        }

        hasMoved = m;
            
        Gm.GetPieces().Remove(log.GetNewPos());
            
        if (log.GetCapturedPiece() != null) {
            Gm.GetPieces().Add(log.GetNewPos(), log.GetCapturedPiece());
            log.GetCapturedPiece()._player.GetPiecesOfPlayer().Add(log.GetCapturedPiece());
            // TODO: geschlagenes Piece an position bewegen
        }
            
        CurrentPosition.x = log.GetOldPos().x;
        CurrentPosition.z = log.GetOldPos().z;
            
        Gm.GetPieces().Add(log.GetOldPos(), this);
        transform.position = CurrentPosition;
            
        Gm.SwitchPlayersTurn();
    }

}