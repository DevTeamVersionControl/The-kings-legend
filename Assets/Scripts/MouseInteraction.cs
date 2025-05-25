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
    private bool selectable = true;

    public bool Selectable
    {
        set
        {
            selectable = value;
        } get { return selectable; }
    }

    private Vector3 mouseDownPosition;
    private Vector3 lastFramePosition;
    private Vector3 initialForwardDirection;

    private float dragThreshold = 10f;



    [SerializeField] bool canDrag;

    [SerializeField] float _dragHeightOffset;

    [FormerlySerializedAs("audioPickUp")] [SerializeField] AudioSource audioSource;
    public AudioClip[] soundPickUp;
    public AudioClip[] soundDrop;
    public AudioClip[] soundClick;
    public AudioClip[] soundHover;

    [SerializeField] private Material highlightMaterial;
    [SerializeField] float rotationMultiplier = 5f;
    [SerializeField] float clamp = 10f;
    [SerializeField] private float rotationSmoothingSpeed = 5f; // Adjust for smoothness
    [SerializeField] private float maxSpeed = 1f; // Adjust for smoothness
    [SerializeField] private Vector3 dragOffset = new Vector3(0.1f, 0.0f, 0.0f);  // Example: small offset on the X-axis
    [SerializeField] private Vector3 lastDragPosition;
    [SerializeField] private float rotationDelay = 0.1f;  // Delay time before applying rotation
    [SerializeField] private float rotationTimer = 0f;   // Timer to track the delay

    public TileUnityEvent StopMovePiece;
    public PieceUnityEvent StartMovePiece;
    public UnityEvent OnClick;

    public float pitchRange = 0.1f;
    
    private MeshRenderer[] _meshRenderers;
    private SkinnedMeshRenderer[] _skinnedMeshRenderers;

    public void Start()
    {
        StartMovePiece = new PieceUnityEvent();
        StopMovePiece = new TileUnityEvent();
        OnClick ??= new UnityEvent();
        _meshRenderers = GetComponentsInChildren<MeshRenderer>()
                     .Append(GetComponent<MeshRenderer>())
                     .Where(renderer => renderer != null)
                     .ToArray();
        _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>()
                            .Append(GetComponent<SkinnedMeshRenderer>())
                            .Where(renderer => renderer != null)
                            .ToArray();
        GameManager.changeTurn += () =>
        {
            if (isDragging)
            {
                OnMouseUp();
            }

            if (isHovering)
            {
                OnMouseExit();
            }
        };
    }

        private void OnMouseDown()
    {
        if (!selectable)
            return;
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

        lastFramePosition = transform.position; // Initialize last frame position

        Piece piece = gameObject.GetComponent<Piece>();

        AudioClip randomClip = soundPickUp[0];
        audioSource.pitch = 1f + Random.Range(-pitchRange, pitchRange);
        audioSource.PlayOneShot(randomClip);
        piece.PlayPickUpSound();
        StartMovePiece.Invoke(piece);
        gameObject.GetComponent<Rigidbody>().useGravity = false;
    }
    
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = mouseZCoordinate;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        if (!selectable)
            return;
        if (!canDrag)
        {
            return;
        }
        int layerMask = 1 << LayerMask.NameToLayer("Board");
        int layerOutsideBoard = 1 << LayerMask.NameToLayer("OutsideBoard");

        if (!isDragging && Vector3.Distance(mouseDownPosition, Input.mousePosition) > dragThreshold)
        {
            isDragging = true;
            initialForwardDirection = transform.forward;
        }

        if(isDragging) { 

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerOutsideBoard))   
            {
                Debug.DrawLine(Camera.main.transform.position, hit.point);

                gameObject.transform.position = hit.point + hit.normal * _dragHeightOffset;
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10000, Color.white);
                transform.position = GetMouseWorldPos() + mouseOffset;
                
            }
            // Calculate velocity
            Vector3 currentFramePosition = transform.position;
            Vector3 velocity = (currentFramePosition - lastFramePosition) / Time.deltaTime;


            float speed = velocity.magnitude;
            // Calculate rotation
            if (speed > maxSpeed)
            {

                // Start the timer when the object starts moving
                if (rotationTimer <= 0f)
                {
                    lastDragPosition = currentFramePosition;
                }

                rotationTimer += Time.deltaTime;

                if (rotationTimer >= rotationDelay)
                {

                    Vector3 velocityDirection = currentFramePosition - lastDragPosition;
                    Vector3 rotationAxis = Vector3.Cross(Vector3.up, velocity.normalized);



                    float rotationAngle = Mathf.Clamp(speed * rotationMultiplier, 0, clamp); // Clamp to a max value

                    Quaternion targetRotation = Quaternion.AngleAxis(rotationAngle, rotationAxis) * Quaternion.LookRotation(initialForwardDirection, Vector3.up);


                    // Apply rotation
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothingSpeed);

                }
            }

            else
            {
                Quaternion uprightRotation = Quaternion.LookRotation(initialForwardDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, uprightRotation, Time.deltaTime * rotationSmoothingSpeed);
                rotationTimer = 0f;
            }
            // Update last frame position
            lastFramePosition = currentFramePosition;
        }
        else
        {
            rotationTimer = 0f;  // Reset the rotation timer if dragging stops
        }
    }

    private void OnMouseUp()
    {
        if (!selectable || !canDrag)
            return;
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
        isDragging = false;
    }
    
    void OnMouseOver()
    {
        if (isHovering || !selectable)
            return;
        isHovering = true;
        if (highlightMaterial)
        {
            if(_meshRenderers != null) { 
                foreach (var meshRenderer in _meshRenderers)
                {
                    if (!meshRenderer.materials.Last().name.Contains(highlightMaterial.name))
                    {
                        meshRenderer.SetMaterials(meshRenderer.materials.Append(highlightMaterial).ToList());
                    }
                }
            }
            if (_skinnedMeshRenderers != null)
            {
                foreach (var skinnedMeshRenderer in _skinnedMeshRenderers)
                {
                    if (!skinnedMeshRenderer.materials.Last().name.Contains(highlightMaterial.name))
                    {
                        skinnedMeshRenderer.SetMaterials(skinnedMeshRenderer.materials.Append(highlightMaterial).ToList());
                    }
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
            if (_meshRenderers != null)
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

            if (_skinnedMeshRenderers != null)
            {
                foreach (var skinnedMeshRenderer in _skinnedMeshRenderers)
                {
                    if (skinnedMeshRenderer.materials.Last().name.Contains(highlightMaterial.name))
                    {
                        var materials = skinnedMeshRenderer.materials.ToList();
                        materials.Remove(materials.Last());
                        skinnedMeshRenderer.SetMaterials(materials);
                    }
                }
            }
            
        }
    }


}
