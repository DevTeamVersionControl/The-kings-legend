using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static Piece;
using Debug = UnityEngine.Debug;

public class Board : MonoBehaviour
{

    [SerializeField] Tile[] _initLegendTiles;
    [SerializeField] Tile[] _initUpgradeTiles;
    [SerializeField] Tile[] _initSoldierTiles;
    [SerializeField] Tile[] _initBoardTiles;
    
    public Tile[][] BoardMap;
    
    public static readonly bool[][] EmptyBoardMap = 
        {   new []{false}, 
            new []{false, false},
            new []{false, false, false},
            new []{false, false},
            new []{false, false, false},
            new []{false, false},
            new []{false, false, false},
            new []{false, false},
            new []{false}
        };
   
    public HashSet<Tile> BoardTiles;
    public HashSet<Tile> UpgradeTiles;
    public HashSet<Tile> SoldierTiles;
    public HashSet<Tile> LegendTiles;
    public List<Tile> AllTiles;
    private Dictionary<Tile, Vector2Int> _positions;

    public AudioSource AudioKill;
    public AudioClip[] SoundKill;


    public void Awake()
    {
        GameManager.loadGame += BoardStart;
    }

    public void OnDestroy()
    {
        GameManager.loadGame -= BoardStart;
    }
    public void BoardStart()
    {

        
        FillBoardMap();
        BoardTiles = new HashSet<Tile>(_initBoardTiles);
        SoldierTiles = new HashSet<Tile>(_initSoldierTiles);
        UpgradeTiles = new HashSet<Tile>(_initUpgradeTiles);
        LegendTiles = new HashSet<Tile>(_initLegendTiles);
        AllTiles = new List<Tile>(BoardTiles.Union(UpgradeTiles).Union(LegendTiles).Union(SoldierTiles));
        StartCoroutine(WaitUntilGameManagerIsReady());
    }


    private IEnumerator WaitUntilGameManagerIsReady()
    {
        // Wait until GameManager.Instance is not null
        while (GameManager.Instance == null)
        {
            yield return null; // Wait for the next frame
        }

        yield return null;

        // Now GameManager is ready, call GameInits
        GameManager.Instance.GameInit(this);
    }

    public static Vector2Int ConvertToMapPosition(Vector2Int position)
    {
        return new Vector2Int(position.y-EmptyBoardMap[position.x].Length/2, EmptyBoardMap.Length/2-position.x);
    }
    
    public static Vector2Int ConvertMapToRealPosition(Vector2Int position)
    {
        Vector2Int realPosition = new Vector2Int(position.x, position.y);
        if (position.x == 0 && Math.Abs(position.y)%2==1)
        {
            realPosition = new Vector2Int(1, position.y);
        } else if (position.x == 1)
        {
            realPosition = new Vector2Int(2, position.y);
        } else if (position.x == -1 && Math.Abs(position.y) % 2 == 0)
        {
            realPosition = new Vector2Int(-2, position.y);
        }
        return realPosition;
    }
    
    public static Vector2Int ConvertRealToMapPosition(Vector2Int position)
    {
        Vector2Int mapPosition = new Vector2Int(position.x, position.y);
        if (position.x == 1)
        {
            mapPosition = new Vector2Int(0, position.y);
        } else if (position.x == 2)
        {
            mapPosition = new Vector2Int(1, position.y);
        } else if (position.x == -2)
        {
            mapPosition = new Vector2Int(-1, position.y);
        }
        return mapPosition;
    }
    
    public static Vector2Int ConvertToArrayPosition(Vector2Int position)
    {
        
        return new Vector2Int(EmptyBoardMap.Length/2-position.y, position.x+EmptyBoardMap[EmptyBoardMap.Length/2-position.y].Length/2);
    }

    public static bool OutOfWorldCoords(Vector2Int arrayPosition)
    {
        int i = EmptyBoardMap.Length / 2 - arrayPosition.y;
        if (i < 0 || i >= EmptyBoardMap.Length)
        {
            return true;
        }
        int j = arrayPosition.x + EmptyBoardMap[i].Length / 2;
        if (j < 0 || j >= EmptyBoardMap[i].Length)
        {
            return true;
        }

        return false;
    }

