using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public enum PieceType
    {
        SOLDIER,
        KNIGHT,
        MAGE,
        LEGEND
    };
    
    public PlayerColor _playerColor;

    public bool[][] Movement;
    
    public PieceType Type;

    bool[][] Attack;

    int _EnemiesKilled;

    bool _hasMoved;

    const int ENEMIES_FOR_LEGEND = 3;
    
    static readonly bool[][] SoldierMovement = 
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
    static readonly bool[][] SoldierAttack = 
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
    
    static readonly bool[][] KnightMovement = 
    {   new []{true}, 
        new []{true, true},
        new []{true, false, true},
        new []{false, false},
        new []{true, false, true},
        new []{false, false},
        new []{true, false, true},
        new []{true, true},
        new []{true}
    };
    static readonly bool[][] KnightAttack = 
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
    static readonly bool[][] MageMovement = 
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
    static readonly bool[][] MageAttack = 
    {   new []{true}, 
        new []{true, true},
        new []{true, false, true},
        new []{false, false},
        new []{true, false, true},
        new []{false, false},
        new []{true, false, true},
        new []{true, true},
        new []{true}
    };

    static readonly bool[][] LegendAttack = 
    {   new []{true}, 
        new []{true, true},
        new []{true, true, true},
        new []{true, true},
        new []{true, true, true},
        new []{true, true},
        new []{true, true, true},
        new []{true, true},
        new []{true}
    };

    static readonly bool[][] LegendMovement = 
    {   new []{true}, 
        new []{true, true},
        new []{true, true, true},
        new []{true, true},
        new []{true, true, true},
        new []{true, true},
        new []{true, true, true},
        new []{true, true},
        new []{true}
    };
    
    public static readonly Dictionary<PieceType, bool[][]> MovementMap = new(){
        {PieceType.SOLDIER, SoldierMovement},
        {PieceType.MAGE, MageMovement},
        {PieceType.KNIGHT, KnightMovement},
        {PieceType.LEGEND, LegendMovement}
    };
    
    public static readonly Dictionary<PieceType, bool[][]> AttackMap = new(){
        {PieceType.SOLDIER, SoldierAttack},
        {PieceType.MAGE, MageAttack},
        {PieceType.KNIGHT, KnightAttack},
        {PieceType.LEGEND, LegendAttack}
    };

    public void Start()
    {
        Movement = MovementMap[Type];
        Attack = AttackMap[Type];
    }

    public void OnKill()
    {
        
    }

    public void Moved()
    {

    }
}
