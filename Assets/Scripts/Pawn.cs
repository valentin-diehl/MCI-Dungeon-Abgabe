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
        
        
        var newPosition = new Vector3(RoundMove(newPos.x / Scaling)*Scaling,Scaling, RoundMove(newPos.z / Scaling)*Scaling);
        if(!PlayersTurn && Player.GetColor() != "White" || !IsValidMove(newPosition)) return false;
        if(PlayersTurn && Player.GetColor() != "Black" || !IsValidMove(newPosition)) return false;


        if (!IsValidMove(newPosition)) return false;
        
        LogMove lm;
        var oldPos = CurrentPosition;
        if (Pieces[newPosition] != null) {
            lm = new LogMove(this,oldPos, newPosition, true, Pieces[newPosition],null);
            Opponent.GetPiecesOfPlayer().Remove(Pieces[newPosition]);
            Pieces.Remove(newPosition);
        }
        else lm = new LogMove(this,oldPos, newPosition, false,null,null);
        Pieces.Remove(new Vector3(CurrentPosition.x,Scaling, CurrentPosition.z));
        CurrentPosition = newPosition;

        Pieces.Add(CurrentPosition, this);
        transform.position = new Vector3(CurrentPosition.x*Scaling,Scaling,CurrentPosition.z*Scaling) * Time.deltaTime; 
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

        switch (PlayersTurn)
        {
            case false when Player.GetColor() != "White":
            case true when Player.GetColor() != "Black":
                return false;
        }

        var difX = (int)Math.Abs(CurrentPosition.x - newPos.x);
        var difY = (int)Math.Abs(CurrentPosition.z - newPos.z);
        var rightDirection = false;

        if (Player.GetColor().Equals("White")) rightDirection = CurrentPosition.x < newPos.x;
        if (Player.GetColor().Equals("Black")) rightDirection = CurrentPosition.x > newPos.x;

        if (((!hasMoved && difX is 2 or 1) || difX == 1) && (difY == 0 && rightDirection && Pieces[newPos] == null)) {
            return true;
        }

        return difX == 1 && difY == 1 && rightDirection && Pieces[newPos].Player == Opponent;
    }
}