    private void FillBoardMap()
    {
        BoardMap = new Tile[EmptyBoardMap.Length][];
        _positions = new();
        for (int i = 0; i < BoardMap.Length; i++)
        {
            BoardMap[i] = new Tile[EmptyBoardMap[i].Length];
            for (int j = 0; j < BoardMap[i].Length; j++)
            {
                string tileName = $"{j-BoardMap[i].Length/2},{BoardMap.Length/2-i}";
                BoardMap[i][j] = _initBoardTiles.First(e => e.name == tileName);
                _positions[BoardMap[i][j]] = new Vector2Int(i, j);
            }
        }
    }
    public int OnNextTurn(PlayerColor playerColor)
    {
        int piecesNb = 0;
        bool soldier = false;
        SetLock(BoardTiles, playerColor, false);
        SetLock(BoardTiles, PlayerColorExtensions.GetOpposite(playerColor), true);
        foreach (var tile in BoardTiles)
        {
            if (tile.GetPiece()?.Color == playerColor)
            {
                if (tile.GetPiece()?.Type == Piece.PieceType.SOLDIER)
                {
                    soldier = true;
                }
                piecesNb++;
            }
        }
        if (soldier)
        {
            SetLock(UpgradeTiles, playerColor, false);
            SetLock(LegendTiles, playerColor, false);
        }

        return piecesNb;
    }

    public void OnPieceMoved(Tile startTile, Tile endTile)
    {
        switch (endTile.GetHighlight())
        {
            case Tile.HighlightType.NONE:
                startTile.AddPiece(startTile.RemovePiece());
                startTile.SetLocked(false);
                if (!BoardTiles.Contains(startTile)){
                    SetLock(SoldierTiles, startTile.GetPiece().Color, false);
                }
                break;
            case Tile.HighlightType.MOVE:
                endTile.AddPiece(startTile.RemovePiece());
                startTile.RemovePiece();
                break;
            case Tile.HighlightType.ATTACK:
                AudioClip randomClip = SoundKill[UnityEngine.Random.Range(0, SoundKill.Length)];
                AudioKill.PlayOneShot(randomClip);
                startTile.GetPiece().OnKill();
                endTile.GetPiece().EnemiesKilled = 0;
                startTile.AddPiece(startTile.RemovePiece());
                StartCoroutine(PieceDeath(startTile, endTile));
                break;
        }
        UnhighlightAll();
    }
    
    IEnumerator PieceDeath(Tile startTile, Tile endTile)
    {
        Material dissolve;
        Piece piece = endTile.RemovePiece();
        
        Vector3 animationPosition = piece.transform.position;
        piece.StartingTile.AddPiece(piece);
        Vector3 finalPosition = piece.transform.position;
        piece.transform.position = animationPosition;

        dissolve = piece.materialKilled;

        piece.ActivateVFX(dissolve);
        
        yield return new WaitForSeconds(1);
        //change the dissolve for the corresponding team
        if (piece.Color == PlayerColor.PURPLE)
        {
            dissolve = piece.materialVFXPurple;
        }
        else
        {
            dissolve = piece.materialVFXGreen;
        }
        piece.AddVFX(dissolve);
        piece.transform.position = finalPosition;
        yield return new WaitForSeconds(1);

    }
    
    public IEnumerator OnPieceUpgrade(Tile startTile, Tile endTile)
    {
        Piece upgrade = startTile.RemovePiece();
        
        if (endTile.GetHighlight() == Tile.HighlightType.MOVE)
        {
            Piece soldier = endTile.RemovePiece();
            
            Vector3 animationPosition = soldier.transform.position;
            soldier.StartingTile.AddPiece(soldier);
            Vector3 finalPosition = soldier.transform.position;
            soldier.transform.position = animationPosition;
            
            endTile.AddPiece(upgrade);
            upgrade.EnemiesKilled = soldier.EnemiesKilled;
            soldier.EnemiesKilled = 0;
            SetLock(UpgradeTiles, upgrade.Color, true);
            SetLock(LegendTiles, upgrade.Color, true);
            Material dissolve;
            
            if (soldier.Color == PlayerColor.PURPLE)
            {
                dissolve = soldier.materialVFXPurple;
            }
            else
            {
                dissolve = soldier.materialVFXGreen;
            }
            
            soldier.ActivateVFX(dissolve);
            upgrade.gameObject.SetActive(false);
            upgrade.ActivateVFX(dissolve);

            yield return new WaitForSeconds(2);

            upgrade.gameObject.SetActive(true);
            soldier.transform.position = finalPosition;
            soldier.AddVFX(dissolve);
            upgrade.AddVFX(dissolve);
        }
        else
        {
            startTile.AddPiece(upgrade);
            startTile.SetLocked(false);
        }
        UnhighlightAll();
        yield return null;
    }


    public void HighlightPiece(Tile startingTile)
    {
        var piece = startingTile.GetPiece();
        if (BoardTiles.Contains(startingTile))
        {
            var highlightedTiles = ClipMovesToBoard(piece.Movement, ConvertToMapPosition(_positions[startingTile]));
            for (int i = 0; i < highlightedTiles.Length; i++)
            {
                for (int j = 0; j < highlightedTiles[i].Length; j++)
                {
                    if (highlightedTiles[i][j] && BoardMap[i][j].GetPiece() == null)
                    {
                        BoardMap[i][j].Highlight(Tile.HighlightType.MOVE);
                    }
                }
            }
            HighlightPieceAttack(startingTile);
        }
    }

