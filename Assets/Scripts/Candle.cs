using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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
    public float RemainingTime { get=>remainingTime; }

    private bool active = false;
    public bool Active { get => active; set => active = value; }
    
    public UnityEvent timeout = new();
    
    private SkinnedMeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        remainingTime = waitTime;
    }
    void Update()
    {
        if (GameManager.Instance.isPaused)
        {
            return;
        }

        if (particleInstance != null)
        {
            Vector3 distance = topPosition.transform.position - bottomPosition.transform.position;
            Vector3 position = distance * remainingTime / waitTime + bottomPosition.transform.position;
            particleInstance.GetComponent<Transform>().position = position;
            light.transform.position = position;
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
        light.transform.position = topPosition.transform.position;
        light.enabled = true;
    }

    public void StopTimer()
    {
        active = false;
        if (particleInstance != null)
            Destroy(particleInstance);
        Vector3 distance = topPosition.transform.position - bottomPosition.transform.position;
        Vector3 position = distance * remainingTime / waitTime + bottomPosition.transform.position;
        particleInstance = Instantiate(smokePrefab, position, topPosition.transform.rotation);
        light.transform.position = position;
        light.enabled = false;
    }

}
