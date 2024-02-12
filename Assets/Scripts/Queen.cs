using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Queen : ChessPiece {

    public Queen() {
        chessPieceValue = 9;
    }
    protected override bool IsValidMove(Vector2 newPos) {
        if (!horizontalMove(newPos) && !verticalMove(newPos) && !diagonalMove(newPos)) return false; 

        // base.move(newPos);
        hasMoved = true; 
        return true;
    }
}