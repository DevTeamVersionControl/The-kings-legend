using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 mouseOffset;

    private float mouseZCoordinate;

    private void OnMouseDown()
    {
        mouseZCoordinate = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        mouseOffset = gameObject.transform.position - GetMouseWorldPos();

        Debug.Log("Mouse is down");

        Debug.Log("mouseOffset" + mouseOffset.ToString());

    }


    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = mouseZCoordinate;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + mouseOffset;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
    }

    private void OnMouseUp()
    {
        gameObject.GetComponent<Rigidbody>().useGravity = true;
    }


}
