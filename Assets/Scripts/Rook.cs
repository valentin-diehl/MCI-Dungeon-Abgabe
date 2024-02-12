using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece {

    public Rook() {
        chessPieceValue = 5;
    }
    protected override bool IsValidMove(Vector2 newPos) {
        if (!HorizontalMove(newPos) && !VerticalMove(newPos)) return false; 

        // base.move(newPos);
        hasMoved = true; 
        return true;
    }
}