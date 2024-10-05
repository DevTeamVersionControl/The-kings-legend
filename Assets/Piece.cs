using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public GameManager.PlayerColor _playerColor;
 
    bool[,] Movement;

    bool[,] Attack;

    int _EnemiesKilled;

    bool _hasMoved;

    const int ENEMIES_FOR_LEGEND = 3;

    public void OnKill()
    {

    }

    public void Moved()
    {

    }
}
