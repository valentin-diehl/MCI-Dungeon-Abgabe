using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  
    private int _fieldSize = 8;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _cam;
    [SerializeField] private ChessPiece _rookPrefab;
    [SerializeField] private ChessPiece _bishopPrefab;
    [SerializeField] private ChessPiece _kingPrefab;
    [SerializeField] private ChessPiece _queenPrefab;
    [SerializeField] private ChessPiece _pawnPrefab;
    [SerializeField] private ChessPiece _knightPrefab;

    private Dictionary<Vector2, Tile> _tiles;
    private Dictionary<Vector2, ChessPiece> _pieces;
    private ChessPiece _selectedPiece;

    void Start()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        _pieces = new Dictionary<Vector2, ChessPiece>();

        GenerateGrid();
        GeneratePieces();
    }

    void GenerateGrid()
    {
        for (var x = 0; x < _fieldSize; x++)
        {
            for (var y = 0; y < _fieldSize; y++)
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                var isEvenRow = y % 2 == 0;
                var isEvenColumn = x % 2 == 0;
                var isOffset = isEvenRow ^ isEvenColumn;

                spawnedTile.Init(isOffset);
                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }
    }

    void GeneratePieces()
    {
        for (var x = 0; x < _fieldSize; x++)
        {
            for (var y = 0; y < _fieldSize; y++)
            {
                var erg = (x == 0);
                switch (x)
                {
                    case 0:
                    case 7:
                        switch (y)
                        {
                            case 0:
                            case 7:
                                SpawnPiece(_rookPrefab, x, y, !erg);
                                break;
                            case 1:
                            case 6:
                                SpawnPiece(_knightPrefab, x, y, !erg);
                                break;
                            case 2:
                            case 5:
                                SpawnPiece(_bishopPrefab, x, y, !erg);
                                break;
                            case 3:
                                SpawnPiece(erg ? _queenPrefab : _kingPrefab, x, y, !erg);
                                break;
                            case 4:
                                SpawnPiece(erg ? _kingPrefab : _queenPrefab, x, y, !erg);
                                break;
                        }
                        break;
                    case 1:
                    case 6:
                        var erg2 = (x == 1);
                        SpawnPiece(_pawnPrefab, x, y, !erg2);
                        break;
                }
            }
        }
    }

    void SpawnPiece(ChessPiece prefab, int x, int y, bool isOffset) {
        var piece = Instantiate(prefab, new Vector3(x, 0.5f, y), Quaternion.identity);

        // Ändere die Rotation des Prefabs basierend auf der Ausrichtung
        if ((x == 0 || x== 7) && (y==1 || y == 6)) {
            var roti = 270f;
            if (x == 0) roti = 90f;
            piece.transform.Rotate(Vector3.up, roti);
        }

        piece.name = $"{prefab.name} {x} {y}";
        piece.Init(isOffset);
        _pieces[new Vector2(x, y)] = piece;
    }


    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }
}