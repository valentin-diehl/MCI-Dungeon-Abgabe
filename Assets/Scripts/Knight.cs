using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Knight : ChessPiece {

    public Knight() {
        ChessPieceValue = 3;
    }

    protected override bool IsValidMove(Vector3 newPos) {
        
        var difX = (int)(Math.Abs(RoundMove(CurrentPosition.x /Scaling) - RoundMove(newPos.x /Scaling)));
        var difZ = (int)(Math.Abs(RoundMove(CurrentPosition.z /Scaling) - RoundMove(newPos.z /Scaling)));
        
        if (Gm.GetPieces().ContainsKey(newPos) && Gm.GetPieces()[newPos].GetOwnPlayer() == GetOwnPlayer()) return false;
        if (!(difX == 2  && difZ == 1 || difX == 1  && difZ == 2)) return false;
        return !Gm.GetPieces().ContainsKey(newPos) || Gm.GetPieces()[newPos].GetOwnPlayer() == GetOpponentPlayer();
    }
}