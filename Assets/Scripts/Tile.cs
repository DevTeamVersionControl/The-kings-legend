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

    [SerializeField] Material _materialMove;
    [SerializeField] Material _materialAttack;
    MeshRenderer _meshRenderer;

    float _pieceYOffset = .5f;

    public void Start()
    {
        if (_piece != null)
        {
            AddPiece(_piece);
        }
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void AddPiece(Piece piece)
    {
        _piece = piece;
        rigidbodyPiece = _piece.GetComponent<Rigidbody>();
        
        _piece.transform.position = transform.position + Vector3.up * _pieceYOffset;
        piece.SetFreeze(true);
        SetLocked(true);
        
    }

    [CanBeNull]
    public Piece RemovePiece()
    {
        var temp = _piece;
        _piece = null;
        return temp;
    }

    public void SetLocked(bool locked)
    {
        if (locked)
        {
            _locked = true;
        }
        else
        {
            _locked = false;
        }
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

    public void Highlight(bool move)
    {
        _highlighted = true;
        _meshRenderer.SetMaterials(new() { _meshRenderer.materials[0], move ? _materialMove : _materialAttack });
    }
    
    public void Unhighlight()
    {
        _highlighted = false;
        _meshRenderer.SetMaterials(new() { _meshRenderer.materials[0] });
    }

    public bool IsHighlighted()
    {
        return _highlighted;
    }

}
