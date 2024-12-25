using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    [SerializeField] GameObject _tuto;


    public void OnStartGame()
    {
        GameManager.Instance.ChangeLevel(GameManager.GameLevel.GAME);


    }

    public void OnOpenTuto()
    {
        _tuto.SetActive(true);
    }
}
