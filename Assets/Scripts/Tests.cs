using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Tests
{
    public static void TestClipMovesToBoard()
    {
        // Arrange
        bool testResult = true;
        var pos = new Vector2Int(-1, 1);
        bool[][] SoldierMovement = 
        {   new []{false}, 
            new []{false, false},
            new []{false, true, false},
            new []{true, true},
            new []{false, false, false},
            new []{true, true},
            new []{false, true, false},
            new []{false, false},
            new []{false}
        };
        // Act
        var result = ClipMovesToBoard(SoldierMovement, pos);
        // Assert
        for (int i = 0; i < SoldierMovement.Length; i++)
        {
            for (int j = 0; j < SoldierMovement[i].Length; j++)
            {
                testResult = result[i][j] == EmptyBoardMap[i][j];
                Debug.Log($"{i},{j} : {testResult}");
            }
        }
    }
    
    public static readonly bool[][] EmptyBoardMap = 
    {   new []{false}, 
        new []{true, false},
        new []{true, true, false},
        new []{false, false},
        new []{true, true, false},
        new []{true, false},
        new []{false, false, false},
        new []{false, false},
        new []{false}
    };
    
    
    public static bool[][] ClipMovesToBoard(bool[][] board, Vector2Int position)
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
                Debug.Log($"i:{i} j:{j}");
                Debug.Log($"realPosition:{realPosition}");
                Debug.Log("offset : "+offsetMapPosition);
                var mapPosition = Board.ConvertRealToMapPosition(offsetMapPosition);
                Debug.Log("map : "+mapPosition);
                if (!Board.OutOfWorldCoords(mapPosition))
                {
                    var arrayPosition = Board.ConvertToArrayPosition(mapPosition);
                    clippedBoard[arrayPosition.x][arrayPosition.y] = board[i][j];
                }
            }
        }
        return clippedBoard;
    }
}
