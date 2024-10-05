using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Tile : MonoBehaviour
{


    [CanBeNull] Piece _piece;
    bool _locked;

    public void AddPiece()
    {
        
    }

    public void RemovePiece()
    {

    }

    public void Unlock()
    {

    }

    public void Lock()
    {

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

}
