using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ShaderAssigner : MonoBehaviour
{
    public Shader baseShader;


    private Texture mainTexture;
    private Texture normalMap;
    private Texture metallicMap;
    private Texture roughnessMap;
    private Texture aoMap;
    private Color baseColor;
    private float smoothness;


    public float duration = 1.0f; 

    void Start()
    {
        baseShader = this.GetComponent<Renderer>().material.shader;
    }

    public void assignShaderMaterial(Material newMaterial)
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null && newMaterial != null)
        {
            // Get the current material
            Material originalMaterial = meshRenderer.material;

            // Store the main texture and other maps from the original material
            mainTexture = originalMaterial.GetTexture("_MainTex");
            normalMap = originalMaterial.GetTexture("_BumpMap");
            metallicMap = originalMaterial.GetTexture("_MetallicGlossMap");
            roughnessMap = originalMaterial.GetTexture("_SpecGlossMap");
            aoMap = originalMaterial.GetTexture("_OcclusionMap");

            // Create a copy of the new material to avoid affecting shared assets
            Material materialInstance = new Material(newMaterial);

            // Assign the stored textures to the new material
            materialInstance.mainTexture = mainTexture;
            if (normalMap) materialInstance.SetTexture("_BumpMap", normalMap);
            if (metallicMap) materialInstance.SetTexture("_MetallicGlossMap", metallicMap);
            if (roughnessMap) materialInstance.SetTexture("_SpecGlossMap", roughnessMap);
            if (aoMap) materialInstance.SetTexture("_OcclusionMap", aoMap);

            // Apply the new material to the object
            meshRenderer.material = materialInstance;
            StartCoroutine(ActivateVFX(newMaterial));
        }
        else
        {
            Debug.LogWarning("MeshRenderer or newMaterial is missing!");
        }
    }

    IEnumerator ActivateVFX(Material newMaterial)
    {


        Renderer renderer = GetComponent<Renderer>();
        Material material = renderer.material;
        string sliderProperty = "_CutoffHeight";
    
        float startValue = 5.0f;
        float targetValue = 0.0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Interpolate the slider value over time
            float newValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            material.SetFloat(sliderProperty, newValue);

            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame
        }
    }

    public IEnumerator AddVFXCoroutine()
    {
        Renderer renderer = GetComponent<Renderer>();
        Material material = renderer.material;
        string sliderProperty = "_CutoffHeight";
        float startValue = 0.0f;
        float targetValue = 6.0f;

        float elapsedTime = 0f;
        duration = 1.0f;
        while (elapsedTime < duration)
        {
            // Interpolate the slider value over time
            float newValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            material.SetFloat(sliderProperty, newValue);
            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame
        }

        ////set the shader and material back to its original state

        material.shader = baseShader;

        // Reassign stored textures and properties
        if (mainTexture)
        {
            material.SetTexture("_MainTex", mainTexture);
            material.SetFloat("_UseColorMap", 1); // Enable Color Map checkbox
        }
        if (normalMap)
        {
            material.SetTexture("_NormalMap", normalMap);
            material.SetFloat("_UseNormalMap", 1); // Enable Normal Map checkbox
        }
        if (metallicMap)
        {
            material.SetTexture("_MetallicMap", metallicMap);
            material.SetFloat("_UseMetallicMap", 1); // Enable Metallic Map checkbox
        }
        if (roughnessMap)
        {
            material.SetTexture("_RoughnessMap", roughnessMap);
            material.SetFloat("_UseRoughnessMap", 1); // Enable Roughness Map checkbox
        }
        if (aoMap)
        {
            material.SetTexture("_AoMap", aoMap);
            material.SetFloat("_UseAoMap", 1); // Enable AO Map checkbox
        }
        material.SetColor("_BaseColor", baseColor);

        yield return null;

    }

    
    

}
