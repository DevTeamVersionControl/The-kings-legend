using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject CameraPurple;
    [SerializeField] GameObject CameraGreen;
    [SerializeField] GameObject CameraMenu;
    public void Awake()
    {
        GameManager.changeTurn += changeCamera;
        GameManager.mainMenu += InMenu;
    }

    private void OnDestroy()
    {
        GameManager.changeTurn -= changeCamera;
        GameManager.mainMenu -= InMenu;
    }

    void InMenu()
    {
        
        CameraPurple.SetActive(false);
        CameraGreen.SetActive(false);
        CameraMenu.SetActive(true);
    }

    void changeCamera()
    {
        if (GameManager.Instance._playerColorTurn == PlayerColor.GREEN)
        {         

               GreenPosition();
        }
        else
        {

                PurplePosition();
        }
    }

    void PurplePosition() {
        CameraPurple.SetActive(true);
        CameraGreen.SetActive(false);
    }

    void GreenPosition()
    {
        CameraGreen.SetActive(true);
        CameraPurple.SetActive(false);
    }

}