    public void HighlightPieceSoldier(Tile startingTile)
    {
        Piece piece = startingTile.GetPiece();
        var tiles = new Tile[3];
        if (piece.Color == PlayerColor.GREEN)
        {
            tiles = new[]{BoardMap[2][0], BoardMap[4][0], BoardMap[6][0]};
        }
        else
        {
            tiles = new[]{BoardMap[2][2], BoardMap[4][2], BoardMap[6][2]};
        }

        foreach (var tile in tiles)
        {
            if (tile.GetPiece() == null)
            {
                tile.Highlight(Tile.HighlightType.MOVE);
            }
        }
    }

    public void HighlightPieceUpgrade(Tile startingTile)
    {
        Piece piece = startingTile.GetPiece();
        foreach (var tile in BoardTiles)
        {
            var potentialPiece = tile.GetPiece();
            if (potentialPiece != null && !tile.GetLocked())
            {
                if (piece.Type == Piece.PieceType.LEGEND)
                {
                    if (potentialPiece.EnemiesKilled >= 3)
                    {
                        tile.Highlight(Tile.HighlightType.MOVE);
                    }
                }
                else
                {
                    if (potentialPiece.Type == Piece.PieceType.SOLDIER)
                    {
                        tile.Highlight(Tile.HighlightType.MOVE);
                    }
                }
            }
        }
    }

    private void HighlightPieceAttack(Tile startingTile)
    {
        var piece = startingTile.GetPiece();
        var highlightedTiles = ClipMovesToBoard(piece.Attack, ConvertToMapPosition(_positions[startingTile]));
        for (int i = 0; i < highlightedTiles.Length; i++)
        {
            for (int j = 0; j < highlightedTiles[i].Length; j++)
            {
                var highlightedPiece = BoardMap[i][j].GetPiece();
                if (highlightedTiles[i][j] && highlightedPiece != null && highlightedPiece.Color != piece.Color)
                {
                    BoardMap[i][j].Highlight(Tile.HighlightType.ATTACK);
                }
            }
        }
    }

    public void UnhighlightAll()
    {
        foreach (var tile in _initBoardTiles)
        {
            tile.Highlight(Tile.HighlightType.NONE);
        }
    }

    public bool HasPiece(PlayerColor playerColor)
    {
        if (_initBoardTiles.Any(t => t.GetPiece()?.Color == playerColor))
        {
            return true;
        }
        return false;
    }

    bool[][] ClipMovesToBoard(bool[][] board, Vector2Int position)
    {
        position = Board.ConvertMapToRealPosition(position);
        var clippedBoard = new bool[EmptyBoardMap.Length][];
        for (int i = 0; i < board.Length; i++)
        {
            clippedBoard[i] = new bool[board[i].Length];
        }
        for (int i = 0; i < EmptyBoardMap.Length; i++)
        {
            for (int j = 0; j < EmptyBoardMap[i].Length; j++)
            {
                var realPosition = Board.ConvertMapToRealPosition(Board.ConvertToMapPosition(new Vector2Int(i, j)));
                var offsetMapPosition = realPosition + position;
                var mapPosition = Board.ConvertRealToMapPosition(offsetMapPosition);
                if (!Board.OutOfWorldCoords(mapPosition))
                {
                    var arrayPosition = Board.ConvertToArrayPosition(mapPosition);
                    clippedBoard[arrayPosition.x][arrayPosition.y] = board[i][j];
                }
            }
        }
        return clippedBoard;
    }

    public void SetLock(IEnumerable<Tile> tiles, PlayerColor playerColor, bool locked)
    {
        foreach (var tile in tiles)
        {
            if (tile.GetPiece()?.Color == playerColor)
            {
                tile.SetLocked(locked);
            }
        }
    }
    
    
    public void AddStartingPieces()
    {
        Dictionary<PlayerColor, Vector2Int[]> startingTilesDict = new()
        {
            { PlayerColor.GREEN, new[] { new Vector2Int(-1, 2), new Vector2Int(-1, 0), new Vector2Int(-1, -2) } },
            { PlayerColor.PURPLE, new[] { new Vector2Int(1, 2), new Vector2Int(1, 0), new Vector2Int(1, -2) } }
        };
        int i = 0, j = 0;
        foreach (var tile in SoldierTiles)
        {
            if (tile.GetPiece()?.Color == PlayerColor.GREEN)
            {
                var position = ConvertToArrayPosition(startingTilesDict[PlayerColor.GREEN][i++]);
                BoardMap[position.x][position.y].AddPiece(tile.RemovePiece());
            }
            else if (tile.GetPiece()?.Color == PlayerColor.PURPLE)
            {
                var position = ConvertToArrayPosition(startingTilesDict[PlayerColor.PURPLE][j++]);
                BoardMap[position.x][position.y].AddPiece(tile.RemovePiece());
            }
        }
    }
}
