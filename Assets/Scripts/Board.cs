using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    public UnityEvent CancelMoveEvent;
    public TileUnityEvent PieceKilledEvent;

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
    private Dictionary<Tile, Vector2Int> _positions; 

    public void Start()
    {
        FillBoardMap();
        BoardTiles = new HashSet<Tile>(_initBoardTiles);
        SoldierTiles = new HashSet<Tile>(_initSoldierTiles);
        BoardTiles = new HashSet<Tile>(_initBoardTiles);

        HighlightPieceMoves(BoardMap[2][0]);
    }

    private Vector2Int ConvertToMapPosition(int i, int j)
    {
        return new Vector2Int(j-EmptyBoardMap[i].Length/2, EmptyBoardMap.Length/2-i);
    }
    
    private Vector2Int ConvertToArrayPosition(int x, int y)
    {
        return new Vector2Int(EmptyBoardMap.Length/2-y, x+EmptyBoardMap[EmptyBoardMap.Length/2-y].Length/2);
    }

    private bool OutOfWorldCoords(Vector2Int position)
    {
        int i = EmptyBoardMap.Length / 2 - position.y;
        if (i < 0 || i >= EmptyBoardMap.Length)
        {
            return true;
        }
        int j = position.x + EmptyBoardMap[i].Length / 2;
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

    public void OnNextTurn(PlayerColor playerColor)
    {
        foreach (var tile in _initBoardTiles)
        {
            var piece = tile.GetPiece();
            if (piece != null)
            {
                if (piece._playerColor == playerColor)
                {
                    tile.Unlock();
                }
                else
                {
                    tile.Lock();
                }
            }
        }
    }

    public void OnPieceMoved(Tile startTile, Tile endTile)
    {
        if (endTile.IsHighlighted())
        {
            endTile.AddPiece(startTile.RemovePiece());
        }
        else
        {
            startTile.UnhidePiece();
        }
        UnHighlightAll();
    }

    public void OnPieceAttack(Tile endTile)
    {
        if (endTile.IsHighlighted())
        {
            PieceKilledEvent.Invoke(endTile);
        }
        UnHighlightAll();
    }

    public void HighlightPieceMoves(Tile tile)
    {
        var piece = tile.GetPiece();
        var highlightedTiles = ClipMovesToBoard(piece.Movement, ConvertToMapPosition(_positions[tile].x, _positions[tile].y));
        for (int i = 0; i < highlightedTiles.Length; i++)
        {
            for (int j = 0; j < highlightedTiles[i].Length; j++)
            {
                if (highlightedTiles[i][j] && BoardMap[i][j].GetPiece() == null)
                {
                    BoardMap[i][j].Highlight();
                }
            }
        }
    }

    public void HighlightPieceAttack(Tile tile)
    {
        var piece = tile.GetPiece();
        var highlightedTiles = ClipMovesToBoard(piece.Attack, ConvertToMapPosition(_positions[tile].x, _positions[tile].y));
        for (int i = 0; i < highlightedTiles.Length; i++)
        {
            for (int j = 0; j < highlightedTiles[i].Length; j++)
            {
                if (highlightedTiles[i][j] && BoardMap[i][j].GetPiece() == null)
                {
                    BoardMap[i][j].Highlight();
                }
            }
        }
    }

    private void UnHighlightAll()
    {
        foreach (var tile in _initBoardTiles)
        {
            tile.Unhighlight();
        }
    }

    public bool HasPiece(PlayerColor playerColor)
    {
        if (_initBoardTiles.Any(t => t.GetPiece()?._playerColor == playerColor))
        {
            return true;
        }
        return false;
    }

    private bool[][] ClipMovesToBoard(bool[][] board, Vector2Int position)
    {
        var clippedBoard = new bool[EmptyBoardMap.Length][];
        for (int i = 0; i < board.Length; i++)
        {
            clippedBoard[i] = new bool[board[i].Length];
        }
        for (int i = 0; i < EmptyBoardMap.Length; i++)
        {
            for (int j = 0; j < EmptyBoardMap[i].Length; j++)
            {
                var worldPosition = ConvertToMapPosition(i, j);
                var offsetMapPosition = new Vector2Int(worldPosition.x + position.x, worldPosition.y + position.y);
                if (!OutOfWorldCoords(offsetMapPosition))
                {
                    var arrayPosition = ConvertToArrayPosition(offsetMapPosition.x, offsetMapPosition.y);
                    clippedBoard[arrayPosition.x][arrayPosition.y] = board[i][j];
                }
            }
        }
        return clippedBoard;
    }
    
    private void printArray(Tile[][] tiles)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            string row = "[";
            for (int j = 0; j < tiles[i].Length; j++)
            {
                row += $"{tiles[i][j].GetPiece().name}, ";
            }
            Debug.Log(row+"]");
        }
    }
    
    private void printArray(bool[][] tiles)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            string row = "[";
            for (int j = 0; j < tiles[i].Length; j++)
            {
                row += $"{tiles[i][j]}, ";
            }
            Debug.Log(row+"]");
        }
    }
}
