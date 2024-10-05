using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Tile : MonoBehaviour
{


    [CanBeNull] [SerializeField] Piece _piece;
    private bool _locked;
    private bool _highlighted;

    public void AddPiece(Piece piece)
    {
        _piece = piece;
    }

    [CanBeNull]
    public Piece RemovePiece()
    {
        return _piece;
    }

    public void Unlock()
    {
        _locked = false;
    }

    public void Lock()
    {
        _locked = true;
    }

    public bool GetLocked()
    {
        return _locked;
    }

    [CanBeNull]
    public Piece GetPiece()
    {
        return _piece;
    }

    public void HidePiece()
    {
        
    }

    public void UnhidePiece()
    {

    }

    public void Highlight()
    {
        _highlighted = true;
    }
    
    public void Unhighlight()
    {
        _highlighted = false;
    }

    public bool IsHighlighted()
    {
        return _highlighted;
    }

}
