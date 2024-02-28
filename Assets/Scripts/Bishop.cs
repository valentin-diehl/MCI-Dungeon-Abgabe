using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece {
    
    public Bishop() {
        ChessPieceValue = 3;
    }
    protected override bool IsValidMove(Vector3 newPos) {
        Debug.Log("Löufer: " + DiagonalMove(newPos));
        return DiagonalMove(newPos);
    }
}