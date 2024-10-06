using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject Camera1;
    [SerializeField] GameObject Camera2;
    public void Awake()
    {
        GameManager.changeTurn += changeCamera;
    }

    private void OnDestroy()
    {
        GameManager.changeTurn -= changeCamera;
    }

    void changeCamera()
    {
        if (Camera1.activeInHierarchy)
        {
            Camera1.SetActive(false);
            Camera2.SetActive(true);
        }
        else
        {
            Camera1.SetActive(true);
            Camera2.SetActive(true);
        }
    }


}
