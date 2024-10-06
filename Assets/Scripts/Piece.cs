using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Piece : MonoBehaviour
{
    private UnityEvent CanBecomeLegend;
    public enum PieceType
    {
        SOLDIER,
        KNIGHT,
        MAGE,
        LEGEND
    };
    
    [FormerlySerializedAs("_playerColor")] public PlayerColor Color;
    public PieceType Type;

    public bool[][] Movement;
    public bool[][] Attack;
    private GameObject _mesh;

    public int EnemiesKilled;

    const int ENEMIES_FOR_LEGEND = 3;
    
    [SerializeField] GameObject _soldierPrefab;
    
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
    
    [SerializeField] GameObject _knightPrefab;
    
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
    
    [SerializeField] GameObject _magePrefab;
    
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
    
    [SerializeField] GameObject _legendPrefab;

    static readonly bool[][] LegendAttack = 
    {   new []{true}, 
        new []{true, true},
        new []{true, true, true},
        new []{true, true},
        new []{true, false, true},
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
        new []{true, false, true},
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

    public Dictionary<PieceType, GameObject> MeshMap;

    public void Start()
    {
        MeshMap = new ()
        {
            {PieceType.SOLDIER, _soldierPrefab},
            {PieceType.MAGE, _magePrefab},
            {PieceType.KNIGHT, _knightPrefab},
            {PieceType.LEGEND, _legendPrefab}
        };
        Movement = MovementMap[Type];
        Attack = AttackMap[Type];
        _mesh?.SetActive(false);
        _mesh = MeshMap[Type];
        _mesh.SetActive(true);
    }

    public void OnKill()
    {
        if (++EnemiesKilled == ENEMIES_FOR_LEGEND)
        {
            CanBecomeLegend.Invoke();
        }
    }

}
