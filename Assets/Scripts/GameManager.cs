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

    [SerializeField] GameObject _winScreen;

    [SerializeField] GameObject _skipButton;

    [SerializeField] Board _board;

    [SerializeField] GameObject game;

    public Tile _currentlyDragging;

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

        changeTurn?.Invoke();
        Debug.Log("Player turn : " + _playerColorTurn.ToString());
        _board.OnNextTurn(color);


        _currentGameState = _gameState.ADDUPGRADE;

        
    }

    public void OnPieceKilled()
    {
        if (!_board.HasPiece(PlayerColorExtensions.GetOpposite(_playerColorTurn)))
        {
            OnWin(_playerColorTurn);
        }
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

            if (_board.UpgradeTiles.Contains(_currentlyDragging)){
                _currentGameState = _gameState.ATTACKMOVE;
                Debug.Log("Upgraded");
            }
            if (_board.SoldierTiles.Contains(_currentlyDragging))
            {
                _currentGameState = _gameState.ATTACKMOVE;
                Debug.Log("added a soldier");
            }
            else
            {
                _board.SetLock(_board.SoldierTiles, _playerColorTurn, --AvailableSoldiers[_playerColorTurn] <= 0);
                Debug.Log("Available soldiers " + AvailableSoldiers[_playerColorTurn]);
                //a changer
                if(AvailableSoldiers[_playerColorTurn] <= 0)
                {
                    OnNextTurn(PlayerColorExtensions.GetOpposite(_playerColorTurn));
                }
            }
        }
        else
        {
            _currentlyDragging.AddPiece(_currentlyDragging.GetPiece());
            
        }

        _board.UnhighlightAll();

        
        //Debug.Log("OnDragEnd is called on the tile" + tile.ToString());
    }

    public void OnClick(Piece piece)
    {
        Debug.Log("in Onclick");
        Tile tile = FindTile(piece);
        if (_currentGameState == _gameState.ATTACKMOVE && _board.BoardTiles.Contains(tile))
        {
            
            _board.HighlightPieceAttack(tile);
            Debug.Log("this is the start of an attack");
        }
        else
        {
            Debug.Log("cant attack");
        }
    }

    public void CancelAttack()
    {

    }
    public void PotentialAttack(Tile tile)
    {
        _board.OnPieceAttack(tile, null);
    }

    public void GameInit(Board board)
    {

        _board = board;
     
        foreach(Tile tile in board.BoardTiles) 
        {
            tile.GetComponent<MouseInteraction>().EndAttack.AddListener(PotentialAttack);
        }
        foreach (Tile tile in board.AllTiles){
            var piece = tile.GetPiece();       
            if (piece != null)
            {
                MouseInteraction interaction = piece.GetComponent<MouseInteraction>();
                interaction.StopMovePiece.AddListener(OnDragEnd);
                interaction.StartMovePiece.AddListener(OnDragStart);
                interaction.StartAttack.AddListener(OnClick);

                piece.StartingTile = tile;
            }
        }

        //initialize starting tiles
        Debug.Log("in game Init" + _currentLevel);
        _playerColorTurn = PlayerColor.GREEN;
        OnNextTurn(_playerColorTurn);

        //AddStartingPieces(_playerColorTurn);

        // _playerColorTurn = PlayerColor.PURPLE;
        // OnNextTurn(_playerColorTurn);
        // AvailableSoldiers.Add(PlayerColor.GREEN, 3);

        AvailableSoldiers.Add(PlayerColor.GREEN, 3);
        AvailableSoldiers.Add(PlayerColor.PURPLE, 3);

        _skipButton.SetActive(true);
    }

    private Dictionary<PlayerColor, Vector2Int[]> startingTilesDict = new()
    {
        { PlayerColor.GREEN, new[] { new Vector2Int(2, 0), new Vector2Int(4, 0), new Vector2Int(6, 0) } },
        { PlayerColor.PURPLE, new[] { new Vector2Int(2, 2), new Vector2Int(4, 2), new Vector2Int(6, 2) } }
    };

    private void OnWin(PlayerColor playerColor)
    {
        _skipButton.SetActive(false);
        Debug.Log(playerColor + "has won!");
        _winScreen.SetActive(true);
    }

    public void OnSkip()
    {
        if(_currentGameState == _gameState.ADDUPGRADE)
        {
            _currentGameState = _gameState.ATTACKMOVE;
        }
        else if(_currentGameState == _gameState.ATTACKMOVE)
        {
            OnNextTurn(PlayerColorExtensions.GetOpposite(_playerColorTurn));
        }
    }

    public void PlayeAgain()
    {
        ChangeLevel(GameManager.GameLevel.GAME);
        

    }
    
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
