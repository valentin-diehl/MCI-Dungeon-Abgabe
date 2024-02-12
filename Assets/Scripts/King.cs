using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using UnityEngine;
using System; 
public class King : ChessPiece {
    
    public King() {
        ChessPieceValue = 999;
    }

    protected override bool IsValidMove(Vector2 newPos){
        var difX = (int)Math.Abs(x - newPos.x); 
        var difY = (int)Math.Abs(y - newPos.y); 

        if(difY == 1 && difX == 0 || difX == 1 && difY == 0 || difY == 1 && difX == 1){
            hasMoved = true; 
        }
        
        
        return false;
    }

}