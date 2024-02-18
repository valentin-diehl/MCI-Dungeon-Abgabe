
using System;
using System.Collections.Generic;
using System.Linq;

public class KiIvm {
        private string _pieceType, _color;
        private int _x, _y;
        private bool _hasMoved;
        
        public KiIvm(string pieceType, string color, int x, int y, bool hasmoved) {
                _pieceType = pieceType;
                _color = color;
                _x = x;
                _y = y;
                _hasMoved = hasmoved;
        }

        public void SetPieceType(string pt) {
                _pieceType = pt;
        }
        public void SetColor(string col) {
                _color = col;
        }

        public void SetX(int x) {
                _x = x;
        }

        public void SetY(int y) {
                _y = y;
        }

        public string GetPieceType() {
                return _pieceType;
        }
        public string GetColor() {
                return _color;
        }

        public int GetX() {
                return _x;
        }
        public int GetY() {
                return _y;
        }

        public void SetHasMoved(bool hm) {
                _hasMoved = hm;
        }

        public bool GetHasMoved() {
                return _hasMoved;
        }

        public KiIvm SearchForKiivm(int x, int y, List<KiIvm> pieces) {
                return pieces.FirstOrDefault(piece => piece.GetX() == x && piece.GetY() == y);
        }
        
        /*
        protected bool IsValidMove(int x, int y, KiIvm pt, List<KiIvm> pieces ){

                switch (pt.GetPieceType()) {
                        
                        case "King":
                                var difX =pt.GetX() - x; 
                                var difY =pt.GetY() - y; 

                                if(difY == 1 && difX == 0 || difX == 1 && difY == 0 || difY == 1 && difX == 1){
                                        pt.SetHasMoved(true); 
                                }
                                //return false;
                                
                                if (pt.GetHasMoved()) return false;
                                if (y != 0 && y != 7) return false;

                                if (difX is 2 && difY is 0){
                                        if (_x > x){
                                                switch (pt.GetColor()){
                                                        case "White" when Pieces[new Vector2(0, 0)].hasMoved:
                                                        case "Black" when Pieces[new Vector2(0, 7)].hasMoved:
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
                                break;
                        
                }
                
        }*/
        
}
