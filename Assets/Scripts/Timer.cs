using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] private float waitTime = 20;
    
    private float remainingTime;
    public float RemainingTime { get=>remainingTime; }

    private bool active = false;
    public bool Active { get => active; set => active = value; }
    
    public Animation animation;

    public UnityEvent timeout = new();
    
    void Update()
    {
        if (!active)
            return;
        
        remainingTime -= Time.deltaTime;

        if (remainingTime > 0)
        {
            animation["burn"].time = animation["burn"].length * (1 - remainingTime / waitTime);
            return;
        }
        
        remainingTime = waitTime;
        timeout?.Invoke();
    }

    public void StartTimer()
    {
        active = true;
        remainingTime = waitTime;
        animation["burn"].time = 0;
    }

    public void StopTimer()
    {
        active = false;
        remainingTime = 0;
        animation.Play("fill");
    }

}
