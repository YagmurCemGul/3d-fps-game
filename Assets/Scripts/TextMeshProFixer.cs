using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TextMeshProFixer : MonoBehaviour
{
    [Header("TextMeshPro Shader Fixer")]
    [SerializeField] private bool autoFixOnStart = false;
    
    void Start()
    {
        if (autoFixOnStart)
        {
            FixTextMeshProShaders();
        }
    }
    
    [ContextMenu("Fix TextMeshPro Shaders")]
    public void FixTextMeshProShaders()
    {
        Debug.Log("=== TEXTMESHPRO SHADER FIXER START ===");
        
        // Scene'deki tüm TextMeshPro componentlerini bul
        TextMeshProUGUI[] uiTexts = FindObjectsOfType<TextMeshProUGUI>();
        TextMeshPro[] worldTexts = FindObjectsOfType<TextMeshPro>();
        
        Debug.Log($"🔤 Found {uiTexts.Length} UI texts and {worldTexts.Length} world texts");
        
        int fixedCount = 0;
        
        // UI TextMeshPro'ları düzelt
        foreach (var text in uiTexts)
        {
            if (FixTextMeshProComponent(text.gameObject, text))
            {
                fixedCount++;
            }
        }
        
        // World TextMeshPro'ları düzelt
        foreach (var text in worldTexts)
        {
            if (FixTextMeshProComponent(text.gameObject, text as TMP_Text))
            {
                fixedCount++;
            }
        }
        
        Debug.Log($"✅ Fixed {fixedCount} TextMeshPro shader issues");
        Debug.Log("=== TEXTMESHPRO SHADER FIXER END ===");
    }
    
    private bool FixTextMeshProComponent(GameObject obj, TMP_Text tmpText)
    {
        if (tmpText == null) return false;
        
        Material currentMaterial = tmpText.fontMaterial;
        if (currentMaterial == null)
        {
            // Default material ata
            tmpText.font = Resources.GetBuiltinResource<TMP_FontAsset>("LegacyRuntime.ttf");
            Debug.Log($"✅ {obj.name}: Assigned default font");
            return true;
        }
        
        Shader currentShader = currentMaterial.shader;
        if (currentShader == null || currentShader.name.Contains("Distance Field SSD"))
        {
            // Çalışan shader'a geç
            Shader workingShader = null;
            
            // UI için
            if (tmpText is TextMeshProUGUI)
            {
                workingShader = Shader.Find("TextMeshPro/Distance Field");
                if (workingShader == null)
                    workingShader = Shader.Find("GUI/Text Shader");
            }
            // World için
            else
            {
                workingShader = Shader.Find("TextMeshPro/Distance Field");
                if (workingShader == null)
                    workingShader = Shader.Find("Sprites/Default");
            }
            
            if (workingShader != null)
            {
                // Yeni material oluştur
                Material newMaterial = new Material(workingShader);
                newMaterial.name = $"{obj.name}_FixedTMP_Material";
                
                // Temel özellikleri kopyala
                if (currentMaterial != null)
                {
                    newMaterial.mainTexture = currentMaterial.mainTexture;
                    if (currentMaterial.HasProperty("_MainTex"))
                        newMaterial.mainTexture = currentMaterial.GetTexture("_MainTex");
                    if (currentMaterial.HasProperty("_Color"))
                        newMaterial.color = currentMaterial.GetColor("_Color");
                }
                
                tmpText.fontMaterial = newMaterial;
                Debug.Log($"✅ {obj.name}: Fixed shader from '{currentShader?.name}' to '{workingShader.name}'");
                return true;
            }
        }
        
        return false;
    }
    
    [ContextMenu("Reset All TextMeshPro to Default")]
    public void ResetAllTextMeshProToDefault()
    {
        Debug.Log("🔄 Resetting all TextMeshPro to default settings...");
        
        TextMeshProUGUI[] uiTexts = FindObjectsOfType<TextMeshProUGUI>();
        TextMeshPro[] worldTexts = FindObjectsOfType<TextMeshPro>();
        
        foreach (var text in uiTexts)
        {
            text.font = Resources.GetBuiltinResource<TMP_FontAsset>("LegacyRuntime.ttf");
            text.fontMaterial = null; // Unity otomatik default atar
        }
        
        foreach (var text in worldTexts)
        {
            text.font = Resources.GetBuiltinResource<TMP_FontAsset>("LegacyRuntime.ttf");
            text.fontMaterial = null; // Unity otomatik default atar
        }
        
        Debug.Log($"✅ Reset {uiTexts.Length + worldTexts.Length} TextMeshPro components");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TextMeshProFixer))]
public class TextMeshProFixerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        TextMeshProFixer fixer = (TextMeshProFixer)target;
        
        if (GUILayout.Button("🔧 Fix TextMeshPro Shaders", GUILayout.Height(30)))
        {
            fixer.FixTextMeshProShaders();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("🔄 Reset All to Default", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Reset TextMeshPro", 
                "This will reset all TextMeshPro components to default settings. Continue?", 
                "Yes", "Cancel"))
            {
                fixer.ResetAllTextMeshProToDefault();
            }
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "TextMeshPro Shader Issues Çözümü:\n\n" +
            "• Broken shader'ları working shader'lara değiştirir\n" +
            "• Distance Field SSD hatalarını düzeltir\n" +
            "• Missing material'ları default'a atar\n" +
            "• UI ve World text'leri ayrı ayrı handle eder",
            MessageType.Info
        );
        
        // Mevcut durum göstergesi
        TextMeshProUGUI[] uiTexts = FindObjectsOfType<TextMeshProUGUI>();
        TextMeshPro[] worldTexts = FindObjectsOfType<TextMeshPro>();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current Status:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"UI Texts Found: {uiTexts.Length}");
        EditorGUILayout.LabelField($"World Texts Found: {worldTexts.Length}");
        
        // Problematic shader'ları say
        int problematicCount = 0;
        foreach (var text in uiTexts)
        {
            if (text.fontMaterial?.shader?.name?.Contains("SSD") == true)
                problematicCount++;
        }
        foreach (var text in worldTexts)
        {
            if (text.fontMaterial?.shader?.name?.Contains("SSD") == true)
                problematicCount++;
        }
        
        if (problematicCount > 0)
        {
            EditorGUILayout.HelpBox($"⚠️ {problematicCount} TextMeshPro components have problematic shaders", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.HelpBox("✅ No TextMeshPro shader issues detected", MessageType.Info);
        }
    }
}
#endif
