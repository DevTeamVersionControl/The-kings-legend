using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Piece : MonoBehaviour
{
    private UnityEvent OnCanBecomeLegend;
    public enum PieceType
    {
        SOLDIER,
        KNIGHT,
        MAGE,
        LEGEND
    };

    private bool freeze;

    [FormerlySerializedAs("_playerColor")] public PlayerColor Color;
    public PieceType Type;

    public bool[][] Movement;
    public bool[][] Attack;
    private GameObject _mesh;

    public int EnemiesKilled;
    const int ENEMIES_FOR_LEGEND = 3;
    
    [SerializeField] GameObject _soldierPrefabGreen;
    [SerializeField] GameObject _soldierPrefabPurple;

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
    
    [SerializeField] GameObject _knightPrefabGreen;
    [SerializeField] GameObject _knightPrefabPurple;
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
    
    [SerializeField] GameObject _magePrefabGreen;
    [SerializeField] GameObject _magePrefabPurple;
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
    
    [SerializeField] GameObject _legendPrefabGreen;
    [SerializeField] GameObject _legendPrefabPurple;
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
    public Tile StartingTile { set; get; }

    public void Awake()
    {
        Movement = MovementMap[Type];
        Attack = AttackMap[Type];
        if (Color == PlayerColor.GREEN)
        {
            MeshMap = new ()
            {
                {PieceType.SOLDIER, _soldierPrefabGreen},
                {PieceType.MAGE, _magePrefabGreen},
                {PieceType.KNIGHT, _knightPrefabGreen},
                {PieceType.LEGEND, _legendPrefabGreen}
            };
        }
        else if (Color == PlayerColor.PURPLE)
        {
            MeshMap = new()
            {
                { PieceType.SOLDIER, _soldierPrefabGreen },
                { PieceType.MAGE, _magePrefabGreen },
                { PieceType.KNIGHT, _knightPrefabGreen },
                { PieceType.LEGEND, _legendPrefabGreen }
            };
        }
        
        {
            
        };
        _mesh?.SetActive(false);
        _mesh = MeshMap[Type];
        _mesh.SetActive(true);
        SetFreeze(true);

        OnCanBecomeLegend = new UnityEvent();
    }

    public void OnKill()
    {
        if (++EnemiesKilled == ENEMIES_FOR_LEGEND)
        {
            OnCanBecomeLegend.Invoke();
        }
    }

    public bool CanBecomeLegend()
    {
        return EnemiesKilled >= ENEMIES_FOR_LEGEND;
    }

    public void SetFreeze(bool freeze)
    {
        this.freeze= freeze;
        if (freeze)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

}
