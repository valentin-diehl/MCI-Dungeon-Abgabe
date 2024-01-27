using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessPiece : MonoBehaviour
{
    [SerializeField] protected Color _baseColor, _offsetColor;
    [SerializeField] protected MeshRenderer _renderer;
    void Start() {
        if (_renderer == null) {
            _renderer = GetComponent<MeshRenderer>();
        }
    }
    
    public void Init(bool isOffset) {
        if (_renderer != null) {
            _renderer.material.color = isOffset ? _offsetColor : _baseColor;
        }
    }
}