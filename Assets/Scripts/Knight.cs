using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Knight : ChessPiece {

    public Knight() {
        ChessPieceValue = 3;
    }

    protected override bool IsValidMove(Vector3 newPos) {
        float difX =Math.Abs(CurrentPosition.x - newPos.x), difZ = Math.Abs(CurrentPosition.z -newPos.z);
        if (!(difX == 2 && difZ == 1 || difX == 1 && difZ == 2)) return false;
        return Pieces[newPos] == null || Pieces[newPos].Player != Player;
    }
}