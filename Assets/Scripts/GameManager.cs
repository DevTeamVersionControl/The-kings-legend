using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private PlayerColor _playerColorTurn;

    public enum GameLevel { MAINMENU, GAME };

    private GameLevel _currentLevel;

    [SerializeField] GameObject _winScreen;

    [SerializeField] GameObject _skipButton;

    [SerializeField] Board _board;

    [SerializeField] GameObject game;

    public Tile _current;
    
    public static event Action changeTurn;

    public static GameManager Instance;


    public AudioSource audioSource; 
    public AudioClip[] musicTracks;

    private int currentTrackIndex;


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
        }
    }

    public void OnNextTurn(PlayerColor color)
    {
        if (!_board.HasPiece(PlayerColorExtensions.GetOpposite(_playerColorTurn)))
        {
            OnWin(_playerColorTurn);
        }
        _playerColorTurn = color;
        changeTurn?.Invoke();
        if (_board.OnNextTurn(color) < 3){
            _board.SetLock(_board.SoldierTiles, color, false);
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



}
