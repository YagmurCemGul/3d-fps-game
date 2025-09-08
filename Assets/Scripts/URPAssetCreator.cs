using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Rendering.Universal;
#endif

public class URPAssetCreator : MonoBehaviour
{
    [Header("URP Asset Creator")]
    [SerializeField] private bool autoCreateOnStart = false;
    
    void Start()
    {
        if (autoCreateOnStart)
        {
            CheckAndCreateURPAsset();
        }
    }
    
    [ContextMenu("Create URP Asset")]
    public void CheckAndCreateURPAsset()
    {
        Debug.Log("=== URP ASSET CREATOR START ===");
        
        // Mevcut render pipeline kontrolü
        RenderPipelineAsset currentRP = GraphicsSettings.renderPipelineAsset;
        
        if (currentRP == null)
        {
            Debug.LogWarning("⚠️ No Render Pipeline Asset assigned! Creating URP Asset...");
            
#if UNITY_EDITOR
            CreateURPAssetInEditor();
#else
            Debug.LogError("❌ URP Asset creation only available in Editor mode!");
#endif
        }
        else if (currentRP is UniversalRenderPipelineAsset)
        {
            Debug.Log("✅ URP Asset already assigned and working!");
            LogURPAssetInfo(currentRP as UniversalRenderPipelineAsset);
        }
        else
        {
            Debug.LogWarning($"⚠️ Non-URP asset detected: {currentRP.GetType().Name}");
#if UNITY_EDITOR
            CreateURPAssetInEditor();
#endif
        }
        
        Debug.Log("=== URP ASSET CREATOR END ===");
    }
    
    private void LogURPAssetInfo(UniversalRenderPipelineAsset urpAsset)
    {
        Debug.Log($"📦 URP Asset Name: {urpAsset.name}");
        Debug.Log($"� Supports HDR: {urpAsset.supportsHDR}");
        Debug.Log($"🎮 MSAA Quality: {urpAsset.msaaSampleCount}");
        Debug.Log($"� Render Scale: {urpAsset.renderScale}");
        Debug.Log($"� Shadow Distance: {urpAsset.shadowDistance}");
        Debug.Log($"🔢 Shadow Cascades: {urpAsset.shadowCascadeCount}");
    }

#if UNITY_EDITOR
    private void CreateURPAssetInEditor()
    {
        Debug.Log("🔧 Creating URP Asset in Editor...");
        
        try
        {
            // URP Asset oluştur
            var urpAsset = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
            
            // Klasör kontrolü ve oluşturma
            string folderPath = "Assets/Settings";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets", "Settings");
                Debug.Log($"📁 Created folder: {folderPath}");
            }
            
            // Asset'i kaydet
            string assetPath = $"{folderPath}/UniversalRenderPipelineAsset.asset";
            AssetDatabase.CreateAsset(urpAsset, assetPath);
            
            // Forward Renderer oluştur
            var forwardRenderer = ScriptableObject.CreateInstance<UniversalRendererData>();
            string rendererPath = $"{folderPath}/ForwardRenderer.asset";
            AssetDatabase.CreateAsset(forwardRenderer, rendererPath);
            
            // Renderer'ı URP Asset'e bağla (URP 13.1.8 için SerializedObject kullan)
            var serializedObject = new SerializedObject(urpAsset);
            var rendererListProperty = serializedObject.FindProperty("m_RendererDataList");
            rendererListProperty.arraySize = 1;
            rendererListProperty.GetArrayElementAtIndex(0).objectReferenceValue = forwardRenderer;
            
            var defaultRendererProperty = serializedObject.FindProperty("m_DefaultRendererIndex");
            defaultRendererProperty.intValue = 0;
            
            serializedObject.ApplyModifiedProperties();
            
            // Optimize ayarlar
            urpAsset.supportsHDR = true;
            urpAsset.msaaSampleCount = 4;
            urpAsset.renderScale = 1.0f;
            
            // Shadow ayarları
            urpAsset.shadowDistance = 150f;
            urpAsset.shadowCascadeCount = 4;
            
            // Advanced ayarlar
            urpAsset.supportsCameraDepthTexture = true;
            urpAsset.supportsCameraOpaqueTexture = true;
            
            // Asset'leri kaydet
            EditorUtility.SetDirty(urpAsset);
            EditorUtility.SetDirty(forwardRenderer);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // Graphics Settings'e ata
            GraphicsSettings.renderPipelineAsset = urpAsset;
            
            // Quality Settings'e de ata
            QualitySettings.renderPipeline = urpAsset;
            
            Debug.Log($"✅ URP Asset created successfully: {assetPath}");
            Debug.Log($"✅ Forward Renderer created: {rendererPath}");
            Debug.Log("✅ Graphics Settings updated");
            Debug.Log("✅ Quality Settings updated");
            
            // Asset'i seç
            Selection.activeObject = urpAsset;
            EditorGUIUtility.PingObject(urpAsset);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error creating URP Asset: {e.Message}");
            Debug.LogException(e);
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(URPAssetCreator))]
public class URPAssetCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        URPAssetCreator creator = (URPAssetCreator)target;
        
        if (GUILayout.Button("🔧 Check & Create URP Asset", GUILayout.Height(30)))
        {
            creator.CheckAndCreateURPAsset();
        }
        
        EditorGUILayout.Space();
        
        // Mevcut durum göstergesi
        RenderPipelineAsset currentRP = GraphicsSettings.renderPipelineAsset;
        if (currentRP == null)
        {
            EditorGUILayout.HelpBox("❌ No Render Pipeline Asset assigned!", MessageType.Error);
        }
        else if (currentRP is UniversalRenderPipelineAsset)
        {
            EditorGUILayout.HelpBox($"✅ URP Asset: {currentRP.name}", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox($"⚠️ Non-URP Asset: {currentRP.GetType().Name}", MessageType.Warning);
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "Bu tool URP Asset eksikse otomatik olarak oluşturur:\n\n" +
            "• UniversalRenderPipelineAsset oluşturur\n" +
            "• ForwardRenderer oluşturur\n" +
            "• Optimal ayarları yapar\n" +
            "• Graphics Settings'e atar\n" +
            "• Quality Settings'i günceller",
            MessageType.Info
        );
    }
}
#endif
