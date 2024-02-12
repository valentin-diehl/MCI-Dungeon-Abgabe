using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using UnityEngine;
using System; 
public class King : ChessPiece {
    
    public King() {
        ChessPieceValue = 999;
    }

    protected override bool IsValidMove(Vector2 newPos){
        var difX = (int)Math.Abs(x - newPos.x); 
        var difY = (int)Math.Abs(y - newPos.y); 

        if(difY == 1 && difX == 0 || difX == 1 && difY == 0 || difY == 1 && difX == 1){
            hasMoved = true; 
        }
        return false;
    }
    
    private bool IsValidRochade(Vector2 newPos){

        if (hasMoved) return false;
        if (newPos.y != 0 && (int)newPos.y != 7) return false;

        var difX = Math.Abs(x - newPos.x);
        var difY = Math.Abs(y - newPos.y);
        if (difX is 2 && difY is 0){
            if (x > newPos.x){
                switch (Player.GetColor()){
                    case "Weiß" when Pieces[new Vector2(0, 0)].hasMoved:
                    case "Schwarz" when Pieces[new Vector2(0, 7)].hasMoved:
                        return false;
                }
                for (var i = 1; i < 4; i++)
                    if (Pieces[new Vector2(i, newPos.y)] != null) return false;
            }
            else{
                switch (Player.GetColor()){
                    case "Weiß" when Pieces[new Vector2(7, 0)].hasMoved:
                    case "Schwarz" when Pieces[new Vector2(7, 7)].hasMoved:
                        return false;
                }
                for (var i = 5; i < 7; i++)
                    if (Pieces[new Vector2(i, newPos.y)] != null) return false;

            }
        }

        return true;
    }

}