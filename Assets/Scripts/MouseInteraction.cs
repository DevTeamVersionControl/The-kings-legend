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

    [SerializeField] GameObject _piece;

    [SerializeField] float _dragHeightOffset;

    private void OnMouseDown()
    {
        mouseZCoordinate = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        mouseOffset = gameObject.transform.position - GetMouseWorldPos();

        //Debug.Log("Mouse is down");

        //Debug.Log("mouseOffset" + mouseOffset.ToString());

        isDragging = false;

        mouseDownPosition = Input.mousePosition;

    }
    

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = mouseZCoordinate;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {

        if (Vector3.Distance(mouseDownPosition, Input.mousePosition) > dragThreshold)
        {
            isDragging = true;

           // Debug.Log("dragging");
        }

        if(isDragging) { 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int layerMask = 1 << LayerMask.NameToLayer("Board");
        int layerOutsideBoard = 1 << LayerMask.NameToLayer("OutsideBoard");

 

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerOutsideBoard))   
        {
            Debug.DrawLine(Camera.main.transform.position, hit.point);
            //Debug.Log(hit.transform.name);
            //Debug.Log("outisde board");

            gameObject.transform.position = hit.point + hit.normal *_dragHeightOffset;
        }
        else
        {
            Debug.DrawLine(Camera.main.transform.position, hit.transform.position);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10000, Color.white);
           // Debug.Log("Did not Hit");
            transform.position = GetMouseWorldPos() + mouseOffset;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
        }

        }

        
    }

    private void OnMouseUp()
    {


        if (isDragging) { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int layerMask = 1 << LayerMask.NameToLayer("Board");     

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                //Debug.Log(hit.transform.name);
                //Debug.Log("hit");

                Vector3 dropZone = hit.transform.position;
                //Debug.Log("Mouse Position" + dropZone.ToString());
                Instantiate(_piece, dropZone, Quaternion.identity);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10000, Color.white);
               // Debug.Log("Did not Hit");
            }    

        }

        else
        {
           // Debug.Log("this is a click");
        }

    }




}