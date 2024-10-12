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

    public Tile _current;
    
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
                _subState = subState.NONE;
                break;
            default:
                break;
        }
    }

    public void OnNextTurn(PlayerColor color)
    {
        _playerColorTurn = color;
        _subState = subState.NONE;
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
        if(piece.Type == Piece.PieceType.LEGEND){
            Debug.Log("OnDragStart called on Legend");
            Debug.Log("Legend is " + (tile.GetLocked() ? "locked" : "unlocked"));
        }
        if(_subState == subState.ATTACKING)
        {
            PotentialAttack(tile);
            return;
        } else if (_subState == subState.NONE){
            _subState = subState.DRAGGING;
            _current = tile;
        } else {
            Debug.LogError("Invalid state");
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
        if(_subState != subState.DRAGGING){
            Debug.LogError($"{_subState} is not valid in this function");
            return;
        }
        _subState = subState.NONE;
        if (tile != null && !_current.GetLocked() && tile != _current)
        {
            if (_board.UpgradeTiles.Contains(_current) || _board.LegendTiles.Contains(_current))
            {
                _board.OnPieceUpgrade(_current, tile);
                _board.SetLock(_board.UpgradeTiles, _playerColorTurn, true);
                _board.SetLock(_board.LegendTiles, _playerColorTurn, true);
            }
            if (_board.SoldierTiles.Contains(_current))
            {
                _board.OnPieceMoved(_current, tile);
                _board.SetLock(_board.SoldierTiles, _playerColorTurn, true);
            }
            if (_board.BoardTiles.Contains(_current))
            {
                _board.OnPieceMoved(_current, tile);
            }
        }
        else
        {
            var temp = _current.GetLocked();
            _current.AddPiece(_current.GetPiece());
            _current.SetLocked(temp);
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
                _current = tile;
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
        _board.OnPieceAttack(_current, tile);
        _board.UnhighlightAll();
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
