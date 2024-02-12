using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private MeshRenderer _renderer;
//test
    void Start() {
        if (_renderer == null) {
            // Falls _renderer im Inspector nicht zugewiesen wurde, versuche es automatisch zu finden
            _renderer = GetComponent<MeshRenderer>();
        }
    }

    public void Init(bool isOffset) {
        if (_renderer != null) {
            _renderer.material.color = isOffset ? _offsetColor : _baseColor;
        }
    }
    
    public void MoveTo(Tile targetTile)
    {
        // Hier kannst du die Logik für die Überprüfung des Zugs implementieren.
        // Zum Beispiel, ob der Zug gültig ist usw.
        // Wenn der Zug gültig ist, verschiebe die Schachfigur an die neue Position.
        if (IsValidMove(targetTile))
        {
            transform.position = targetTile.transform.position;
        }
    }

    private bool IsValidMove(Tile targetTile)
    {
        // Hier kannst du die Logik für die Überprüfung des Zugs implementieren.
        // Zum Beispiel, ob die Zieltile gültig ist, ob ein anderer Spielstein dort steht, usw.
        // Rückgabe von true, wenn der Zug gültig ist, ansonsten false.
        return true; // Hier musst du die Logik entsprechend deiner Anforderungen anpassen.
    }

}