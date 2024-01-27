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

}