using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class ChessPiece : MonoBehaviour {
    private Player _player;
    private Player _opponent;
    protected int ChessPieceValue;
    protected Vector3 CurrentPosition;
    protected List<LogMove> History;
    protected const float Scaling = 0.1f;
    protected GameManager Gm;

    public bool hasMoved;

    /* Interaction Glove*/

    private int _touchingFingers;
    private bool _located;
    private Vector3 _firstLocation;


    protected virtual bool IsValidMove(Vector3 newPosition) {
        return true;
    }

    public int GetChessPieceValue() {
        return ChessPieceValue;
    }

    public void Init(Player player, Player opponent,
        List<LogMove> history, GameManager gm, Vector3 position) {
        _touchingFingers = 0;
        Gm = gm;
        _player = player;
        _opponent = opponent;
        History = history;
        CurrentPosition = position;
        hasMoved = false;
    }

    public Player GetOwnPlayer() {
        return _player;
    }
    public Player GetOpponentPlayer() {
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
        Debug.Log("Casual Move: " +name + ", "+ newPos );
        var xVal = RoundMove(newPos.x / Scaling) * Scaling;
        var zVal = RoundMove(newPos.z / Scaling) * Scaling;
        
        var newPosition = new Vector3(xVal,Scaling/2, zVal);

        if (Gm.GetPieces().ContainsKey(newPosition) && Gm.GetPieces()[newPosition]._player == _player) return false;
        if(!Gm.GetPlayersTurn() && _player.GetColor() == "White" || Gm.GetPlayersTurn() && _player.GetColor() == "Black") return false;
        LogMove lm;
        if (Gm.GetPieces().ContainsKey(newPosition)) {
            lm = new LogMove(this, CurrentPosition, newPosition,Gm.GetPieces()[newPosition], null);
            _opponent.GetPiecesOfPlayer().Remove(Gm.GetPieces()[newPosition]);
            Gm.GetPieces().Remove(newPosition);
        }
        else lm = new LogMove(this, CurrentPosition, newPosition,null, null);

        Gm.GetPieces().Remove(CurrentPosition);
        CurrentPosition = newPosition;

        Gm.GetPieces().Add(CurrentPosition, this);
        transform.position = CurrentPosition;
        History.Add(lm);

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


    protected bool VerticalMove(Vector3 vec) {
        float limit = 0, init = 0;
        if (Math.Abs(vec.z - CurrentPosition.z) > 0) return false;
        if (vec.x > CurrentPosition.x) {
            limit = vec.x;
            init = CurrentPosition.x + Scaling;
        }
        else if (vec.x < CurrentPosition.x) {
            init = vec.x + Scaling;
            limit = CurrentPosition.x;
        }

        for (var i = init; i <= limit; i+=Scaling) {
            var nVec = new Vector3(i,Scaling/2, vec.z);
            if (i - limit == 0 && (Gm.GetPieces().ContainsKey(nVec) && Gm.GetPieces()[nVec]._player == _opponent) || !Gm.GetPieces().ContainsKey(nVec)) return true;
            if (Gm.GetPieces().ContainsKey(nVec)) return false;
        }
        return true;
    }
    

    protected bool HorizontalMove(Vector3 vec) {
        float limit = 0, init = 0;
        if (Math.Abs(vec.x - CurrentPosition.x) > 0) return false;
        if (vec.z > CurrentPosition.z) {
            limit = vec.z;
            init = CurrentPosition.z + Scaling;
        }
        else if (vec.z < CurrentPosition.z) {
            init = vec.z + Scaling;
            limit = CurrentPosition.z;
        }

        for (var i = init; i <= limit; i+=Scaling) {
            var nVec = new Vector3(vec.x,Scaling/2, i);
            if (i - limit == 0 && (Gm.GetPieces().ContainsKey(nVec) && Gm.GetPieces()[nVec]._player == _opponent) || !Gm.GetPieces().ContainsKey(nVec)) return true;
            if (Gm.GetPieces().ContainsKey(nVec)) return false;
        }
        return true;
    }

    protected bool DiagonalMove(Vector3 vec) {
        if ((Math.Abs(CurrentPosition.x - CurrentPosition.z) - Math.Abs(vec.x - vec.z)) > 0) return false;
        if (Gm.GetPieces().ContainsKey(vec) && Gm.GetPieces()[vec]._player == _player) return false;
        
        //to right bottom
        if (CurrentPosition.x > vec.x && CurrentPosition.z > vec.z) {
            for (var i = vec.x + Scaling; i >= CurrentPosition.x; i+=Scaling) {
                var nVec = new Vector3(CurrentPosition.x - i,Scaling/2, CurrentPosition.z - i);
                var difX = Math.Abs(CurrentPosition.x - nVec.x);
                var difZ = Math.Abs(CurrentPosition.z - nVec.z);
                if (Gm.GetPieces().ContainsKey(nVec) &&  difX == 0 &&  difZ == 0 && Gm.GetPieces()[nVec]._player == _opponent) return true;
                if (Gm.GetPieces().ContainsKey(nVec)) return false;
            }
        }
        //to left bottom
        else if (CurrentPosition.x > vec.x && CurrentPosition.z < vec.z) {
            for (var i = vec.x + Scaling; i >= CurrentPosition.x; i+=Scaling) {
                var nVec = new Vector3(CurrentPosition.x - i,Scaling/2, CurrentPosition.z + i);
                var difX = Math.Abs(CurrentPosition.x - nVec.x);
                var difZ = Math.Abs(CurrentPosition.z - nVec.z);
                if (Gm.GetPieces().ContainsKey(nVec) &&  difX == 0 &&  difZ == 0 && Gm.GetPieces()[nVec]._player == _opponent) return true;
                if (Gm.GetPieces().ContainsKey(nVec)) return false;
            }
        }
        //to top left
        else if (CurrentPosition.x < vec.x && CurrentPosition.z < vec.z) {
            for (var i = CurrentPosition.x + Scaling; i >= vec.x; i+=Scaling) {
                var nVec = new Vector3(CurrentPosition.x + i,Scaling/2, CurrentPosition.z + i);
                var difX = Math.Abs(vec.x - nVec.x);
                var difZ = Math.Abs(vec.z - nVec.z);
                if (Gm.GetPieces().ContainsKey(nVec) &&  difX == 0 &&  difZ == 0 && Gm.GetPieces()[nVec]._player == _opponent) return true;
                if (Gm.GetPieces().ContainsKey(nVec)) return false;
            }
        }
        //to top right
        else if (CurrentPosition.x < vec.x && CurrentPosition.z > vec.z) {
            for (var i = CurrentPosition.x + Scaling; i >= vec.x; i+=Scaling) {
                var nVec = new Vector3(CurrentPosition.x + i,Scaling/2, CurrentPosition.z - i);
                var difX = Math.Abs(vec.x - nVec.x);
                var difZ = Math.Abs(vec.z - nVec.z);
                if (Gm.GetPieces().ContainsKey(nVec) &&  difX == 0 &&  difZ == 0 && Gm.GetPieces()[nVec]._player == _opponent) return true;
                if (Gm.GetPieces().ContainsKey(nVec)) return false;
            }
        }
        return true;
    }

    public void RefreshChessPieces() {
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
        _firstLocation = transform.position;
    }


    private void OnTriggerExit(Collider other) {
        if (!(other.CompareTag("Thumb") || other.CompareTag("Index") || other.CompareTag("Middle"))) return;
        if (_touchingFingers > 0) _touchingFingers--;
        if (!_located) return;
        if (_touchingFingers > 0) return;
        if (_firstLocation == transform.position) return;
        var mov = Move(transform.position);
        if (mov) {
            Debug.Log("Gaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaandalf");
            _located = false;
            return;
        }
        RefreshChessPieces();
        _located = false;
    }
    
    private void UndoMove() {
        var log = History[^1];

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
        
        History.Remove(History[^1]);
        var m = false;
            
        foreach (var lm in History.Where(lm => lm.GetSelf() == this)) {
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