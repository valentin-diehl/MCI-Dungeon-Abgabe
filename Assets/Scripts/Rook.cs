using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece {

    public Rook() {
        ChessPieceValue = 5;
    }
    protected override bool IsValidMove(Vector3 newPos) {
        if (!HorizontalMove(newPos) && !VerticalMove(newPos)) return false; 
        
        hasMoved = true; 
        return true;
    }
}