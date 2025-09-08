using UnityEngine;
using System.Collections.Generic;

public class AdvancedMaterialManager : MonoBehaviour
{
    [Header("Texture Resources")]
    public Texture2D diffuseTexture;
    public Texture2D normalTexture;
    public Texture2D metallicTexture;
    public Texture2D ambientTexture;
    
    [Header("Material Settings")]
    public bool autoLoadTextures = true;
    public bool useAdvancedMaterials = true;
    
    void Start()
    {
        if (autoLoadTextures)
        {
            LoadTexturesFromResources();
        }
        SetupAdvancedMaterials();
    }
    
    void LoadTexturesFromResources()
    {
        Debug.Log("AdvancedMaterialManager: Textureler yükleniyor...");
        
        // _Barking_Dog asset'indeki textureleri yükle
        diffuseTexture = Resources.Load<Texture2D>("_Barking_Dog/3D Free Modular Kit/_Textures/Diffuse_01");
        normalTexture = Resources.Load<Texture2D>("_Barking_Dog/3D Free Modular Kit/_Textures/Normal_01");
        metallicTexture = Resources.Load<Texture2D>("_Barking_Dog/3D Free Modular Kit/_Textures/Metal_01");
        ambientTexture = Resources.Load<Texture2D>("_Barking_Dog/3D Free Modular Kit/_Textures/Ambient_01");
        
        if (diffuseTexture == null)
        {
            Debug.LogWarning("Diffuse texture bulunamadı. Alternatif yol deneniyor...");
            // Alternatif yollar dene
            diffuseTexture = LoadTextureAlternative("Diffuse_01");
        }
        
        Debug.Log($"Texture yükleme durumu - Diffuse: {diffuseTexture != null}, Normal: {normalTexture != null}, Metallic: {metallicTexture != null}, Ambient: {ambientTexture != null}");
    }
    
    Texture2D LoadTextureAlternative(string textureName)
    {
        // Farklı yolları dene
        string[] paths = {
            $"Assets/_Barking_Dog/3D Free Modular Kit/_Textures/{textureName}",
            $"_Textures/{textureName}",
            textureName
        };
        
        foreach (string path in paths)
        {
            Texture2D tex = Resources.Load<Texture2D>(path);
            if (tex != null)
            {
                Debug.Log($"Texture bulundu: {path}");
                return tex;
            }
        }
        
        return null;
    }
    
    [ContextMenu("Setup Advanced Materials")]
    public void SetupAdvancedMaterials()
    {
        Debug.Log("AdvancedMaterialManager: Gelişmiş material kurulumu başlıyor...");
        
        // Sahne objelerini kategorilere ayır
        CategorizeAndAssignMaterials();
        
        // Lighting ayarlarını optimize et
        OptimizeLighting();
    }
    
    void CategorizeAndAssignMaterials()
    {
        Renderer[] allRenderers = FindObjectsOfType<Renderer>();
        
        Dictionary<string, Material> materialCache = new Dictionary<string, Material>();
        
        foreach (Renderer renderer in allRenderers)
        {
            string category = GetObjectCategory(renderer.gameObject);
            
            if (!materialCache.ContainsKey(category))
            {
                materialCache[category] = CreateMaterialForCategory(category);
            }
            
            if (materialCache[category] != null)
            {
                renderer.material = materialCache[category];
            }
        }
        
        Debug.Log($"AdvancedMaterialManager: {materialCache.Count} farklı kategori için material oluşturuldu.");
    }
    
    string GetObjectCategory(GameObject obj)
    {
        string name = obj.name.ToLower();
        
        // Detaylı kategorizasyon
        if (name.Contains("floor") || name.Contains("ground"))
            return "Floor";
        else if (name.Contains("wall") || name.Contains("duvar"))
            return "Wall";
        else if (name.Contains("roof") || name.Contains("ceiling"))
            return "Ceiling";
        else if (name.Contains("door") || name.Contains("kapi"))
            return "Door";
        else if (name.Contains("window") || name.Contains("glass") || name.Contains("cam"))
            return "Glass";
        else if (name.Contains("metal") || name.Contains("steel") || name.Contains("pipe"))
            return "Metal";
        else if (name.Contains("light") || name.Contains("lamp"))
            return "Light";
        else if (name.Contains("console") || name.Contains("computer") || name.Contains("screen"))
            return "Tech";
        else if (name.Contains("column") || name.Contains("pillar"))
            return "Structure";
        else
            return "Default";
    }
    
