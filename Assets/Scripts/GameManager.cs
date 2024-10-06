using System;
using System.Collections.Generic;
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

    private Dictionary<PlayerColor, int> AvailableSoldiers = new();
    
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

        
    }

    public void OnPieceKilled()
    {

    }
    public void OnEndTurn()
    {
        changeTurn?.Invoke();
    }

    public void OnDragStart(Piece piece)
    {
        Tile tile = FindTile(piece);
        _currentlyDragging = tile;
        if (!tile.GetLocked())
        {
            _board.HighlightPieceMoves(tile);
            _board.SetLock(_board.AllTiles, piece.Color, true);
            tile.SetLocked(false);
        }

        Debug.Log("OnDragStart is called on the tile" + tile);
    }

    private Tile FindTile(Piece piece)
    {
        foreach (var tile in _board.AllTiles)
        {
            Piece pieceToCheck = tile.GetPiece();
            if (tile.GetPiece() != null && pieceToCheck == piece)
                return tile;
        }

        return null;
    }
    
    public void OnDragEnd(Tile tile)
    {
        if (tile != null && !_currentlyDragging.GetLocked())
        {
            _board.OnPieceMoved(_currentlyDragging, tile);
            _board.SetLock(_board.SoldierTiles, _playerColorTurn, --AvailableSoldiers[_playerColorTurn] <= 0);
            Debug.Log("Available soldiers " + AvailableSoldiers[_playerColorTurn]);
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
     
        foreach (Tile tile in board.AllTiles){
            var piece = tile.GetPiece();
            if (piece != null)
            {
                MouseInteraction interaction = piece.GetComponent<MouseInteraction>();
                interaction.StopMovePiece.AddListener(OnDragEnd);
                interaction.StartMovePiece.AddListener(OnDragStart);
                piece.StartingTile = tile;
            }
        }

        //initialize starting tiles
        Debug.Log("in game Init" + _currentLevel);
        _playerColorTurn = PlayerColor.GREEN;
        _board.OnNextTurn(_playerColorTurn);
        //AddStartingPieces(_playerColorTurn);
        
        // _playerColorTurn = PlayerColor.PURPLE;
        // OnNextTurn(_playerColorTurn);
        // AvailableSoldiers.Add(PlayerColor.GREEN, 3);

    }

    private Dictionary<PlayerColor, Vector2Int[]> startingTilesDict = new()
    {
        { PlayerColor.GREEN, new[] { new Vector2Int(2, 0), new Vector2Int(4, 0), new Vector2Int(6, 0) } },
        { PlayerColor.PURPLE, new[] { new Vector2Int(2, 2), new Vector2Int(4, 2), new Vector2Int(6, 2) } }
    };

    // private void AddStartingPieces(PlayerColor color)
    // {
    //     AvailableSoldiers.Add(color, 3);
    //     var startingTiles = startingTilesDict[color];
    //     for(int i = 0; i < startingTiles.Length; i++)
    //     {
    //         
    //     }
    // }

}
