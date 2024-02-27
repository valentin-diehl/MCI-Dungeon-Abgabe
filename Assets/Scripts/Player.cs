using System.Collections.Generic;
using System.Linq;
public class Player {
    private readonly string _color;
    private readonly List<ChessPiece> _pieces;

    public Player(string color, List<ChessPiece> pieces){
        _color = color;
        _pieces = pieces;
    }

    public void DeletePiecesFromList(ChessPiece piece) {
        _pieces.Remove(piece);
    }

    public List<ChessPiece> GetPiecesOfPlayer() {
        return _pieces;
    }

    public string GetColor() {
        return _color;
    }

    public bool RemainsKing() {
        return _pieces.Any(t => t.GetChessPieceValue() == 999);
    }
        
}