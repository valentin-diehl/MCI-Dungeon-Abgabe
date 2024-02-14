using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using UnityEngine;
using System; 
public class King : ChessPiece {
    
    public King() {
        ChessPieceValue = 999;
    }

    public override bool Move(Vector2 newPos) {
        
        var newPosition = new Vector2(roundMove(newPos.x) , roundMove(newPos.y));
        if(!PlayersTurn && Player.GetColor() != "White" || !IsValidMove(newPosition)) return false;
        if(PlayersTurn && Player.GetColor() != "Black" || !IsValidMove(newPosition)) return false;
        
        LogMove lm;
        var oldPos = new Vector2(x, y);
        ChessPiece rook;
        if (IsValidMove(newPosition)) {

            if (Pieces[newPosition] != null) {
                lm = new LogMove(this,oldPos, newPosition, true, Pieces[newPosition],null);
                Opponent.GetPiecesOfPlayer().Remove(Pieces[newPosition]);
                Pieces.Remove(newPosition);
            }
            else lm = new LogMove(this,oldPos, newPosition, false,null,null);
            
            Pieces.Remove(new Vector2(x, y));
            x = (int)newPosition.x;
            y = (int)newPosition.y;

            Pieces.Add(new Vector2(x, y), this);
            transform.position = new Vector3(x,0,y) * Time.deltaTime; 
            History.Add(lm);
            SwitchPlayersTurn();
            return true;
        }
        if (IsValidRochade(newPosition)) {
            lm = new LogMove(this,oldPos, newPosition, false,null,"Rochade");
            //hier kein turn switch da dieser vom turmmove gewechselt wird
            rook = GetRookForRochade(newPosition);
            int i = 0, j = 3;
            if (newPosition.x > 0) i = 7;
            if (newPosition.y > 3) j = 5;
            rook.Move(new Vector2(i, j));
            
            Pieces.Remove(new Vector2(x, y));
            x = (int)newPosition.x;
            y = (int)newPosition.y;

            Pieces.Add(new Vector2(x, y), this);
            transform.position = new Vector3(x,0,y) * Time.deltaTime; 
            History.Add(lm);
            return true;
        }
        return false;
    }

    private ChessPiece GetRookForRochade(Vector2 pos)
    {
        int i = 0, j = 0;
        if (pos.x > 0) i = 7;
        if (pos.y > 2) j = 7;
        return Pieces[new Vector2(i, j)];
    }

    protected override bool IsValidMove(Vector2 newPos){
        var difX = (int)Math.Abs(x - newPos.x); 
        var difY = (int)Math.Abs(y - newPos.y); 

        if(difY == 1 && difX == 0 || difX == 1 && difY == 0 || difY == 1 && difX == 1){
            hasMoved = true; 
        }
        return false;
    }
    
    private bool IsValidRochade(Vector2 newPos){

        if (hasMoved) return false;
        if (newPos.y != 0 && (int)newPos.y != 7) return false;

        var difX = Math.Abs(x - newPos.x);
        var difY = Math.Abs(y - newPos.y);
        if (difX is 2 && difY is 0){
            if (x > newPos.x){
                switch (Player.GetColor()){
                    case "Weiß" when Pieces[new Vector2(0, 0)].hasMoved:
                    case "Schwarz" when Pieces[new Vector2(0, 7)].hasMoved:
                        return false;
                }
                for (var i = 1; i < 4; i++)
                    if (Pieces[new Vector2(i, newPos.y)] != null) return false;
            }
            else{
                switch (Player.GetColor()){
                    case "Weiß" when Pieces[new Vector2(7, 0)].hasMoved:
                    case "Schwarz" when Pieces[new Vector2(7, 7)].hasMoved:
                        return false;
                }
                for (var i = 5; i < 7; i++)
                    if (Pieces[new Vector2(i, newPos.y)] != null) return false;

            }
        }

        return true;
    }

}