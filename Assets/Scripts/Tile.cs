using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Tile : MonoBehaviour
{


    [CanBeNull] Piece _piece;
    bool _locked;

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
        
    }
    
    public void Unhighlight()
    {
        
    }

}
