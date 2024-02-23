using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMove : MonoBehaviour
{
    private Vector3 _oldPos; // über die x,y Werte kann man ja herausfinden um welche Spielfigur es sich handelt
    private Vector3 _newPos; 
    private ChessPiece _capturedPiece, _self; 
    private bool _hasCapturedFigure;
    private string _specialMove;

    // Constructor
   public LogMove(ChessPiece self,Vector3 oldPos, Vector3 newPos, bool hasCapturedFigure, ChessPiece capturedPiece,string specialMove) {
        _self = self;
        _oldPos = oldPos;
        _newPos = newPos;
        _capturedPiece = hasCapturedFigure ? capturedPiece : null;
        _hasCapturedFigure = hasCapturedFigure;
        _specialMove = specialMove;
    }

    public string GetSpecialMove() {
        return _specialMove;
    }

    public Vector3 GetOldPos() {
        return _oldPos;
    }

    public Vector3 GetNewPos() {
        return _newPos;
    }

    public ChessPiece GetCapturedPiece() {
        return _capturedPiece;
    }

    public ChessPiece GetSelf() {
        return _self;
    }


}