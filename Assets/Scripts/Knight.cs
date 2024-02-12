using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Knight : ChessPiece {

    public Knight() {
        chessPieceValue = 3;
    }
    public bool IsValidMove(Vector2 newPos)
    {
        float difX =Math.Abs(base.x - newPos.x), difY = Math.Abs(base.y -newPos.y);
        if (!(difX == 2 && difY == 1 || difX == 1 && difY == 2)) return false;
        if (Pieces[newPos] != null && Pieces[newPos].Player == this.Player) return false;
        //move
        return true;
    }
}