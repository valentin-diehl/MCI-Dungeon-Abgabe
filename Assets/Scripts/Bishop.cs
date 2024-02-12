using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece {
    
    public Bishop() {
        ChessPieceValue = 3;
    }
    public bool isValidMove(Vector2 newPos) {
        if (!base.DiagonalMove(newPos)) return false; 

        // base.move(newPos);
        base.hasMoved = true; 
        return true;
    }
}