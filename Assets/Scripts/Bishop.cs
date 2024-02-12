﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece {
    
    public Bishop() {
        chessPieceValue = 3;
    }
    public bool isValidMove(Vector2 newPos) {
        if (!base.diagonalMove(newPos)) return false; 

        // base.move(newPos);
        base.hasMoved = true; 
        return true;
    }
}