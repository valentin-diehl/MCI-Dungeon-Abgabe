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
        
        if(!Gm.GetPlayersTurn() && GetOwnPlayer().GetColor() == "White" || Gm.GetPlayersTurn() && GetOwnPlayer().GetColor() == "Black") return false;

        if (!IsValidMove(newPosition)) return false;
        
        Debug.Log("nach is valid");
        LogMove lm;
        var oldPos = CurrentPosition;
        if (Pieces.ContainsKey(newPosition)) {
            lm = new LogMove(this,oldPos, newPosition, Pieces[newPosition],null);
            GetOpponentPlayer().GetPiecesOfPlayer().Remove(Pieces[newPosition]);
            Pieces.Remove(newPosition);
        }
        else lm = new LogMove(this,oldPos, newPosition,null,null);
        Pieces.Remove(CurrentPosition);
        CurrentPosition = newPosition;

        Pieces.Add(CurrentPosition, this);
        transform.position = CurrentPosition;
        History.Add(lm);
        Gm.SwitchPlayersTurn();
        RefreshChessPieces();

        if (newPos.x is not (0 or 7)) return true;
        lm = new LogMove(this,newPosition, newPosition,null,"TransformationToQueen");
        History.Add(lm);
        GetOwnPlayer().GetPiecesOfPlayer().Remove(this);
        Gm.ChangePawnToQueen(newPosition,GetOwnPlayer(),GetOpponentPlayer(), Gm);
        return true;
    }

    protected override bool IsValidMove(Vector3 newPos) {

        var difX = RoundMove(Math.Abs(CurrentPosition.x - newPos.x)/Scaling)*Scaling;
        var difZ = RoundMove(Math.Abs(CurrentPosition.z - newPos.z)/Scaling)*Scaling;
        var rightDirection = false;

        if (GetOwnPlayer().GetColor().Equals("White")) rightDirection = CurrentPosition.x < newPos.x;
        if (GetOwnPlayer().GetColor().Equals("Black")) rightDirection = CurrentPosition.x > newPos.x;

        if (!Pieces.ContainsKey(newPos)) {
            if (!hasMoved && Math.Abs(difX - 2*Scaling) < Scaling/2  && difZ == 0 && rightDirection) return true;
            else if(Math.Abs(difX - Scaling) < Scaling/2 && difZ == 0 && rightDirection)return true;
            else return false;
        }
        
        return Math.Abs(difX - Scaling) < Scaling/2 && Math.Abs(difZ - Scaling) < Scaling/2 && rightDirection && Pieces[newPos].GetOwnPlayer() == GetOpponentPlayer();
    }
}