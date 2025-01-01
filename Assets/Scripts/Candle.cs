using UnityEngine;
using UnityEngine.Events;

public class Candle : MonoBehaviour
{
    [SerializeField] private float waitTime = 20;
    
    private float remainingTime;
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
        if (!active)
        {
            remainingTime += Time.deltaTime * 2;
            
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
    }

    public void StopTimer()
    {
        active = false;
    }

}
