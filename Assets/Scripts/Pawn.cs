using System;
using System.Collections;
using System.Collections.Generic;
using SG.Util;
using UnityEngine;

public class Pawn : ChessPiece {
    
    public Pawn() {
        ChessPieceValue = 1;
    }

    public override bool Move(Vector3 newPos) {
        Debug.Log("Pawn Move: " +name + ", "+ newPos );
        var xVal = RoundMove(newPos.x / Scaling) * Scaling;
        var zVal = RoundMove(newPos.z / Scaling) * Scaling;
        
        var newPosition = new Vector3(xVal,Scaling/2, zVal);
        
        if(!PlayersTurn && Player.GetColor() == "White" || PlayersTurn && Player.GetColor() == "Black") return false;

        if (!IsValidMove(newPosition)) return false;
        
        Debug.Log("nach is valid");
        LogMove lm;
        var oldPos = CurrentPosition;
        if (Pieces.ContainsKey(newPosition)) {
            lm = new LogMove(this,oldPos, newPosition, true, Pieces[newPosition],null);
            Opponent.GetPiecesOfPlayer().Remove(Pieces[newPosition]);
            Pieces.Remove(newPosition);
        }
        else lm = new LogMove(this,oldPos, newPosition, false,null,null);
        Pieces.Remove(CurrentPosition);
        CurrentPosition = newPosition;

        Pieces.Add(CurrentPosition, this);
        transform.position = CurrentPosition;
        History.Add(lm);
        SwitchPlayersTurn();

        if (newPos.x is not (0 or 7)) return true;
        lm = new LogMove(this,newPosition, newPosition, false,null,"TransformationToQueen");
        History.Add(lm);
        Player.GetPiecesOfPlayer().Remove(this);
        Gm.ChangePawnToQueen(newPosition,Player,Opponent, Gm);
        return true;
    }

    protected override bool IsValidMove(Vector3 newPos) {

        var difX = Math.Abs(CurrentPosition.x - newPos.x);
        var difZ = Math.Abs(CurrentPosition.z - newPos.z);
        var rightDirection = false;

        if (Player.GetColor().Equals("White")) rightDirection = CurrentPosition.x < newPos.x;
        if (Player.GetColor().Equals("Black")) rightDirection = CurrentPosition.x > newPos.x;

        if (!Pieces.ContainsKey(newPos)) {
            if (((!hasMoved && Math.Abs(difX - 2*Scaling) < Scaling ) || Math.Abs(difX - Scaling) < Scaling) && difZ == 0 && rightDirection) return true;
                    
            return Math.Abs(difX - Scaling) < Scaling && Math.Abs(difZ - Scaling) < Scaling && rightDirection;
        }
        
        return Math.Abs(difX - Scaling) < Scaling && Math.Abs(difZ - Scaling) < Scaling && rightDirection && Pieces[newPos].Player == Opponent;
    }
}