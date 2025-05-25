using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject CameraPurple;
    [SerializeField] GameObject CameraGreen;
    [SerializeField] GameObject CameraMenu;
    [SerializeField] GameObject CameraTransition;
    bool _onMenu = true;
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
            
            if (_onMenu)
            {
                _onMenu = false;
                StartCoroutine(BookToGreenPosition());
                
            }
            else
            {
               GreenPosition();
            }
        }
        else
        {
            
            if (_onMenu)
            {
                _onMenu = false;
                StartCoroutine(BookToPurplePosition());
                
            }
            else 
            {
                PurplePosition();
                
            }
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

    IEnumerator BookToGreenPosition()
    {
        CameraMenu.SetActive(false);
        CameraTransition.SetActive(true);
        CameraPurple.SetActive(false);
        yield return new WaitForSeconds(2);
        CameraTransition.SetActive(false);
        CameraGreen.SetActive(true);
    }
    IEnumerator BookToPurplePosition()
    {
        CameraMenu.SetActive(false);
        CameraTransition.SetActive(true);
        CameraGreen.SetActive(false);
        yield return new WaitForSeconds(2);
        CameraTransition.SetActive(false);
        CameraPurple.SetActive(true);
    }

}
