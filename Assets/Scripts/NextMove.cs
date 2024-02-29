using UnityEngine;

public class NextMove {
    private readonly Vector3 _targetPos;
    private readonly int _capturedValue;
    private readonly ChessPiece _myPiece;
    
    public NextMove(Vector3 position, int capturedValue, ChessPiece myPiece){
        _targetPos = position;
        _capturedValue = capturedValue;
        _myPiece = myPiece;
    }

    public Vector3 TargetGetPosition() {
        return _targetPos;
    }

    public ChessPiece GetMyPiece() {
        return _myPiece;
    }

    public int GetCapturedValue() {
        return _capturedValue;
    }


}