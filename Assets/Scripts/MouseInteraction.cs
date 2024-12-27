using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MouseInteraction : MonoBehaviour
{
    private Vector3 mouseOffset;

    private float mouseZCoordinate;

    private bool isDragging;
    private bool isHovering;

    private Vector3 mouseDownPosition;

    private float dragThreshold = 10f;

    [SerializeField] bool canDrag;

    [SerializeField] float _dragHeightOffset;

    [FormerlySerializedAs("audioPickUp")] [SerializeField] AudioSource audioSource;
    public AudioClip[] soundPickUp;
    public AudioClip[] soundDrop;
    public AudioClip[] soundClick;
    public AudioClip[] soundHover;

    [SerializeField] private Material highlightMaterial;

    public TileUnityEvent StopMovePiece;
    public PieceUnityEvent StartMovePiece;
    public UnityEvent OnClick;

    public float pitchRange = 0.1f;
    
    private MeshRenderer[] _meshRenderers;

    public void Start()
    {
        StartMovePiece = new PieceUnityEvent();
        StopMovePiece = new TileUnityEvent();
        OnClick ??= new UnityEvent();
        _meshRenderers = GetComponentsInChildren<MeshRenderer>().Append(GetComponent<MeshRenderer>()).ToArray();
    }

    private void OnMouseDown()
    {
        if (!canDrag)
        {
            OnClick.Invoke();
            if (soundClick.Length > 0)
            {
                audioSource.pitch = 1;
                audioSource.PlayOneShot(soundClick[Random.Range(0, soundClick.Length)]);
            }
            return;
        }
        mouseZCoordinate = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        mouseOffset = gameObject.transform.position - GetMouseWorldPos();

        isDragging = false;

        mouseDownPosition = Input.mousePosition;

        Piece piece = gameObject.GetComponent<Piece>();

        AudioClip randomClip = soundPickUp[0];
        audioSource.pitch = 1f + Random.Range(-pitchRange, pitchRange);
        audioSource.PlayOneShot(randomClip);
        piece.PLayPickUpSound();
        StartMovePiece.Invoke(piece);
    }
    
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = mouseZCoordinate;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        if (!canDrag)
        {
            return;
        }
        int layerMask = 1 << LayerMask.NameToLayer("Board");
        int layerOutsideBoard = 1 << LayerMask.NameToLayer("OutsideBoard");

        if (!isDragging && Vector3.Distance(mouseDownPosition, Input.mousePosition) > dragThreshold)
        {
            isDragging = true;
        }

        if(isDragging) { 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerOutsideBoard))   
        {
            Debug.DrawLine(Camera.main.transform.position, hit.point);

            gameObject.transform.position = hit.point + hit.normal *_dragHeightOffset;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10000, Color.white);
            transform.position = GetMouseWorldPos() + mouseOffset;
            gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
        }
    }

    private void OnMouseUp()
    {
        if (!canDrag)
        {
            return;
        }
        Tile TileDrop = null;
        AudioClip randomClip = soundDrop[0];
        audioSource.pitch = 1f + Random.Range(-pitchRange, pitchRange);
        audioSource.PlayOneShot(randomClip);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int layerMask = 1 << LayerMask.NameToLayer("Board");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance,
                Color.yellow);
            TileDrop = hit.transform.GetComponent<Tile>();
           
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10000, Color.white);
        }
        StopMovePiece.Invoke(TileDrop);
    }
    
    void OnMouseOver()
    {
        if (isHovering)
            return;
        isHovering = true;
        if (highlightMaterial)
        {
            foreach (var meshRenderer in _meshRenderers)
            {
                if (!meshRenderer.materials.Last().name.Contains(highlightMaterial.name))
                {
                    meshRenderer.SetMaterials(meshRenderer.materials.Append(highlightMaterial).ToList());
                }
            }
        }

        if (soundHover.Length > 0)
        {
            audioSource.pitch = 1f + Random.Range(-pitchRange, pitchRange);
            audioSource.PlayOneShot(soundHover[0]);
        }
    }

    void OnMouseExit()
    {
        isHovering = false;
        if (highlightMaterial)
        {
            foreach (var meshRenderer in _meshRenderers)
            {
                if (meshRenderer.materials.Last().name.Contains(highlightMaterial.name))
                {
                    var materials = meshRenderer.materials.ToList();
                    materials.Remove(materials.Last());
                    meshRenderer.SetMaterials(materials);
                }
            }
        }
    }


}
