using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{


    [SerializeField] GameObject book;
    void Start()
    {

        GameManager.pause += UpdateMouseInteractMainMenu;
        GameManager.loadGame += UpdateMouseInteractLoadGame;

    }


    private void OnDestroy()
    {
        GameManager.mainMenu -= UpdateMouseInteractMainMenu;
        GameManager.loadGame -= UpdateMouseInteractLoadGame;
    }

    void UpdateMouseInteractMainMenu()
    {
        
        book.SetActive(false);
    }

    void UpdateMouseInteractLoadGame()
    {
        book.SetActive(true);
    }

    public void Pause()
    {
        GameManager.Instance.ChangeLevel(GameManager.GameLevel.PAUSE);
    }
}