    Material CreateMaterialForCategory(string category)
    {
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.name = $"Auto_{category}_Material";
        
        // Kategori bazında material ayarları
        switch (category)
        {
            case "Floor":
                mat.color = new Color(0.6f, 0.6f, 0.65f);
                mat.SetFloat("_Metallic", 0.1f);
                mat.SetFloat("_Smoothness", 0.4f);
                break;
                
            case "Wall":
                mat.color = new Color(0.8f, 0.85f, 0.9f);
                mat.SetFloat("_Metallic", 0f);
                mat.SetFloat("_Smoothness", 0.2f);
                break;
                
            case "Ceiling":
                mat.color = new Color(0.9f, 0.9f, 0.95f);
                mat.SetFloat("_Metallic", 0f);
                mat.SetFloat("_Smoothness", 0.1f);
                break;
                
            case "Door":
                mat.color = new Color(0.4f, 0.3f, 0.2f);
                mat.SetFloat("_Metallic", 0.3f);
                mat.SetFloat("_Smoothness", 0.6f);
                break;
                
            case "Glass":
                mat.color = new Color(0.8f, 0.9f, 1f, 0.3f);
                mat.SetFloat("_Surface", 1); // Transparent
                mat.SetFloat("_Metallic", 0f);
                mat.SetFloat("_Smoothness", 0.9f);
                SetupTransparency(mat);
                break;
                
            case "Metal":
                mat.color = new Color(0.7f, 0.7f, 0.8f);
                mat.SetFloat("_Metallic", 0.8f);
                mat.SetFloat("_Smoothness", 0.7f);
                break;
                
            case "Light":
                mat.color = Color.white;
                mat.SetColor("_EmissionColor", Color.white * 0.5f);
                mat.EnableKeyword("_EMISSION");
                break;
                
            case "Tech":
                mat.color = new Color(0.2f, 0.2f, 0.3f);
                mat.SetFloat("_Metallic", 0.7f);
                mat.SetFloat("_Smoothness", 0.8f);
                mat.SetColor("_EmissionColor", new Color(0, 0.5f, 1f) * 0.2f);
                mat.EnableKeyword("_EMISSION");
                break;
                
            case "Structure":
                mat.color = new Color(0.7f, 0.7f, 0.75f);
                mat.SetFloat("_Metallic", 0.2f);
                mat.SetFloat("_Smoothness", 0.3f);
                break;
                
            default:
                mat.color = Color.white;
                mat.SetFloat("_Metallic", 0.1f);
                mat.SetFloat("_Smoothness", 0.3f);
                break;
        }
        
        // Texture uygulaması
        if (diffuseTexture != null && category != "Glass" && category != "Light")
        {
            mat.mainTexture = diffuseTexture;
        }
        
        if (normalTexture != null && category != "Glass")
        {
            mat.SetTexture("_BumpMap", normalTexture);
            mat.EnableKeyword("_NORMALMAP");
        }
        
        return mat;
    }
    
    void SetupTransparency(Material mat)
    {
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
    
    void OptimizeLighting()
    {
        Debug.Log("AdvancedMaterialManager: Lighting optimizasyonu başlıyor...");
        
        // Ambient lighting ayarı
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.5f, 0.7f, 1f);
        RenderSettings.ambientEquatorColor = new Color(0.4f, 0.4f, 0.6f);
        RenderSettings.ambientGroundColor = new Color(0.2f, 0.2f, 0.3f);
        
        // Fog ayarları
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.3f, 0.4f, 0.6f);
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 20f;
        RenderSettings.fogEndDistance = 100f;
        
        Debug.Log("Lighting optimizasyonu tamamlandı.");
    }
    
    [ContextMenu("Reset All Materials")]
    public void ResetAllMaterials()
    {
        Renderer[] allRenderers = FindObjectsOfType<Renderer>();
        
        foreach (Renderer renderer in allRenderers)
        {
            renderer.material = null;
        }
        
        Debug.Log("Tüm materialler sıfırlandı.");
    }
}
