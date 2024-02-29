using System;
using System.Collections;
using System.Collections.Generic;
using SG.Util;
using UnityEngine;

public class Pawn : ChessPiece {
    
    public Pawn() {
        ChessPieceValue = 1;
    }

    public override bool Move(Vector3 newPos) {
        var xVal = RoundMove(newPos.x / Scaling) * Scaling;
        var zVal = RoundMove(newPos.z / Scaling) * Scaling;
        
        var newPosition = new Vector3(xVal,Scaling/2, zVal);
        
        if(!Gm.GetPlayersTurn() && GetOwnPlayer().GetColor() == PlayerColor.White || Gm.GetPlayersTurn() && GetOwnPlayer().GetColor() == PlayerColor.Black) return false;

        if (!IsValidMove(newPosition)) return false;
        
        LogMove lm;
        var oldPos = CurrentPosition;
        if (Gm.GetPieces().ContainsKey(newPosition)) {
            CapturePiece(newPosition);
            lm = new LogMove(this,oldPos, newPosition, Gm.GetPieces()[newPosition],null);
        }
        else lm = new LogMove(this,oldPos, newPosition,null,null);
        
        Gm.GetPieces().Remove(CurrentPosition);
        
        CurrentPosition = newPosition;

        Gm.GetPieces()[CurrentPosition] = this;
        transform.position = CurrentPosition;
        Gm.AddToHistory(lm);
        Gm.SwitchPlayersTurn();
        RefreshChessPieces();
        hasMoved = true;

        if (newPosition.x is not (0 or 7 * Scaling)) return true;
        lm = new LogMove(this,newPosition, newPosition,null,"TransformationToQueen");
        Gm.AddToHistory(lm);
        CapturePiece(CurrentPosition);
        Gm.ChangePawnToQueen(CurrentPosition,GetOwnPlayer(),GetOpponentPlayer(),Gm);
        return true;
    }

    protected override bool IsValidMove(Vector3 newPos) {
        var difX = RoundMove(Math.Abs(CurrentPosition.x - newPos.x) / Scaling) * Scaling;
        var difZ = RoundMove(Math.Abs(CurrentPosition.z - newPos.z) / Scaling) * Scaling;
        var rightDirection = false;

        if (GetOwnPlayer().GetColor()== PlayerColor.White) rightDirection = CurrentPosition.x < newPos.x;
        if (GetOwnPlayer().GetColor()== PlayerColor.Black) rightDirection = CurrentPosition.x > newPos.x;
        if (!rightDirection) return false;

        // Prüfen Sie, ob das Feld zwischen der aktuellen Position und der neuen Position frei ist, wenn der Bauer versucht, zwei Felder vorwärts zu bewegen.
        if (!hasMoved && Math.Abs(difX - 2 * Scaling) < Scaling / 2 && difZ == 0f) {
            // Berechnen der Position direkt vor dem Bauern
            var intermediateX = GetOwnPlayer().GetColor() == PlayerColor.White ? CurrentPosition.x + Scaling : CurrentPosition.x - Scaling;
            var intermediatePosition = new Vector3(intermediateX, newPos.y, newPos.z);

            // Prüfen, ob sowohl das direkte Feld vor dem Bauern als auch das Ziel zwei Felder vorwärts frei sind
            return !Gm.GetPieces().ContainsKey(intermediatePosition) && !Gm.GetPieces().ContainsKey(newPos);
        }

        // Standardbewegung um ein Feld nach vorne, ohne Gegner zu schlagen
        if (!Gm.GetPieces().ContainsKey(newPos)) {
            return Math.Abs(difX - Scaling) < Scaling / 2 && difZ == 0f;
        }

        // Schlagbewegung: Ein Feld diagonal vorwärts, wenn dort eine gegnerische Figur steht
        return Math.Abs(difX - Scaling) < Scaling / 2 && Math.Abs(difZ - Scaling) < Scaling / 2 && Gm.GetPieces()[newPos].GetOwnPlayer() == GetOpponentPlayer();
    }

}