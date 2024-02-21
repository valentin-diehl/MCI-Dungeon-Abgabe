using System;
using System.Collections;
using System.Collections.Generic;
using SG.Util;
using UnityEngine;

public class Pawn : ChessPiece {
    
    public Pawn() {
        ChessPieceValue = 1;
    }

    public override bool Move(Vector2 newPos) {
        
        var newPosition = new Vector2(RoundMove(newPos.x/Scaling) , RoundMove(newPos.y/Scaling));
        if(!PlayersTurn && Player.GetColor() != "White" || !IsValidMove(newPosition)) return false;
        if(PlayersTurn && Player.GetColor() != "Black" || !IsValidMove(newPosition)) return false;


        if (!IsValidMove(newPosition)) return false;
        
        LogMove lm;
        var oldPos = new Vector2(currentX, currentY);
        if (Pieces[newPosition] != null) {
            lm = new LogMove(this,oldPos, newPosition, true, Pieces[newPosition],null);
            Opponent.GetPiecesOfPlayer().Remove(Pieces[newPosition]);
            Pieces.Remove(newPosition);
        }
        else lm = new LogMove(this,oldPos, newPosition, false,null,null);
        Pieces.Remove(new Vector2(currentX, currentY));
        currentX = (int)newPosition.x;
        currentY = (int)newPosition.y;

        Pieces.Add(new Vector2(currentX, currentY), this);
        transform.position = new Vector3(currentX*Scaling,0.05f,currentY*Scaling) * Time.deltaTime; 
        History.Add(lm);
        SwitchPlayersTurn();
            
        if (newPos.x is 0 or 7) {
            lm = new LogMove(this,newPosition, newPosition, false,null,"TransformationToQueen");
            History.Add(lm);
            Player.GetPiecesOfPlayer().Remove(this);
            Gm.ChangePawnToQueen(newPosition,Player,Opponent, Gm);
        }
        return true;
    }

    protected override bool IsValidMove(Vector2 newPos) {

        var newPosition = new Vector2(RoundMove(newPos.x), RoundMove(newPos.y));
        if (!PlayersTurn && Player.GetColor() != "White") return false;
        if (PlayersTurn && Player.GetColor() != "Black") return false;

        LogMove lm;
        var oldPos = new Vector2(currentX, currentY);
        var difX = (int)Math.Abs(currentX - newPosition.x);
        var difY = (int)Math.Abs(currentY - newPosition.y);
        var rightDirection = false;

        if (Player.GetColor().Equals("White")) rightDirection = currentX < newPos.x;
        if (Player.GetColor().Equals("Black")) rightDirection = currentX > newPos.x;

        if (((!hasMoved && difX is 2 or 1) || difX == 1) &&
            (difY == 0 && rightDirection && Pieces[newPosition] == null))
        {
            hasMoved = true;
            Pieces.Remove(new Vector2(currentX, currentY));
            currentX = (int)newPosition.x;
            currentY = (int)newPosition.y;
            lm = new LogMove(this,oldPos, newPosition, false, null, null);
            Pieces.Add(new Vector2(currentX, currentY), this);
            transform.position = new Vector3(currentX, 0, currentY) * Time.deltaTime;
            History.Add(lm);
            return true;
        }

        if (difX == 1 && difY == 1 && rightDirection && Pieces[newPosition].Player == Opponent)
        {
            hasMoved = true;
            Pieces.Remove(new Vector2(currentX, currentY));
            Opponent.GetPiecesOfPlayer().Remove(Pieces[newPosition]);
            Pieces.Remove(newPosition);
            Opponent.GetPiecesOfPlayer().Remove(Pieces[newPosition]);
            currentX = (int)newPosition.x;
            currentY = (int)newPosition.y;
            lm = new LogMove(this,oldPos, newPosition, false, Pieces[newPosition], null);
            Pieces.Add(new Vector2(currentX, currentY), this);
            transform.position = new Vector3(currentX, 0, currentY) * Time.deltaTime;
            History.Add(lm);
            return true;
        }

        return false;
    }
}