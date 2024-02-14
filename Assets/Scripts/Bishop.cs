using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece {
    
    public Bishop() {
        ChessPieceValue = 3;
    }


    // protected void Start(){
    //     transform.position += new Vector3(0,5,0); 
    // }


    protected override bool IsValidMove(Vector2 newPos) {
        if (!DiagonalMove(newPos)) return false; 

        hasMoved = true; 
        return true;
    }
}