using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    [SerializeField] Tile[] _initUpgradeTiles;
    [SerializeField] Tile[] _initSoldierTiles;
    [SerializeField] Tile[] _initBoardTiles;
    
    public Tile[][] BoardMap;
    public bool[][] SoldierBoardMap;
    
    public HashSet<Tile> BoardTiles;
    public HashSet<Tile> UpgradeTiles;
    public HashSet<Tile> SoldierTiles;

    public void Start()
    {
        
    }

    public void OnNextTurn(PlayerColor playerColor)
    {
        
    }

    public void OnPieceMoved(Tile tile, Vector2 vector)
    {

    }

    public void OnPieceAttack(Tile tile, Vector2 vector)
    {

    }

    public void HighlightPieceMoves(Tile tile)
    {

    }

    public void HighlightPieceAttack(Tile tile)
    {

    }

    public void HasPiece(PlayerColor playerColor)
    {

    }
}
