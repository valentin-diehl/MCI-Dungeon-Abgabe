using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Knight : ChessPiece {

    public Knight() {
        ChessPieceValue = 3;
    }

    protected override bool IsValidMove(Vector3 newPos) {
        var difX = RoundMove(Math.Abs(CurrentPosition.x - newPos.x)/Scaling)*Scaling;
        var difZ = RoundMove(Math.Abs(CurrentPosition.z - newPos.z)/Scaling)*Scaling;
        if (!(Math.Abs(difX - 2*Scaling) < Scaling/2 && Math.Abs(difZ - 1*Scaling) < Scaling/2 || Math.Abs(difX - 1*Scaling) < Scaling/2 && Math.Abs(difZ - 2*Scaling) < Scaling/2)) return false;
        return !Gm.GetPieces().ContainsKey(newPos) || Gm.GetPieces()[newPos].GetOwnPlayer() == GetOpponentPlayer();
    }
}