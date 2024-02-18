using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Knight : ChessPiece {

    public Knight() {
        ChessPieceValue = 3;
    }

    protected override bool IsValidMove(Vector2 newPos) {
        float difX =Math.Abs(X - newPos.x), difY = Math.Abs(Y -newPos.y);
        if (!(difX == 2 && difY == 1 || difX == 1 && difY == 2)) return false;
        return Pieces[newPos] == null || Pieces[newPos].Player != Player;
    }
}