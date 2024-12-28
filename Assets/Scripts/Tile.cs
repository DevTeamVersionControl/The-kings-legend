using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [CanBeNull] [SerializeField] Piece _piece;
    private Rigidbody rigidbodyPiece;
    private bool _locked;
    private HighlightType _highlight = HighlightType.NONE;

    public enum HighlightType
    {
        NONE,
        MOVE,
        ATTACK
    }

    private Dictionary<HighlightType, List<Material>> _highlightMaterials = null;

    [SerializeField] Material _materialMove;
    [SerializeField] Material _materialAttack;
    MeshRenderer _meshRenderer;

    public float _pieceYOffset = .5f;



    public void Awake()
    {
        GameManager.loadGame += TileStart;
    }

    public void OnDestroy()
    {
        GameManager.loadGame -= TileStart;
    }
    public void TileStart()
    {
        if (_piece != null)
        {
            AddPiece(_piece);
        }
        _meshRenderer = GetComponent<MeshRenderer>();
        _highlightMaterials = new()
        {
            {HighlightType.NONE, new(){_meshRenderer.materials[0]}},
            {HighlightType.MOVE, new(){_meshRenderer.materials[0], _materialMove}},
            {HighlightType.ATTACK, new(){_meshRenderer.materials[0], _materialAttack}}
        };
    }

    public void AddPiece(Piece piece)
    {
        _piece = piece;

        //rigidbodyPiece = _piece.GetComponent<Rigidbody>();
        
        _piece.transform.position = transform.position + Vector3.up * _pieceYOffset;

        if (piece.Color == PlayerColor.GREEN){
            _piece.transform.rotation = Quaternion.LookRotation(new Vector3(-1, 0, UnityEngine.Random.Range(-0.5f, 0.5f)));
        } else {
            _piece.transform.rotation = Quaternion.LookRotation(new Vector3(1, 0, UnityEngine.Random.Range(-0.5f, 0.5f)));
        }
        piece.SetFreeze(true);
        SetLocked(true);
        
    }

    public void AddPieceAndMoveBack(Piece piece)
    {
        _piece = piece;
        //rigidbodyPiece = _piece.GetComponent<Rigidbody>();
        float distance = Vector3.Distance(transform.position, piece.transform.position);
        float speed = 100;
        float lerpDuration = distance/speed;

        StartCoroutine(LerpToTile(lerpDuration, piece));
        
        if (piece.Color == PlayerColor.GREEN)
        {
            _piece.transform.rotation = Quaternion.LookRotation(new Vector3(-1, 0, UnityEngine.Random.Range(-0.5f, 0.5f)));
        }
        else
        {
            _piece.transform.rotation = Quaternion.LookRotation(new Vector3(1, 0, UnityEngine.Random.Range(-0.5f, 0.5f)));
        }
        piece.SetFreeze(true);
        SetLocked(true);

    }

    private IEnumerator LerpToTile(float lerpDuration, Piece piece)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = piece.transform.position;

        while (elapsedTime < lerpDuration)
        {
            // Calculate the position at the current lerp factor
            piece.transform.position = Vector3.Lerp(startPosition, transform.position + Vector3.up * _pieceYOffset, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the piece is exactly at the original position
        piece.transform.position = transform.position + Vector3.up * _pieceYOffset;
    
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

    public void Highlight(HighlightType type)
    {
        _highlight = type;
        _meshRenderer.SetMaterials(_highlightMaterials[type]);
    }

    public HighlightType GetHighlight()
    {
        return _highlight;
    }

}
