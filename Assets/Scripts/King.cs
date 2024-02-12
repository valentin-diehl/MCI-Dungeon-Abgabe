using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using UnityEngine;
using System; 
public class King : ChessPiece {
    
    public King() {
        chessPieceValue = 999;
    }

    protected override bool IsValidMove(Vector2 newPos){
        var difX = Math.Abs(base.x - newPos.x); 
        var difY = Math.Abs(base.y - newPos.y); 

        if(difY == 1 || difX == 1){
            //base.move(); 
            hasMoved = true; 
        }else if(difY == 1 && difX == 1){
            // base.move(); 
            hasMoved = true; 
        }
        return true;// switch
    }

}