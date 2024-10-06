using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
    private Vector3 mouseOffset;

    private float mouseZCoordinate;

    private bool isDragging;

    private Vector3 mouseDownPosition;

    private float dragThreshold = 10f;

    [SerializeField] float _dragHeightOffset;

    public TileUnityEvent StopMovePiece;
    public PieceUnityEvent StartMovePiece;

    public void Start()
    {
        StartMovePiece = new PieceUnityEvent();
        StopMovePiece = new TileUnityEvent();
    }
    private void OnMouseDown()
    {
        mouseZCoordinate = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        mouseOffset = gameObject.transform.position - GetMouseWorldPos();

        //Debug.Log("Mouse is down");

        //Debug.Log("mouseOffset" + mouseOffset.ToString());

        isDragging = false;

        mouseDownPosition = Input.mousePosition;
        
        Piece piece = gameObject.GetComponent<Piece>();
        
        StartMovePiece.Invoke(piece);

    }
    

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = mouseZCoordinate;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
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
            //Debug.Log(hit.transform.name);
            //Debug.Log("outisde board");

            gameObject.transform.position = hit.point + hit.normal *_dragHeightOffset;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10000, Color.white);
           // Debug.Log("Did not Hit");
            transform.position = GetMouseWorldPos() + mouseOffset;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
        }

        }

        
    }

    private void OnMouseUp()
    {
        Debug.Log("start mouse up" + GameManager.Instance._currentlyDragging);
        Tile TileDrop = null;
        if (isDragging) {
            Debug.Log("mouse up is dragging" + GameManager.Instance._currentlyDragging);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int layerMask = 1 << LayerMask.NameToLayer("Board");     

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {

                Debug.Log("raycast is dragging" + GameManager.Instance._currentlyDragging);
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                //Debug.Log(hit.transform.name);
                //Debug.Log("hit");
                TileDrop = hit.transform.GetComponent<Tile>();

                //Debug.Log("Mouse Position" + dropZone.ToString());
            }
            else
            {
                Debug.Log("else is dragging" + GameManager.Instance._currentlyDragging);
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10000, Color.white);
               // Debug.Log("Did not Hit");
            }
            Debug.Log("just before" + GameManager.Instance._currentlyDragging);
            StopMovePiece.Invoke(TileDrop);
        } else

        {
            
        }

    }




}
