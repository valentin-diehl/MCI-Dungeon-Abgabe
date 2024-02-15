using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

public class Bishop : ChessPiece {
    
    private XRGrabInteractable grabInteractable;
    
    public Bishop() {
        ChessPieceValue = 3;
    }

    // private void Start()
    // {
    //     grabInteractable = GetComponent<XRGrabInteractable>();

    //     if (grabInteractable != null)
    //     {
    //         grabInteractable.onSelectExited.AddListener(OnSelectExitHandler);
    //     }
    // }

    // private void OnSelectExitHandler(XRBaseInteractor interactor)
    // {
    //     if (interactor != null)
    //     {   // Pos beim Loslassen
    //         Vector3 lastPosition = interactor.transform.position;
    //         Move(lastPosition);
    //     }
    // }

    protected override bool IsValidMove(Vector2 newPos) {
        if (!DiagonalMove(newPos)) return false; 

        hasMoved = true; 
        return true;
    }
}