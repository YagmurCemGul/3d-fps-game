using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UltimateFixer : MonoBehaviour
{
    [Header("Ultimate Game Fixer")]
    [SerializeField] private bool autoFixOnStart = false;
    
    void Start()
    {
        if (autoFixOnStart)
        {
            FixAllProblems();
        }
    }
    
    [ContextMenu("Fix All Problems")]
    public void FixAllProblems()
    {
        Debug.Log("🚀 === ULTIMATE FIXER START ===");
        
        // 1. TextMeshPro sorunları
        FixTextMeshProIssues();
        
        // 2. URP Renderer sorunları
        FixURPRendererIssues();
        
        // 3. Package conflict'leri kontrol et
        CheckPackageConflicts();
        
        // 4. Comprehensive game fixes
        FixGameIssues();
        
        // 5. Material issues
        FixMaterialIssues();
        
        Debug.Log("✅ === ULTIMATE FIXER COMPLETE ===");
        Debug.Log("🎮 Oyun artık çalışmaya hazır!");
    }
    
    private void FixTextMeshProIssues()
    {
        Debug.Log("📝 Fixing TextMeshPro issues...");
        
        var tmpFixer = GetOrCreateComponent<TextMeshProFixer>();
        tmpFixer.FixTextMeshProShaders();
    }
    
    private void FixURPRendererIssues()
    {
        Debug.Log("🎨 Fixing URP Renderer issues...");
        
        var urpFixer = GetOrCreateComponent<URPRendererFixer>();
        urpFixer.FixURPRendererFeatures();
    }
    
    private void CheckPackageConflicts()
    {
        Debug.Log("📦 Checking package conflicts...");
        
        var packageDetector = GetOrCreateComponent<PackageConflictDetector>();
        packageDetector.DetectPackageConflicts();
    }
    
    private void FixGameIssues()
    {
        Debug.Log("⚙️ Fixing game issues...");
        
        var gameFixer = GetOrCreateComponent<ComprehensiveGameFixer>();
        gameFixer.FixAllIssues();
    }
    
    private void FixMaterialIssues()
    {
        Debug.Log("🎭 Fixing material issues...");
        
        // MaterialAssignmentManager varsa kullan
        var materialManager = FindObjectOfType<MaterialAssignmentManager>();
        if (materialManager != null)
        {
            materialManager.GetType().GetMethod("AssignMaterials")?.Invoke(materialManager, null);
        }
        
        // AdvancedMaterialManager varsa kullan
        var advancedMaterialManager = FindObjectOfType<AdvancedMaterialManager>();
        if (advancedMaterialManager != null)
        {
            advancedMaterialManager.GetType().GetMethod("FixAllMaterials")?.Invoke(advancedMaterialManager, null);
        }
    }
    
    private T GetOrCreateComponent<T>() where T : Component
    {
        T component = GetComponent<T>();
        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }

#if UNITY_EDITOR
    [ContextMenu("Delete Sample Assets")]
    public void DeleteSampleAssets()
    {
        if (EditorUtility.DisplayDialog("Delete Sample Assets", 
            "Bu işlem tüm sample asset'leri silecek. Devam etmek istiyor musunuz?", 
            "Evet", "Hayır"))
        {
            Debug.Log("🗑️ Deleting sample assets...");
            
            string[] sampleFolders = {
                "Assets/Samples",
                "Assets/Imported Assests/EffectExamples",
            };
            
            foreach (string folder in sampleFolders)
            {
                if (AssetDatabase.IsValidFolder(folder))
                {
                    AssetDatabase.DeleteAsset(folder);
                    Debug.Log($"🗑️ Deleted: {folder}");
                }
            }
            
            AssetDatabase.Refresh();
            Debug.Log("✅ Sample assets deleted");
        }
    }
    
    [ContextMenu("Reimport All Assets")]
    public void ReimportAllAssets()
    {
        if (EditorUtility.DisplayDialog("Reimport All Assets", 
            "Bu işlem tüm asset'leri yeniden import edecek. Zaman alabilir. Devam?", 
            "Evet", "Hayır"))
        {
            Debug.Log("🔄 Reimporting all assets...");
            AssetDatabase.ImportAsset("Assets", ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);
            Debug.Log("✅ All assets reimported");
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(UltimateFixer))]
public class UltimateFixerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        UltimateFixer fixer = (UltimateFixer)target;
        
        // Ana fix butonu
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("🚀 FIX ALL PROBLEMS", GUILayout.Height(50)))
        {
            fixer.FixAllProblems();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        
        // Individual fix butonları
        EditorGUILayout.LabelField("Individual Fixes:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("📝 Fix TextMeshPro"))
        {
            var tmpFixer = fixer.GetComponent<TextMeshProFixer>();
            if (tmpFixer == null) tmpFixer = fixer.gameObject.AddComponent<TextMeshProFixer>();
            tmpFixer.FixTextMeshProShaders();
        }
        if (GUILayout.Button("🎨 Fix URP Renderer"))
        {
            var urpFixer = fixer.GetComponent<URPRendererFixer>();
            if (urpFixer == null) urpFixer = fixer.gameObject.AddComponent<URPRendererFixer>();
            urpFixer.FixURPRendererFeatures();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("📦 Check Packages"))
        {
            var packageDetector = fixer.GetComponent<PackageConflictDetector>();
            if (packageDetector == null) packageDetector = fixer.gameObject.AddComponent<PackageConflictDetector>();
            packageDetector.DetectPackageConflicts();
        }
        if (GUILayout.Button("⚙️ Fix Game Issues"))
        {
            var gameFixer = fixer.GetComponent<ComprehensiveGameFixer>();
            if (gameFixer == null) gameFixer = fixer.gameObject.AddComponent<ComprehensiveGameFixer>();
            gameFixer.FixAllIssues();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        // Tehlikeli işlemler
        EditorGUILayout.LabelField("Advanced Operations:", EditorStyles.boldLabel);
        
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("🗑️ Delete Sample Assets"))
        {
            fixer.DeleteSampleAssets();
        }
        
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("🔄 Reimport All Assets"))
        {
            fixer.ReimportAllAssets();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "Ultimate Fixer - Tüm Sorunları Çözer:\n\n" +
            "🚀 Fix All Problems: Tüm sorunları tek seferde çözer\n" +
            "📝 TextMeshPro shader sorunları\n" +
            "🎨 URP Renderer feature sorunları\n" +
            "📦 Package conflict kontrolü\n" +
            "⚙️ Game manager ve material sorunları\n" +
            "🗑️ Sample asset temizleme\n" +
            "🔄 Asset reimport işlemleri",
            MessageType.Info
        );
    }
}
#endif
