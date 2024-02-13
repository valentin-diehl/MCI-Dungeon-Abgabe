using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMove : MonoBehaviour
{
    private Vector2 _oldPos; // über die x,y Werte kann man ja herausfinden um welche Spielfigur es sich handelt
    private Vector2 _newPos; 
    private ChessPiece _capturedPiece; 
    private bool _hasCapturedFigure;
    private string _specialMove;

    // Constructor
   public LogMove(Vector2 oldPos, Vector2 newPos, bool hasCapturedFigure, ChessPiece capturedPiece,string specialMove)
    {
        _oldPos = oldPos;
        _newPos = newPos;
        _capturedPiece = hasCapturedFigure ? capturedPiece : null;
        _hasCapturedFigure = hasCapturedFigure;
        _specialMove = specialMove;
    }

    public Vector2 GetOldPos() {
        return _oldPos;
    }

    public Vector2 GetNewPos() {
        return _newPos;
    }

  
}