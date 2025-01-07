using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject Camera1;
    [SerializeField] GameObject Camera2;
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
        CameraMenu.SetActive(true);
        Camera1.SetActive(false);
        Camera2.SetActive(false);

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

    void initialPosition()
    {
        Camera1.SetActive(true);
        Camera2.SetActive(false);
    }

    void PurplePosition() {
        Camera1.SetActive(true);
        Camera2.SetActive(false);
    }

    void GreenPosition()
    {
        Camera1.SetActive(false);
        Camera2.SetActive(true);
    }

}
