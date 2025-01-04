using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ShaderAssigner : MonoBehaviour
{
    [ColorUsage(true, true)] // Enable HDR and show alpha channel
    public Color hdrColor;


    public float duration = 1.0f; 


    public IEnumerator DissolveDownVFX(Color hdrColor)
    {
        Material material = GetComponent<Renderer>().material;
        string sliderProperty = "_cutoffHeight";
        string dissolveColor = "_dissolveColor";
        float startValue = 5.0f;
        float targetValue = 0.0f;
        float elapsedTime = 0f;

        material.SetColor(dissolveColor, hdrColor);

        while (elapsedTime < duration)
        {
            // Interpolate the slider value over time
            float newValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            material.SetFloat(sliderProperty, newValue);
            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame
        }
        
    }

    public IEnumerator DissolveUpVFX(Color hdrColor)
    {

        Material material = GetComponent<Renderer>().material;

        string sliderProperty = "_cutoffHeight";
        string dissolveColor = "_dissolveColor";
        float startValue = 0.0f;
        float targetValue = 6.0f;
        float elapsedTime = 0f;
        duration = 1.0f;

        material.SetColor(dissolveColor, hdrColor);

        while (elapsedTime < duration)
        {
            // Interpolate the slider value over time
            float newValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            material.SetFloat(sliderProperty, newValue);
            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame
        }
    }

}
