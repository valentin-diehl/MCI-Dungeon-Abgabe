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
        
        Debug.Log("King Move neue Koordinaten: " +name + ", "+ newPosition);
        
        if(!Gm.GetPlayersTurn() && GetOwnPlayer().GetColor() == "White" || Gm.GetPlayersTurn() && GetOwnPlayer().GetColor() == "Black") return false;
        
        if(Gm.GetPieces().ContainsKey(newPosition)) Debug.Log("Its iiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiin");
        LogMove lm;
        switch (IsValidMove(newPosition)) {
            case false when !IsValidCastling(newPosition):
                return false;
            case true: {
                Debug.Log("Mein programm ist eine miisssssssssssssssit");
                if (Gm.GetPieces().ContainsKey(newPosition)){
                    lm = new LogMove(this,CurrentPosition, newPosition, Gm.GetPieces()[newPosition],null);
                    GetOpponentPlayer().GetPiecesOfPlayer().Remove(Gm.GetPieces()[newPosition]);
                    Gm.GetPieces().Remove(newPosition);
                }
                else lm = new LogMove(this,CurrentPosition, newPosition,null,null);
            
                Gm.GetPieces().Remove(CurrentPosition);
                CurrentPosition = newPosition;

                Gm.GetPieces().Add(CurrentPosition, this);
                transform.position = CurrentPosition; 
                History.Add(lm);
                Gm.SwitchPlayersTurn();
                RefreshChessPieces();
                return true;
            }
        }

        Debug.Log("total random");
        lm = new LogMove(this,CurrentPosition, newPosition,null,"Rochade");
        //hier kein turn switch da dieser vom turmmove gewechselt wird
        var rook = GetRookForCastling(newPosition);
        if (rook != null) {
            float i = 0;
            var j = 3*Scaling;
            if (newPosition.x > 0) i = 7*Scaling;
            if (newPosition.y > 3*Scaling) j = 5 * Scaling;
            rook.Move(new Vector3(i,Scaling/2, j));
                
            Gm.GetPieces().Remove(CurrentPosition);
            CurrentPosition = newPosition;
    
            Gm.GetPieces().Add(CurrentPosition, this);
            transform.position = CurrentPosition; 
            History.Add(lm);
            RefreshChessPieces();
            return true;
        }

        return false;
    }
    
    

   

    protected override bool IsValidMove(Vector3 newPos){
        var difX = Math.Abs(CurrentPosition.x - newPos.x);
        var difZ = Math.Abs(CurrentPosition.z - newPos.z);
        if(difX > Scaling || difZ > Scaling) return false;

        if (!Gm.GetPieces().ContainsKey(newPos)) return true;
        
        Debug.Log("Name der Figur die hier ist" + Gm.GetPieces()[newPos].name);
        return Gm.GetPieces()[newPos].GetOwnPlayer() == GetOpponentPlayer();

    }
     private ChessPiece GetRookForCastling(Vector3 pos) {
            //Todo giv pos not piece
            float i = 0, j = 0;
            if (pos.x > 0) i = 7*Scaling;
            if (pos.z > 2) j = 7*Scaling;
            var v = new Vector3(i, Scaling / 2, j);
            return Gm.GetPieces().ContainsKey(v) ? Gm.GetPieces()[v] : null;
        }
    private bool IsValidCastling(Vector3 newPos){

        if (hasMoved) return false;
        
        var difX = Math.Abs(CurrentPosition.x - newPos.x);
        var difZ = Math.Abs(CurrentPosition.z - newPos.z);
        
        if (difX > 0  || difZ > 2*Scaling || difZ < Scaling) return false;
        if (Gm.GetPieces().ContainsKey(newPos) && Gm.GetPieces()[newPos].GetOwnPlayer() == GetOwnPlayer()) {
                Debug.Log("Raaaandalf digga in rochi");
                return false;
        } 
        


        if (difZ is not 2*Scaling || difX is not 0) return false;//TODO check if this is the right way
        if (CurrentPosition.x > newPos.x){
            switch (GetOwnPlayer().GetColor()){
                case "Weiß" when Gm.GetPieces()[new Vector3(0, Scaling,0)].hasMoved:
                case "Schwarz" when Gm.GetPieces()[new Vector3(0,Scaling, 7*Scaling)].hasMoved:
                    return false;
            }
            for (var i = Scaling; i < 4*Scaling; i+= Scaling)
                if (Gm.GetPieces()[new Vector3(i,Scaling, newPos.z)] != null) return false;
        }
        else{
            switch (GetOwnPlayer().GetColor()){
                case "Weiß" when Gm.GetPieces()[new Vector3(7*Scaling, Scaling,0)].hasMoved:
                case "Schwarz" when Gm.GetPieces()[new Vector3(7*Scaling,Scaling, 7*Scaling)].hasMoved:
                    return false;
            }
            for (var i = 5*Scaling; i < 7*Scaling; i+=Scaling)
                if (Gm.GetPieces()[new Vector3(i,Scaling, newPos.z)] != null) return false;

        }

        return true;
    }

}