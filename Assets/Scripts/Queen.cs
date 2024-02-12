using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Queen : ChessPiece {

    public Queen() {
        ChessPieceValue = 9;
    }
    protected override bool IsValidMove(Vector2 newPos) {
        if (!HorizontalMove(newPos) && !VerticalMove(newPos) && !DiagonalMove(newPos)) return false; 

        // base.move(newPos);
        hasMoved = true; 
        return true;
    }
}