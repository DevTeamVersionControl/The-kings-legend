using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{



    private PlayerColor _playerColorTurn;
    private enum _gameState { ADDUPGRADE, ATTACKMOVE };


    private enum _subState { DRAGGING, ATTACKING, NONE };

    public enum GameLevel { MAINMENU, GAME };

    private GameLevel _currentLevel;

    private _gameState _currentGameState;

    private _subState _subGameState;

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


    public void Start()
    {

            
    }
    // Update is called once per frames
    public void Update()
    {
    
    }


    public void ChangeLevel(GameLevel level)
    {
        _currentLevel = level;
        Debug.Log("changeLevel :" + _currentLevel.ToString());
        switch (level)
        {
            case GameLevel.MAINMENU:
                
                SceneManager.LoadScene("MainMenu");
                break;
            case GameLevel.GAME:
                SceneManager.LoadScene("MainGame");
                GameInit();

                break;
            default:
                break;
        }
    }
    public void OnNextTurn(PlayerColor color)
    {
        Debug.Log("Player turn : " + _playerColorTurn.ToString());




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

    private void GameInit()
    {
        Debug.Log("in game Init" + _currentLevel);
        _playerColorTurn = PlayerColor.PURPLE;
        OnNextTurn(_playerColorTurn);

    }

}
