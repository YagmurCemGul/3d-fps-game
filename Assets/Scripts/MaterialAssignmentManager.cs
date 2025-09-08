using UnityEngine;
using System.Collections.Generic;

public class MaterialAssignmentManager : MonoBehaviour
{
    [Header("Auto Material Assignment")]
    public Material defaultWallMaterial;
    public Material defaultFloorMaterial;
    public Material defaultCeilingMaterial;
    public Material defaultMetalMaterial;
    public Material defaultGlassMaterial;
    
    [Header("Debug")]
    public bool showDebugMessages = true;
    
    void Start()
    {
        AutoAssignMaterials();
    }
    
    [ContextMenu("Auto Assign Materials")]
    public void AutoAssignMaterials()
    {
        if (showDebugMessages)
            Debug.Log("MaterialAssignmentManager: Başlıyor material ataması...");
            
        // Sahne üzerindeki tüm Renderer componentlerini bul
        Renderer[] allRenderers = FindObjectsOfType<Renderer>();
        int materialAssignedCount = 0;
        
        foreach (Renderer renderer in allRenderers)
        {
            // Eğer renderer'da material yoksa veya default material varsa
            if (NeedsMaterialAssignment(renderer))
            {
                Material materialToAssign = GetMaterialForObject(renderer.gameObject);
                
                if (materialToAssign != null)
                {
                    renderer.material = materialToAssign;
                    materialAssignedCount++;
                    
                    if (showDebugMessages)
                        Debug.Log($"Material atandı: {renderer.gameObject.name} -> {materialToAssign.name}");
                }
            }
        }
        
        if (showDebugMessages)
            Debug.Log($"MaterialAssignmentManager: Toplam {materialAssignedCount} objeye material atandı.");
    }
    
    private bool NeedsMaterialAssignment(Renderer renderer)
    {
        if (renderer.material == null)
            return true;
            
        // Default material isimlerini kontrol et
        string materialName = renderer.material.name.ToLower();
        return materialName.Contains("default") || 
               materialName.Contains("standard") ||
               materialName.Contains("missing") ||
               materialName == "";
    }
    
    private Material GetMaterialForObject(GameObject obj)
    {
        string objName = obj.name.ToLower();
        
        // Obje ismine göre material seç
        if (objName.Contains("floor") || objName.Contains("ground"))
        {
            return defaultFloorMaterial;
        }
        else if (objName.Contains("wall") || objName.Contains("duvar"))
        {
            return defaultWallMaterial;
        }
        else if (objName.Contains("roof") || objName.Contains("ceiling") || objName.Contains("tavan"))
        {
            return defaultCeilingMaterial;
        }
        else if (objName.Contains("metal") || objName.Contains("steel") || objName.Contains("pipe"))
        {
            return defaultMetalMaterial;
        }
        else if (objName.Contains("glass") || objName.Contains("window") || objName.Contains("cam"))
        {
            return defaultGlassMaterial;
        }
        
        // Default olarak wall material döndür
        return defaultWallMaterial;
    }
    
    [ContextMenu("Create Default Materials")]
    public void CreateDefaultMaterials()
    {
        // Temel renklerde material oluştur
        defaultWallMaterial = CreateMaterial("DefaultWall", Color.white);
        defaultFloorMaterial = CreateMaterial("DefaultFloor", Color.gray);
        defaultCeilingMaterial = CreateMaterial("DefaultCeiling", Color.white);
        defaultMetalMaterial = CreateMaterial("DefaultMetal", Color.gray);
        defaultGlassMaterial = CreateMaterial("DefaultGlass", new Color(0.8f, 0.9f, 1f, 0.5f));
        
        Debug.Log("MaterialAssignmentManager: Default materialler oluşturuldu!");
    }
    
    private Material CreateMaterial(string name, Color color)
    {
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.name = name;
        mat.color = color;
        
        if (name.Contains("Glass"))
        {
            // Cam için transparency ayarı
            mat.SetFloat("_Surface", 1); // Transparent
            mat.SetFloat("_Blend", 0); // Alpha
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
        
        return mat;
    }
}
