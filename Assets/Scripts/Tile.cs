using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Tile : MonoBehaviour
{



    [CanBeNull] [SerializeField] Piece _piece;
    private Rigidbody rigidbodyPiece;
    private bool _locked;
    private bool _highlighted;


    [SerializeField] float _pieceYOffset = 1.5f;

    public void Start()
    {
        if (_piece != null)
        {
            AddPiece(_piece);
        }
        
    }

    public void AddPiece(Piece piece)
    {
        _piece = piece;
        rigidbodyPiece = _piece.GetComponent<Rigidbody>();
        
        _piece.transform.position = transform.position + Vector3.up * _pieceYOffset;
        Lock ();
        
    }

    [CanBeNull]
    public Piece RemovePiece()
    {
        return _piece;
    }

    public void Unlock()
    {
        _locked = false;
        rigidbodyPiece.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void Lock()
    {
        _locked = true;
        rigidbodyPiece.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

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
        _piece?.gameObject.SetActive(false);
    }

    public void UnhidePiece()
    {
        _piece?.gameObject.SetActive(true);
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
