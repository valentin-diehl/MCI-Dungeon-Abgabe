using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Player {
    private readonly string _color;
    private Dictionary<Vector2, ChessPiece> Pieces;

    public Player(string color, Dictionary<Vector2, ChessPiece> pieces){
        this._color = color;
        this.Pieces = pieces;

    }

    public Dictionary<Vector2, ChessPiece> GetPiecesOfPlayer() {
        return Pieces;
    }

    public string GetColor() {
        return _color;
    }

    public bool RemainsKing(){
        return Pieces.Any(fig => Pieces[fig.Key].Equals("King"));
    }
        
}