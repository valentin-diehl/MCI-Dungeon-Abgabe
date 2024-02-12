using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece {
    
    public Pawn() {
        ChessPieceValue = 1;
    }
    
    protected override bool IsValidMove(Vector2 newPos) {
        var difX = (int)Math.Abs(x - newPos.x); 
        var difY = (int)Math.Abs(y - newPos.y); 

        // if(!base.hasMoved && difY <= 2) return true; 

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