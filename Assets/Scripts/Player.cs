using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
public class Player {
    private readonly string _color;
    private List<ChessPiece> Pieces;

    public Player(string color, List<ChessPiece> pieces){
        _color = color;
        Pieces = pieces;

    }

    public List<ChessPiece> GetPiecesOfPlayer() {
        return Pieces;
    }

    public string GetColor() {
        return _color;
    }

    public bool RemainsKing() {
        return Pieces.Any(t => t.GetChessPieceValue() == 999);
    }
        
}