using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Player {

    string color;
    protected Dictionary<Vector2, ChessPiece> Pieces;

    public Player(string color, Dictionary<Vector2, ChessPiece> _pieces){
        this.color = color;
        this.Pieces = _pieces;

    }

    public Dictionary<Vector2, ChessPiece> getPiecesofPlayer() {
        return Pieces;
    }

    public bool remainsKing(){
        return Pieces.Any(fig => Pieces[fig.Key].Equals("King"));
    }
        
}