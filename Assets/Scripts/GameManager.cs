using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

    [SerializeField] GameObject UI;

    [SerializeField] CapsuleCollider _capsuleColliderGreenCandle;

    [SerializeField] CapsuleCollider _capsuleColliderPurpleCandle;

    public Tile _current;
    
    public static event Action changeTurn;

    public static event Action loadGame;

    public static event Action mainMenu;

    public static GameManager Instance;

    public UnityEvent OnEnd;


    public AudioSource audioSource; 
    public AudioClip[] musicTracks;

    [FormerlySerializedAs("greenTimer")] [SerializeField] private Candle greenCandle;
    [FormerlySerializedAs("purpleTimer")] [SerializeField] private Candle purpleCandle;

    private int currentTrackIndex;

    private bool hasWon;

    public bool isPaused;

    private bool firstGame; 
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

        //initialize starting tiles
        _board.AddStartingPieces();
    }

    public void ChangeLevel(GameLevel level)
    {
        _currentLevel = level;
        switch (level)
        {
            case GameLevel.MAINMENU:
                mainMenu?.Invoke();
                UI.SetActive(true);
                break;
            case GameLevel.GAME:
                UI.SetActive(false);
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
            OnWin(_playerColorTurn);
            
        }
        else 
        { 
            if (color == PlayerColor.GREEN)
            {
                greenCandle.StartTimer();
                purpleCandle.StopTimer();
            }
            else
            {
                purpleCandle.StartTimer();
                greenCandle.StopTimer();
            }
        
            _playerColorTurn = color;
        
            changeTurn?.Invoke();
            if (_board.OnNextTurn(color) < 3){
                _board.SetLock(_board.SoldierTiles, color, false);
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
                    
                }
            }

        //initialize starting tiles
        _board.AddStartingPieces();

        _playerColorTurn = PlayerColor.GREEN;
        OnNextTurn(_playerColorTurn);
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
        int newTrackIndex;
        do
        {
            newTrackIndex = UnityEngine.Random.Range(0, musicTracks.Length);
        } while (newTrackIndex == currentTrackIndex);

        currentTrackIndex = newTrackIndex;
        audioSource.clip = musicTracks[currentTrackIndex];
        audioSource.Play();

        
        StartCoroutine(WaitForTrackEnd(audioSource.clip.length));
    }


    IEnumerator WaitForTrackEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        PlayRandomTrack();  // Play the next random track
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
