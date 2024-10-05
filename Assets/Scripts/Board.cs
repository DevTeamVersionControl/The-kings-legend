using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    public UnityEvent CancelMoveEvent;

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

    public void Start()
    {
        BoardMap = GetBoardMap();
        BoardTiles = new HashSet<Tile>(_initBoardTiles);
        SoldierTiles = new HashSet<Tile>(_initSoldierTiles);
        BoardTiles = new HashSet<Tile>(_initBoardTiles);
    }

    private Tile[][] GetBoardMap()
    {
        Tile[][] tiles = new Tile[EmptyBoardMap.Length][];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new Tile[EmptyBoardMap[i].Length];
            for (int j = 0; j < tiles[i].Length; j++)
            {
                string tileName = $"{j-tiles[i].Length/2},{tiles.Length/2-i}";
                tiles[i][j] = _initBoardTiles.First(e => e.name == tileName);
            }
        }
        return tiles;
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
        
    }

    public void OnPieceAttack(Tile startTile, Tile endTile)
    {
        
    }

    public void HighlightPieceMoves(Tile tile)
    {
        var piece = tile.GetPiece();
        
    }

    public void HighlightPieceAttack(Tile tile)
    {

    }

    public void HasPiece(PlayerColor playerColor)
    {

    }

    private bool[][] ClipMovesToBoard(bool[][] board, Vector2 position)
    {
        var clippedBoard = new bool[EmptyBoardMap.Length][];
        for (int i = 0; i < EmptyBoardMap.Length; i++)
        {
            clippedBoard[i] = new bool[board[i].Length];
            for (int j = 0; j < EmptyBoardMap[i].Length; j++)
            {
                if (i + position.x >= 0 || i + position.x < board[i].Length
                    && j + position.y >= 0 || j + position.y < board.Length)
                {
                    clippedBoard[i][j] = board[i][j];
                }
                else
                {
                    clippedBoard[i][j] = false;
                }
            }
        }
        return clippedBoard;
    }
}
