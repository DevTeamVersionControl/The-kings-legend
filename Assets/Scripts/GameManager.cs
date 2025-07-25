using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public PlayerColor _playerColorTurn;

    public enum GameLevel { MAINMENU, GAME };

    public GameLevel _currentLevel;

    [SerializeField] GameObject _winScreen;

    [SerializeField] Board _board;

    [SerializeField] GameObject game;

    [SerializeField] CapsuleCollider _capsuleColliderGreenCandle;

    [SerializeField] CapsuleCollider _capsuleColliderPurpleCandle;

    [SerializeField] PlayableDirector _greenWinCinematic;
    [SerializeField] PlayableDirector _purpleWinCinematic;

    public Tile _current;
    
    public static event Action changeTurn;

    public static event Action loadGame;

    public static event Action mainMenu;

    public static GameManager Instance;

    public UnityEvent OnEnd;


    public AudioSource audioSource; 
    public AudioClip musicTracks;

    [FormerlySerializedAs("greenTimer")] [SerializeField] private Candle greenCandle;
    [FormerlySerializedAs("purpleTimer")] [SerializeField] private Candle purpleCandle;

    private int currentTrackIndex;

    private bool hasWon;

    public bool isPaused;

    private bool firstGame; 
    public void Awake()
    {
        Application.targetFrameRate = 300;
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
        PlayRandomTrack();
        OnEnd ??= new UnityEvent();

        loadGame?.Invoke();
          foreach (Tile tile in _board.AllTiles){
            var piece = tile.GetPiece();       
            if (piece != null)
            {
                piece.StartingTile = tile;
            }
        }

       
        _board.AddStartingPieces();
    }

    public void ChangeLevel(GameLevel level)
    {
        _currentLevel = level;
        switch (level)
        {
            case GameLevel.MAINMENU:
                mainMenu?.Invoke();
                break;
            case GameLevel.GAME:
                _capsuleColliderGreenCandle.enabled = true;
                _capsuleColliderPurpleCandle.enabled = true;
                _playerColorTurn = PlayerColor.GREEN;
                _current = null;
                loadGame?.Invoke();
                GameInit();
                hasWon = false;
                break;
        }
    }

    public void OnNextTurn(PlayerColor color)
    {
        if (!_board.HasPiece(color) && !hasWon)
        {
            hasWon = true;
            StartCoroutine(StartWinCinematic(_playerColorTurn));
        }
        else
        {
            if (color == PlayerColor.GREEN)
            {
                greenCandle.StartTimer();
                purpleCandle.StopTimer();
                _board.PurpleLegendTray.SetLocked(true);
                _board.PurpleSoldierTray.SetLocked(true);
                _board.PurpleUpgradeTray.SetLocked(true);
            }
            else
            {
                purpleCandle.StartTimer();
                greenCandle.StopTimer();
                _board.GreenLegendTray.SetLocked(true);
                _board.GreenSoldierTray.SetLocked(true);
                _board.GreenUpgradeTray.SetLocked(true);
            }

            _playerColorTurn = color;

            changeTurn?.Invoke();
            if (_board.OnNextTurn(color) < 3)
            {
                if (color == PlayerColor.GREEN)
                {
                    _board.GreenSoldierTray.SetLocked(false);
                    _board.GreenUpgradeTray.SetLocked(false);
                }
                else
                {
                    _board.PurpleSoldierTray.SetLocked(false);
                    _board.PurpleUpgradeTray.SetLocked(false);
                }
            }
        }
    }

    public void OnDragStart(Piece piece)
    {
        Tile tile = FindTile(piece);
        _current = tile;
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
                _board.HighlightPiece(tile);
            }
            piece.SetFreeze(false);
        }
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
        if (tile != null && !_current.GetLocked() && tile != _current)
        {
            if (_board.UpgradeTiles.Contains(_current) || _board.LegendTiles.Contains(_current))
            {
                StartCoroutine(_board.OnPieceUpgrade(_current, tile));
                if (!_board.LegendTiles.Contains(_current))
                {
                    if (_playerColorTurn == PlayerColor.GREEN)
                    {
                        _board.GreenSoldierTray.SetLocked(true);
                        _board.GreenUpgradeTray.SetLocked(true);
                    }
                    else
                    {
                        _board.PurpleSoldierTray.SetLocked(true);
                        _board.PurpleUpgradeTray.SetLocked(true);
                    }
                }
            }
            if (_board.SoldierTiles.Contains(_current))
            {
                if (_board.OnPieceMoved(_current, tile))
                {
                    if (_playerColorTurn == PlayerColor.GREEN)
                    {
                        _board.GreenSoldierTray.SetLocked(true);
                        _board.GreenUpgradeTray.SetLocked(true);
                    }
                    else
                    {
                        _board.PurpleSoldierTray.SetLocked(true);
                        _board.PurpleUpgradeTray.SetLocked(true);
                    }
                }
            }
            if (_board.BoardTiles.Contains(_current))
            {
                _board.OnPieceMoved(_current, tile);
            }
        }
        else
        {
            var temp = _current.GetLocked();
            _current.AddPieceAndMoveBack(_current.GetPiece());
            _current.SetLocked(temp);
        }
        _board.UnhighlightAll();
    }
    

    public void GameInit()
    {
        greenCandle.timeout.AddListener(()=>OnNextTurn(PlayerColor.PURPLE));
        purpleCandle.timeout.AddListener(()=>OnNextTurn(PlayerColor.GREEN));
        
            foreach (Tile tile in _board.AllTiles)
            {
                var piece = tile.GetPiece();
                if (piece != null)
                {
                    MouseInteraction interaction = piece.GetComponent<MouseInteraction>();
                    interaction.StopMovePiece.AddListener(OnDragEnd);
                    interaction.StartMovePiece.AddListener(OnDragStart);
                    piece.OnCanBecomeLegend.AddListener(() =>
                    {
                        if (piece.Color == PlayerColor.GREEN)
                        {
                            _board.GreenLegendTray.SetLocked(false);
                        }
                        else
                        {
                            _board.PurpleLegendTray.SetLocked(false);
                        }
                    });
                }
            }

        //initialize starting tiles
        _board.AddStartingPieces();
        _board.GreenLegendTray.SetLocked(true);
        _board.GreenSoldierTray.SetLocked(true);

        _playerColorTurn = PlayerColor.GREEN;
        OnNextTurn(_playerColorTurn);
        //OnSkip();
        //OnSkip();
    }
    
    private IEnumerator StartWinCinematic(PlayerColor playerColor)
    {
        ChangeLevel(GameLevel.MAINMENU);
        if (playerColor == PlayerColor.GREEN)
        {
            _greenWinCinematic.Play();
            yield return new WaitForSeconds((float)_greenWinCinematic.duration);
        }
        else
        {
            _purpleWinCinematic.Play();
            yield return new WaitForSeconds((float)_purpleWinCinematic.duration);
        }
        yield return new WaitForSeconds(1f);
        OnWin(playerColor);
    }

    private void OnWin(PlayerColor playerColor)
    {
        Debug.Log("the game has ended");
        OnEnd?.Invoke();
        mainMenu?.Invoke();
        _board.ResetBoard();
        foreach (Tile tile in _board.BoardTiles)
        {
           tile.RemovePiece();
        }

        foreach (Tile tile in _board.AllTiles)
        {
            var piece = tile.GetPiece();
            if (piece != null)
            {
                MouseInteraction interaction = piece.GetComponent<MouseInteraction>();
                interaction.StopMovePiece.RemoveListener(OnDragEnd);
                interaction.StartMovePiece.RemoveListener(OnDragStart);
            }
        }
        ChangeLevel(GameLevel.MAINMENU);
    }



    public void OnSkip()
    {
        OnNextTurn(PlayerColorExtensions.GetOpposite(_playerColorTurn));
    }

    public void PlayAgain()
    {
        _board.ResetBoard();
        _winScreen.SetActive(false);
        foreach (Tile tile in _board.BoardTiles)
        {
            tile.RemovePiece();
        }

        foreach (Tile tile in _board.AllTiles)
        {
            var piece = tile.GetPiece();
            if (piece != null)
            {
                MouseInteraction interaction = piece.GetComponent<MouseInteraction>();
                interaction.StopMovePiece.RemoveListener(OnDragEnd);
                interaction.StartMovePiece.RemoveListener(OnDragStart);
            }
        }
        ChangeLevel(GameLevel.GAME);
    }

    void PlayRandomTrack()
    {
        audioSource.clip = musicTracks;
        audioSource.Play();

        
        StartCoroutine(WaitForTrackEnd(audioSource.clip.length));
    }


    IEnumerator WaitForTrackEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        PlayRandomTrack();
    }

    public void OnPause()
    {
        if (!isPaused)
        {
            Debug.Log("paused");
            isPaused = true;
            mainMenu?.Invoke();


        }
        else if (isPaused)
        {
            Debug.Log("Unpaused");
            isPaused = false;
            changeTurn?.Invoke();
        }
    }

}
