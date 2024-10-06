using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] Board _board;

    [SerializeField] GameObject game;

    private Tile _currentlyDragging;
    
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
        _board.OnNextTurn(color);

        changeTurn?.Invoke();
    }

    public void OnPieceKilled()
    {

    }
    public void OnEndTurn()
    {
        
    }

    public void OnDragStart(Piece piece)
    {
        Tile tile = FindTile(piece);
        _currentlyDragging = tile;
        if (!tile.GetLocked())
        {
            _board.HighlightPieceMoves(tile);
        }
        Debug.Log("OnDragStart is called on the tile" + tile);
    }

    private Tile FindTile(Piece piece)
    {
        List<List<Tile>> lists = new() {_board.BoardTiles.ToList(), _board.SoldierTiles.ToList(),_board.UpgradeTiles.ToList()};
        foreach (var list in lists)
        {
            foreach (var tile in list)
            {
                Piece pieceToCheck = tile.GetPiece();
                if (tile.GetPiece() != null && pieceToCheck == piece)
                    return tile;
            }
        }
        return null;
    }
    
    public void OnDragEnd(Tile tile)
    {
        if (tile != null && !_currentlyDragging.GetLocked())
        {
            _board.OnPieceMoved(_currentlyDragging, tile);
            OnNextTurn(PlayerColor.GREEN);
        }
        else
        {
            _currentlyDragging.AddPiece(_currentlyDragging.GetPiece());
        }

        _board.UnhighlightAll();
        _currentlyDragging = null;
        Debug.Log("OnDragEnd is called on the tile" + tile.ToString());
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

        _board = board;
     
        foreach (Tile tile in board.SoldierTiles){
            var interaction = tile.GetPiece()?.GetComponent<MouseInteraction>();
            if (interaction != null)
            {
                interaction.StopMovePiece.AddListener(OnDragEnd);
                interaction.StartMovePiece.AddListener(OnDragStart);
            }
        }

        foreach (Tile tile in board.UpgradeTiles)
        {

            Debug.Log("getPiece: " + tile.GetPiece());
            Debug.Log("all: " + tile.GetPiece()?.GetComponent<MouseInteraction>());
           // tile.GetPiece()?.GetComponent<MouseInteraction>().MovePiece.AddListener(OnDrag);
        }


        //initialize starting tiles



        Debug.Log("in game Init" + _currentLevel);
        _playerColorTurn = PlayerColor.GREEN;
        OnNextTurn(_playerColorTurn);

    }

}
