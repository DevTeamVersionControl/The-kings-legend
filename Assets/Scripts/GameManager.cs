using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField] Board boardRef;

    [SerializeField] GameObject game;

    
    public static event Action changeTurn;


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
                

                break;
            default:
                break;
        }
    }
    public void OnNextTurn(PlayerColor color)
    {
        Debug.Log("Player turn : " + _playerColorTurn.ToString());


        changeTurn?.Invoke();

    }

    public void OnPieceKilled()
    {

    }
    public void OnEndTurn()
    {
        
    }

    public void OnDrag(Tile tile)
    {
        Debug.Log("OnDrag is called on the tile" + tile.ToString());
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

    public void GameInit(Board board)
    {

        boardRef = board;
     
        foreach (Tile tile in board.SoldierTiles){

            
            Debug.Log("getPiece: "+ tile.GetPiece());
            Debug.Log("all: " + tile.GetPiece()?.GetComponent<MouseInteraction>());
            tile.GetPiece()?.GetComponent<MouseInteraction>().MovePiece.AddListener(OnDrag);

        }

        foreach (Tile tile in board.UpgradeTiles)
        {

            Debug.Log("getPiece: " + tile.GetPiece());
            Debug.Log("all: " + tile.GetPiece()?.GetComponent<MouseInteraction>());
           // tile.GetPiece()?.GetComponent<MouseInteraction>().MovePiece.AddListener(OnDrag);
        }


        //initialize starting tiles



        Debug.Log("in game Init" + _currentLevel);
        _playerColorTurn = PlayerColor.PURPLE;
        OnNextTurn(_playerColorTurn);

    }

}
