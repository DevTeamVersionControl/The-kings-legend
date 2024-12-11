using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShaderAssigner : MonoBehaviour
{

    public Material newMaterial; // Assign the new material in the Inspector

    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null && newMaterial != null)
        {
            // Get the current material
            Material originalMaterial = meshRenderer.material;

            // Store the main texture and other maps from the original material
            Texture mainTexture = originalMaterial.mainTexture;
            Texture normalMap = originalMaterial.GetTexture("_BumpMap");
            Texture metallicMap = originalMaterial.GetTexture("_MetallicGlossMap");
            Texture roughnessMap = originalMaterial.GetTexture("_SpecGlossMap");
            Texture aoMap = originalMaterial.GetTexture("_OcclusionMap");

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
        }
        else
        {
            Debug.LogWarning("MeshRenderer or newMaterial is missing!");
        }
    }
}
