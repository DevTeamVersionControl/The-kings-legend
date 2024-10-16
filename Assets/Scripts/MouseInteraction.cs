using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
    private Vector3 mouseOffset;

    private float mouseZCoordinate;

    private bool isDragging;

    private Vector3 mouseDownPosition;

    private float dragThreshold = 10f;

    [SerializeField] bool isTile;

    [SerializeField] float _dragHeightOffset;

    [SerializeField] AudioSource audioPickUp;
    [SerializeField] AudioSource audioDrop;
    public AudioClip[] soundPickUp;
    public AudioClip[] soundDrop;

    public TileUnityEvent StopMovePiece;
    public PieceUnityEvent StartMovePiece;
    public PieceUnityEvent StartAttack;
    public TileUnityEvent EndAttack;

    public void Start()
    {
        StartMovePiece = new PieceUnityEvent();
        StopMovePiece = new TileUnityEvent();
        StartAttack = new PieceUnityEvent();
        EndAttack = new TileUnityEvent();
    }
    private void OnMouseDown()
    {
        if (isTile)
        {
            Tile tile = GetComponent<Tile>();
            if (tile.IsHighlighted()) {
                EndAttack.Invoke(tile);
            }
        }
        else
        {
            mouseZCoordinate = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

            mouseOffset = gameObject.transform.position - GetMouseWorldPos();

            isDragging = false;

            mouseDownPosition = Input.mousePosition;

            Piece piece = gameObject.GetComponent<Piece>();

            AudioClip randomClip = soundPickUp[Random.Range(0, soundPickUp.Length)];
            audioPickUp.PlayOneShot(randomClip);

            StartMovePiece.Invoke(piece);
        }

    }
    

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = mouseZCoordinate;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {

        if (isTile)
        {
            return;
        }
        int layerMask = 1 << LayerMask.NameToLayer("Board");
        int layerOutsideBoard = 1 << LayerMask.NameToLayer("OutsideBoard");

        if (!isDragging && Vector3.Distance(mouseDownPosition, Input.mousePosition) > dragThreshold)
        {
            isDragging = true;
        }

        if(isDragging) { 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerOutsideBoard))   
        {
            Debug.DrawLine(Camera.main.transform.position, hit.point);

            gameObject.transform.position = hit.point + hit.normal *_dragHeightOffset;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10000, Color.white);
            transform.position = GetMouseWorldPos() + mouseOffset;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
        }

        }

        
    }

    private void OnMouseUp()
    {
        if (isTile)
        {
            return;
        }
        Tile TileDrop = null;
        if (isDragging) {

            AudioClip randomClip = soundDrop[Random.Range(0, soundDrop.Length)];
            audioDrop.PlayOneShot(randomClip);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int layerMask = 1 << LayerMask.NameToLayer("Board");     

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                TileDrop = hit.transform.GetComponent<Tile>();
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10000, Color.white);
            }
            StopMovePiece.Invoke(TileDrop);
        } else
        {
            StartAttack.Invoke(GetComponent<Piece>());
        }
            

    }


    

}
