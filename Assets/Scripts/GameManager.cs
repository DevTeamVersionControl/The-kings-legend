using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private PlayerColor _playerColorTurn;

    private enum subState { DRAGGING, ATTACKING, NONE };

    public enum GameLevel { MAINMENU, GAME };

    private GameLevel _currentLevel;

    private subState _subState;

    [SerializeField] GameObject _winScreen;

    [SerializeField] GameObject _skipButton;

    [SerializeField] Board _board;

    [SerializeField] GameObject game;

    public Tile _currentlyDragging;
    
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
        _playerColorTurn = color;
        changeTurn?.Invoke();
        Debug.Log("Player turn : " + _playerColorTurn.ToString());
        if (_board.OnNextTurn(color) < 3){
            Debug.Log($"Unlocked {color}'s soldiers since they have less than three pieces");
            _board.SetLock(_board.SoldierTiles, color, false);
        }
        
    }

    public void OnDragStart(Piece piece)
    {
        Tile tile = FindTile(piece);
        _currentlyDragging = tile;

        if(_subState == subState.ATTACKING)
        {
            PotentialAttack(tile);
            return;
        }
       
        if (!tile.GetLocked())
        {
            tile.SetLocked(false);

            if (_board.UpgradeTiles.Contains(tile) || _board.LegendTiles.Contains(tile))
            {
                _board.HighlightPieceUpgrade(tile);
            }
            if (_board.SoldierTiles.Contains(tile))
            {
                _board.HighlightPieceSoldier(tile);
            }
            if (_board.BoardTiles.Contains(tile))
            {
                _board.HighlightPieceMoves(tile);
            }
            piece.SetFreeze(false);
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
        if (tile != null && !_currentlyDragging.GetLocked() && tile != _currentlyDragging)
        {
            if (_board.UpgradeTiles.Contains(_currentlyDragging) || _board.LegendTiles.Contains(_currentlyDragging))
            {
                _board.OnPieceUpgrade(_currentlyDragging, tile);
                _board.SetLock(_board.UpgradeTiles, _playerColorTurn, true);
            }
            if (_board.SoldierTiles.Contains(_currentlyDragging))
            {
                _board.OnPieceMoved(_currentlyDragging, tile);
                _board.SetLock(_board.SoldierTiles, _playerColorTurn, true);
            }
            if (_board.BoardTiles.Contains(_currentlyDragging))
            {
                _board.OnPieceMoved(_currentlyDragging, tile);
            }
        }
        else
        {
            var temp = _currentlyDragging.GetLocked();
            _currentlyDragging.AddPiece(_currentlyDragging.GetPiece());
            _currentlyDragging.SetLocked(temp);
        }

        _board.UnhighlightAll();
    }

    public void OnClick(Piece piece)
    {
        Tile tile = FindTile(piece);
        if (_board.BoardTiles.Contains(tile) && !tile.GetLocked())
        {
            if (_subState != subState.ATTACKING)
            {
                _subState = subState.ATTACKING;
                _board.UnhighlightAll();
                _board.HighlightPieceAttack(tile);
                tile.AddPiece(tile.GetPiece());
                tile.SetLocked(false);
            } else
            {
                PotentialAttack(tile);
            }
        }
    }

    public void PotentialAttack(Tile tile)
    {
        _board.OnPieceAttack(tile, _currentlyDragging);
        if (!_board.HasPiece(PlayerColorExtensions.GetOpposite(_playerColorTurn)))
        {
            OnWin(_playerColorTurn);
        }
        _subState = subState.NONE;
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
        _board.AddStartingPieces();
        _playerColorTurn = PlayerColor.GREEN;
        OnNextTurn(_playerColorTurn);

        _skipButton.SetActive(true);
    }

    private void OnWin(PlayerColor playerColor)
    {
        _skipButton.SetActive(false);
        _winScreen.GetComponentInChildren<TMP_Text>().text = $"{_playerColorTurn} has won";
        _winScreen.SetActive(true);
    }

    public void OnSkip()
    {
        OnNextTurn(PlayerColorExtensions.GetOpposite(_playerColorTurn));
    }

    public void PlayAgain()
    {
        _winScreen.SetActive(false);
        _skipButton.SetActive(true);
        ChangeLevel(GameLevel.GAME);
    }

}
