using UnityEngine;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class URPRendererFixer : MonoBehaviour
{
    [Header("URP Renderer Feature Fixer")]
    [SerializeField] private bool autoFixOnStart = false;
    
    void Start()
    {
        if (autoFixOnStart)
        {
            FixURPRendererFeatures();
        }
    }
    
    [ContextMenu("Fix URP Renderer Features")]
    public void FixURPRendererFeatures()
    {
        Debug.Log("=== URP RENDERER FEATURE FIXER START ===");
        
#if UNITY_EDITOR
        // Problemli renderer data'ları bul
        string[] guids = AssetDatabase.FindAssets("t:UniversalRendererData");
        
        Debug.Log($"🔍 Found {guids.Length} URP Renderer Data assets");
        
        int fixedCount = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            UniversalRendererData rendererData = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(path);
            
            if (rendererData != null)
            {
                Debug.Log($"📊 Checking renderer: {rendererData.name} at {path}");
                
                // Renderer'ın adını kontrol et
                if (rendererData.name.Contains("DepthHistory") || rendererData.name.Contains("KeepFrame"))
                {
                    Debug.LogWarning($"⚠️ Found problematic renderer: {rendererData.name}");
                    
                    // Bu renderer'ları devre dışı bırak veya sil
                    if (FixProblematicRenderer(rendererData, path))
                    {
                        fixedCount++;
                    }
                }
            }
        }
        
        Debug.Log($"✅ Fixed {fixedCount} problematic renderers");
#else
        Debug.LogWarning("⚠️ URP Renderer fixing only available in Editor mode");
#endif
        
        Debug.Log("=== URP RENDERER FEATURE FIXER END ===");
    }

#if UNITY_EDITOR
    private bool FixProblematicRenderer(UniversalRendererData rendererData, string path)
    {
        try
        {
            // Sample renderer'ları ise silebiliriz
            if (path.Contains("Samples/Universal RP"))
            {
                Debug.Log($"🗑️ Deleting sample renderer: {rendererData.name}");
                AssetDatabase.DeleteAsset(path);
                return true;
            }
            else
            {
                // Diğer durumlarda renderer features'ları temizle
                Debug.Log($"🔧 Clearing renderer features for: {rendererData.name}");
                
                var serializedObject = new SerializedObject(rendererData);
                var rendererFeaturesProperty = serializedObject.FindProperty("m_RendererFeatures");
                
                if (rendererFeaturesProperty != null)
                {
                    rendererFeaturesProperty.ClearArray();
                    serializedObject.ApplyModifiedProperties();
                    
                    EditorUtility.SetDirty(rendererData);
                    AssetDatabase.SaveAssetIfDirty(rendererData);
                    
                    Debug.Log($"✅ Cleared renderer features for {rendererData.name}");
                    return true;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error fixing renderer {rendererData.name}: {e.Message}");
        }
        
        return false;
    }
    
    [ContextMenu("Clean All Sample Renderers")]
    public void CleanAllSampleRenderers()
    {
        Debug.Log("🧹 Cleaning all sample renderers...");
        
        // Sample klasörlerindeki tüm renderer'ları sil
        string[] samplePaths = {
            "Assets/Samples/Universal RP",
            "Assets/Imported Assests/EffectExamples"
        };
        
        int deletedCount = 0;
        
        foreach (string samplePath in samplePaths)
        {
            if (AssetDatabase.IsValidFolder(samplePath))
            {
                string[] rendererGuids = AssetDatabase.FindAssets("t:UniversalRendererData", new[] { samplePath });
                
                foreach (string guid in rendererGuids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    AssetDatabase.DeleteAsset(path);
                    deletedCount++;
                    Debug.Log($"🗑️ Deleted sample renderer: {path}");
                }
            }
        }
        
        Debug.Log($"✅ Deleted {deletedCount} sample renderers");
        AssetDatabase.Refresh();
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(URPRendererFixer))]
public class URPRendererFixerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        URPRendererFixer fixer = (URPRendererFixer)target;
        
        if (GUILayout.Button("🔧 Fix URP Renderer Features", GUILayout.Height(30)))
        {
            fixer.FixURPRendererFeatures();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("🧹 Clean All Sample Renderers", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Clean Sample Renderers", 
                "This will delete all sample renderer assets. Continue?", 
                "Yes", "Cancel"))
            {
                fixer.CleanAllSampleRenderers();
            }
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "URP Renderer Feature Issues Çözümü:\n\n" +
            "• Missing RendererFeatures hatalarını düzeltir\n" +
            "• Problematic sample renderer'ları temizler\n" +
            "• DepthHistory ve KeepFrame renderer'ları kaldırır\n" +
            "• Broken renderer features'ları temizler",
            MessageType.Info
        );
        
        // Mevcut durumu göster
        string[] guids = AssetDatabase.FindAssets("t:UniversalRendererData");
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current Status:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Total URP Renderers: {guids.Length}");
        
        int problematicCount = 0;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("DepthHistory") || path.Contains("KeepFrame"))
            {
                problematicCount++;
            }
        }
        
        if (problematicCount > 0)
        {
            EditorGUILayout.HelpBox($"⚠️ {problematicCount} problematic renderers found", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.HelpBox("✅ No problematic renderers detected", MessageType.Info);
        }
    }
}
#endif
