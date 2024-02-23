using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using UnityEngine;
using System; 
public class King : ChessPiece {
    
    public King() {
        ChessPieceValue = 999;
    }

    public override bool Move(Vector3 newPos) {
        var newPosition = new Vector3(RoundMove(newPos.x / Scaling)*Scaling,Scaling, RoundMove(newPos.z / Scaling)*Scaling);
        if(!PlayersTurn && Player.GetColor() != "White" || !IsValidMove(newPosition)) return false;
        if(PlayersTurn && Player.GetColor() != "Black" || !IsValidMove(newPosition)) return false;
        
        LogMove lm;
        var oldPos = CurrentPosition;
        ChessPiece rook;
        if (IsValidMove(newPosition)) {

            if (Pieces[newPosition] != null) {
                lm = new LogMove(this,oldPos, newPosition, true, Pieces[newPosition],null);
                Opponent.GetPiecesOfPlayer().Remove(Pieces[newPosition]);
                Pieces.Remove(newPosition);
            }
            else lm = new LogMove(this,oldPos, newPosition, false,null,null);
            
            Pieces.Remove(oldPos);
            CurrentPosition = newPosition;

            Pieces.Add(CurrentPosition, this);
            transform.position = CurrentPosition; 
            History.Add(lm);
            SwitchPlayersTurn();
            return true;
        }
        if (IsValidRochade(newPosition)) {
            lm = new LogMove(this,oldPos, newPosition, false,null,"Rochade");
            //hier kein turn switch da dieser vom turmmove gewechselt wird
            rook = GetRookForRochade(newPosition);
            float i = 0, j = 3;
            if (newPosition.x > 0) i = 7*Scaling;
            if (newPosition.y > 3) j = 5 * Scaling;
            rook.Move(new Vector2(i, j));
            
            Pieces.Remove(oldPos);
            CurrentPosition = newPosition;

            Pieces.Add(CurrentPosition, this);
            transform.position = CurrentPosition; 
            History.Add(lm);
            return true;
        }
        return false;
    }

    private ChessPiece GetRookForRochade(Vector3 pos)
    {
        float i = 0, j = 0;
        if (pos.x > 0) i = 7*Scaling;
        if (pos.z > 2) j = 7*Scaling;
        return Pieces[new Vector2(i, j)];
    }

    protected override bool IsValidMove(Vector3 newPos){
        var difX = (int)Math.Abs(CurrentPosition.x - newPos.x); 
        var difZ = (int)Math.Abs(CurrentPosition.z - newPos.z); 

        if(difZ == 1 && difX == 0 || difX == 1 && difZ == 0 || difZ == 1 && difX == 1){
            hasMoved = true; 
        }
        return false;
    }
    
    private bool IsValidRochade(Vector3 newPos){

        if (hasMoved) return false;
        if (newPos.y != 0 && (int)newPos.y != 7) return false;

        var difX = Math.Abs(CurrentPosition.x - newPos.x);
        var difY = Math.Abs(CurrentPosition.z - newPos.z);
        if (difX is 2 && difY is 0){
            if (CurrentPosition.x > newPos.x){
                switch (Player.GetColor()){
                    case "Weiß" when Pieces[new Vector3(0, Scaling,0)].hasMoved:
                    case "Schwarz" when Pieces[new Vector3(0,Scaling, 7*Scaling)].hasMoved:
                        return false;
                }
                for (var i = Scaling; i < 4*Scaling; i+= Scaling)
                    if (Pieces[new Vector3(i,Scaling, newPos.z)] != null) return false;
            }
            else{
                switch (Player.GetColor()){
                    case "Weiß" when Pieces[new Vector3(7*Scaling, Scaling,0)].hasMoved:
                    case "Schwarz" when Pieces[new Vector3(7*Scaling,Scaling, 7*Scaling)].hasMoved:
                        return false;
                }
                for (var i = 5*Scaling; i < 7*Scaling; i+=Scaling)
                    if (Pieces[new Vector3(i,Scaling, newPos.z)] != null) return false;

            }
        }

        return true;
    }

}