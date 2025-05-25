using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Piece : MonoBehaviour
{
    public UnityEvent OnCanBecomeLegend;
    private MouseInteraction mouseInteraction;
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
    public const int ENEMIES_FOR_LEGEND = 2;
    
    [SerializeField] GameObject _soldierPrefabGreen;
    [SerializeField] GameObject _soldierPrefabPurple;

    public Material materialKilled;
    public Material materialVFXGreen;
    public Material materialVFXPurple;

    [SerializeField] AudioSource audioPickUp;

    [SerializeField] AudioClip horsePickUpSound;
    [SerializeField] AudioClip soldierPickUpSound;
    [SerializeField] AudioClip magePickUpSound;
    [SerializeField] AudioClip legendPickUpSound;

    private float _stretchAmount = 1.5f;
    private float _squashAmount = 0.6f;
    private float _duration = 1f;

    [SerializeField] Object killParticleOne;
    [SerializeField] Object killParticleTwo;

    private Vector3 _originalScale;

    public float pitchRange = 0.1f;
    public Object particleInstance = new();

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
        GameManager.loadGame += PieceStart;
        mouseInteraction = GetComponent<MouseInteraction>();
    }

    public void OnDestroy()
    {
        GameManager.loadGame -= PieceStart;
    }
    public void PieceStart()
    {
        Destroy(particleInstance);
        _originalScale = transform.localScale;
        EnemiesKilled = 0;      
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
                { PieceType.SOLDIER, _soldierPrefabPurple },
                { PieceType.MAGE, _magePrefabPurple },
                { PieceType.KNIGHT, _knightPrefabPurple },
                { PieceType.LEGEND, _legendPrefabPurple }
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
        if (Type == PieceType.LEGEND)
        {
            return;
        }

        EnemiesKilled++;
        
        if (CanBecomeLegend())
        {
            Debug.Log($"{name} sends signal to become legend");
            OnCanBecomeLegend.Invoke();
        }

       StartCoroutine(OnKillParticule());
    }

    IEnumerator OnKillParticule()
    {
        yield return new WaitForSeconds(0.1f);
        if (EnemiesKilled == 1)
        {
            particleInstance = Instantiate(killParticleOne, transform);
        } else if (EnemiesKilled == 2)
        {
            Destroy(particleInstance);
            particleInstance = Instantiate(killParticleTwo, transform);
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
            GetComponent<Rigidbody>().detectCollisions = true;
        }
        else
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            GetComponent<Rigidbody>().detectCollisions = false;
        }
    }
    

    public void ActivateVFX(Color hdrColor)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf) 
            {
                
                foreach (Transform bodyPart in child)
                {
                    
                    var bodyPartScript = bodyPart.GetComponent<ShaderAssigner>();
                    if (bodyPartScript != null)
                    {
                        StartCoroutine(bodyPartScript.DissolveDownVFX(hdrColor));
                    }
                }

               
                break;
            }
        }
    }

    public void AddVFX(Color hdrColor)
    {

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {

                foreach (Transform bodyPart in child)
                {

                    var bodyPartScript = bodyPart.GetComponent<ShaderAssigner>();
                    if (bodyPartScript != null)
                    {
                        StartCoroutine(bodyPartScript.DissolveUpVFX(hdrColor));
                    }
                }


                break;
            }
        }
    }

    public void PLayPickUpSound()
    {

        audioPickUp.pitch = 1f + Random.Range(-pitchRange, pitchRange);
            
            
        switch (Type)
        {
            case PieceType.SOLDIER:
                audioPickUp.PlayOneShot(soldierPickUpSound);
                break;

            case PieceType.MAGE:
                audioPickUp.PlayOneShot(magePickUpSound);
                break;
            case PieceType.KNIGHT:
                audioPickUp.PlayOneShot(horsePickUpSound);
                break;
            case PieceType.LEGEND:
                audioPickUp.PlayOneShot(legendPickUpSound);
                break;

        }
        
    }

    public void SetSelectable(bool selectable)
    {
        mouseInteraction.Selectable = selectable;
    }

    public async Task PlaySquashAndStetch()
    {
        Sequence seq = DOTween.Sequence();

        Vector3 squashScale = _originalScale * _squashAmount;
        seq.Append(transform.DOScale(squashScale, _duration / 3f).SetEase(Ease.OutQuad));

        Vector3 stretchScale = _originalScale * _stretchAmount;
        seq.Append(transform.DOScale(stretchScale, _duration / 3f).SetEase(Ease.OutBack));

        seq.Append(transform.DOScale(_originalScale, _duration / 3f).SetEase(Ease.InOutQuad));

        seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);

        await seq.AsyncWaitForCompletion();
    }
}


