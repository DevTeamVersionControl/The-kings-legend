using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Candle : MonoBehaviour
{
    [SerializeField] private float waitTime = 20;
    [SerializeField] private Object smokePrefab;
    [SerializeField] private Object flamePrefab;
    [SerializeField] private GameObject topPosition;
    [SerializeField] private GameObject bottomPosition;
    [SerializeField] private Light light;
    
    private float remainingTime;
    private Object particleInstance;
    private MouseInteraction mouseInteraction;
    private SkinnedMeshRenderer meshRenderer;
    
    public float RemainingTime { get=>remainingTime; }

    private bool active = false;
    private Vector3 lightOffset;
    public bool Active { get => active; set => active = value; }
    
    public UnityEvent timeout = new();
    

    void Start()
    {
        meshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        remainingTime = waitTime;
        mouseInteraction = gameObject.GetComponent<MouseInteraction>();
        lightOffset = light.transform.position - topPosition.transform.position;
    }
    void Update()
    {
        if (GameManager.Instance.isPaused)
        {
            return;
        }
        
        if(GameManager.Instance._currentLevel == GameManager.GameLevel.MAINMENU)
        {
            remainingTime += Time.deltaTime * 5;

            if (meshRenderer != null)
            {
                meshRenderer.SetBlendShapeWeight(0, 100 * (1 - remainingTime / waitTime));
            }

            if (remainingTime > waitTime)
            {
                remainingTime = waitTime;
                return;
            }
        }

        if (particleInstance != null)
        {
            Vector3 distance = topPosition.transform.position - bottomPosition.transform.position;
            Vector3 position = distance * remainingTime / waitTime + bottomPosition.transform.position;
            particleInstance.GetComponent<Transform>().position = position;
            light.transform.position = position + lightOffset;
        }

        if (!active)
        {
            remainingTime += Time.deltaTime * 5;
            
            if (meshRenderer != null)
            {
                meshRenderer.SetBlendShapeWeight(0, 100 * (1-remainingTime / waitTime));
            }

            if (remainingTime > waitTime)
            {
                remainingTime = waitTime;
            }
            return;
        }
        
        remainingTime -= Time.deltaTime;

        if (remainingTime > 0)
        {
            if (meshRenderer != null)
            {
                meshRenderer.SetBlendShapeWeight(0, 100 * (1 - remainingTime / waitTime));
            }
            return;
        }
        
        timeout?.Invoke();
    }

    public void StartTimer()
    {
        active = true;
        remainingTime = waitTime;
        if (particleInstance != null)
            Destroy(particleInstance);
        particleInstance = Instantiate(flamePrefab, topPosition.transform.position, topPosition.transform.rotation);
        light.transform.position = topPosition.transform.position + lightOffset;
        light.enabled = true;
        mouseInteraction.Selectable = true;
    }

    public void StopTimer()
    {
        active = false;
        if (particleInstance != null)
            Destroy(particleInstance);
        Vector3 distance = topPosition.transform.position - bottomPosition.transform.position;
        Vector3 position = distance * remainingTime / waitTime + bottomPosition.transform.position;
        particleInstance = Instantiate(smokePrefab, position, topPosition.transform.rotation);
        light.transform.position = position + lightOffset;
        light.enabled = false;
        mouseInteraction.Selectable = false;
    }

}
