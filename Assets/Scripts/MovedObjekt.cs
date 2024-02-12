using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovedObjekt : MonoBehaviour
{
    private Vector2 oldPos; // über die x,y Werte kann man ja herausfinden um welche Spielfigur es sich handelt
    private Vector2 newPos; 
    private ChessPiece capturedPiece; 
    private bool hasCapturedFigure = false; 

    // Constructor
   public MovedObjekt(Vector2 oldPos, Vector2 newPos, bool hasCapturedFigure, ChessPiece capturedPiece = null)
    {
        this.oldPos = oldPos;
        this.newPos = newPos;
    
        if (hasCapturedFigure)
        {
            this.capturedPiece = capturedPiece;
        }
        else
        {
            this.capturedPiece = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
