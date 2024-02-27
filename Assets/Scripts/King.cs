using UnityEngine;
using System;
using Unity.VisualScripting;

public class King : ChessPiece {
    
    public King() {
        ChessPieceValue = 999;
    }

    public override bool Move(Vector3 newPos) {
        
        Debug.Log("King Move: " +name + ", "+ newPos );
        var xVal = RoundMove(newPos.x / Scaling) * Scaling;
        var zVal = RoundMove(newPos.z / Scaling) * Scaling;
        
        var newPosition = new Vector3(xVal,Scaling/2, zVal);
        
        if(!Gm.GetPlayersTurn() && GetOwnPlayer().GetColor() == "White" || Gm.GetPlayersTurn() && GetOwnPlayer().GetColor() == "Black") return false;
        
        LogMove lm;
        switch (IsValidMove(newPosition))
        {
            case false when !IsValidCastling(newPosition):
                return false;
            case true:
            {
                if (Pieces.ContainsKey(newPosition)){
                    lm = new LogMove(this,CurrentPosition, newPosition, Pieces[newPosition],null);
                    GetOpponentPlayer().GetPiecesOfPlayer().Remove(Pieces[newPosition]);
                    Pieces.Remove(newPosition);
                }
                else lm = new LogMove(this,CurrentPosition, newPosition,null,null);
            
                Pieces.Remove(CurrentPosition);
                CurrentPosition = newPosition;

                Pieces.Add(CurrentPosition, this);
                transform.position = CurrentPosition; 
                History.Add(lm);
                Gm.SwitchPlayersTurn();
                RefreshChessPieces();
                return true;
            }
        }


        lm = new LogMove(this,CurrentPosition, newPosition,null,"Rochade");
        //hier kein turn switch da dieser vom turmmove gewechselt wird
        var rook = GetRookForCastling(newPosition);
        if (rook != null) {
            float i = 0;
            var j = 3*Scaling;
            if (newPosition.x > 0) i = 7*Scaling;
            if (newPosition.y > 3*Scaling) j = 5 * Scaling;
            rook.Move(new Vector3(i,Scaling/2, j));
                
            Pieces.Remove(CurrentPosition);
            CurrentPosition = newPosition;
    
            Pieces.Add(CurrentPosition, this);
            transform.position = CurrentPosition; 
            History.Add(lm);
            RefreshChessPieces();
            return true;
        }

        return false;
    }

    private ChessPiece GetRookForCastling(Vector3 pos) {
        //Todo giv pos not piece
        float i = 0, j = 0;
        if (pos.x > 0) i = 7*Scaling;
        if (pos.z > 2) j = 7*Scaling;
        var v = new Vector3(i, Scaling / 2, j);
        return Pieces.ContainsKey(v) ? Pieces[v] : null;
    }

    protected override bool IsValidMove(Vector3 newPos){
        var difX = Math.Abs(CurrentPosition.x - newPos.x);
        var difZ = Math.Abs(CurrentPosition.z - newPos.z);

        if (Pieces.ContainsKey(newPos) && Pieces[newPos].GetOwnPlayer() == GetOwnPlayer()) {
                Debug.Log("Raaaandalf digga");
                return false;
        }
        return Math.Abs(difZ - Scaling) < Scaling/2 && difX == 0 || Math.Abs(difX - Scaling) < Scaling/2 && difZ == 0 || Math.Abs(difZ - Scaling) < Scaling/2 && Math.Abs(difX - Scaling) < Scaling/2;
        
    }
    
    private bool IsValidCastling(Vector3 newPos){

        if (hasMoved) return false;
        if (newPos.x != 0 && Math.Abs(newPos.x - 7*Scaling) > Scaling /2) return false;
        if (Pieces.ContainsKey(newPos) && Pieces[newPos].GetOwnPlayer() == GetOwnPlayer()) {
                Debug.Log("Raaaandalf digga in rochi");
                return false;
        } 
        

        var difX = RoundMove(Math.Abs(CurrentPosition.x - newPos.x)/Scaling)*Scaling;
        var difZ = RoundMove(Math.Abs(CurrentPosition.z - newPos.z)/Scaling)*Scaling;

        if (difZ is not 2 || difX is not 0) return false;//TODO check if this is the right way
        if (CurrentPosition.x > newPos.x){
            switch (GetOwnPlayer().GetColor()){
                case "Weiß" when Pieces[new Vector3(0, Scaling,0)].hasMoved:
                case "Schwarz" when Pieces[new Vector3(0,Scaling, 7*Scaling)].hasMoved:
                    return false;
            }
            for (var i = Scaling; i < 4*Scaling; i+= Scaling)
                if (Pieces[new Vector3(i,Scaling, newPos.z)] != null) return false;
        }
        else{
            switch (GetOwnPlayer().GetColor()){
                case "Weiß" when Pieces[new Vector3(7*Scaling, Scaling,0)].hasMoved:
                case "Schwarz" when Pieces[new Vector3(7*Scaling,Scaling, 7*Scaling)].hasMoved:
                    return false;
            }
            for (var i = 5*Scaling; i < 7*Scaling; i+=Scaling)
                if (Pieces[new Vector3(i,Scaling, newPos.z)] != null) return false;

        }

        return true;
    }

}