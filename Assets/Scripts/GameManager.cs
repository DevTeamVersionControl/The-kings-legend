using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

   

    private PlayerColor _playerColorTurn;
    private enum _gameState { ADDUPGRADE, ATTACKMOVE };

    public enum GameLevel { MAINMENU, GAME };
    private enum _subState { DRAGGING, ATTACKING, NONE };

    private GameLevel _currentLevel;

    public static GameManager Instance;

    public void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    public void Update()
    {
    
    }


    public void ChangeLevel(GameLevel level)
    {
        _currentLevel = level;
        switch (level)
        {
            case GameLevel.MAINMENU:
                
                SceneManager.LoadScene("MainMenu");
                break;
            case GameLevel.GAME:
                SceneManager.LoadScene("MainGame");
                break;


            default:
                break;
        }
    }
    public void OnNextTurn(PlayerColor color)
    {

    }

    public void OnPieceKilled()
    {

    }
    public void OnEndTurn()
    {

    }

    public void OnDrag(Tile tile)
    {

    }

    public void OnClick(Tile tile)
    {

    }

    public void CancelAttack()
    {

    }
    public void PotentialAttack()
    {

    }


}
