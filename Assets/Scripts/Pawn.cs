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
        var difX = Math.Abs(x - newPos.x);
        var difY = Math.Abs(y - newPos.y);
        

        return false;
    }
    
    protected override bool IsValidMove(Vector2 newPos) {
        
        var newPosition = new Vector2(roundMove(newPos.x) , roundMove(newPos.y));
        if(!PlayersTurn && Player.GetColor() != "White" || !IsValidMove(newPosition)) return false;
        if(PlayersTurn && Player.GetColor() != "Black" || !IsValidMove(newPosition)) return false;
        
        LogMove lm;
        var oldPos = new Vector2(x, y);
        var difX = (int)Math.Abs(x - newPosition.x); 
        var difY = (int)Math.Abs(y - newPosition.y); 

        //TODO do funk again!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        if (Player.GetColor().Equals("white")) {

            if (!hasMoved && difX is 2 or 1 && difY == 0 && x < newPos.x && Pieces[newPosition] == null) {
                hasMoved = true;
                Pieces.Remove(new Vector2(x, y));
                x = (int)newPosition.x;
                y = (int)newPosition.y;
                lm = new LogMove(oldPos, newPosition, false,null,null);
                Pieces.Add(new Vector2(x, y), this);
                transform.position = new Vector3(x,0,y) * Time.deltaTime; 
                History.Add(lm);
                return true;
            }
            
        }
        else {
            
        }

        if(hasMoved){
            if(difY <= 1 && difX == 0 && Pieces[newPos] == null){
                hasMoved = true; 
            }else if(difX == 1 && difY == 1 && Pieces[newPos].Player != Player){
                hasMoved = true; 
            }
            return true; 
        }else{
            if(difY <= 2 && difX == 0 && Pieces[newPos] == null){
                hasMoved = true; 
            }else if(difX == 1 && difY == 1 && Pieces[newPos].Player != Player){
                hasMoved = true; 
            }
            return true; 
        }
    }
 
}