using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Queen : ChessPiece {

    public Queen() {
        ChessPieceValue = 9;
    }
    protected override bool IsValidMove(Vector3 newPos) {
        return HorizontalMove(newPos) || VerticalMove(newPos) || DiagonalMove(newPos);
    }
}