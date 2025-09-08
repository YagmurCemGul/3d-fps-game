using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Reflection;
using UnityEditor;

public class RenderingDiagnostics : MonoBehaviour
{
    [Header("Rendering Diagnostics")]
    [SerializeField] private bool enableDebugLogging = true;
    
    void Start()
    {
        if (enableDebugLogging)
        {
            DiagnoseRenderingIssues();
        }
    }
    
    [ContextMenu("Diagnose Rendering Issues")]
    public void DiagnoseRenderingIssues()
    {
        Debug.Log("=== RENDERING DIAGNOSTICS START ===");
        
        // 1. Render Pipeline kontrolü
        CheckRenderPipeline();
        
        // 2. Material ve Shader kontrolü
        CheckMaterialsAndShaders();
        
        // 3. Lighting kontrolü
        CheckLightingSettings();
        
        // 4. Camera kontrolü
        CheckCameraSettings();
        
        // 5. Package Version kontrolü
        CheckPackageVersions();
        
        Debug.Log("=== RENDERING DIAGNOSTICS END ===");
    }
    
    private void CheckRenderPipeline()
    {
        Debug.Log("--- RENDER PIPELINE CHECK ---");
        
        // Aktif render pipeline'ı kontrol et
        RenderPipelineAsset currentRP = GraphicsSettings.renderPipelineAsset;
        if (currentRP == null)
        {
            Debug.LogError("❌ NO RENDER PIPELINE ASSET ASSIGNED! Using Built-in Pipeline");
            Debug.LogError("🔧 FIX: Go to Edit > Project Settings > Graphics and assign a URP Asset");
        }
        else
        {
            Debug.Log($"✅ Render Pipeline: {currentRP.GetType().Name}");
            Debug.Log($"📦 Asset Name: {currentRP.name}");
            
            if (currentRP is UniversalRenderPipelineAsset urpAsset)
            {
                Debug.Log($"📊 URP Asset detected");
                Debug.Log($"🎯 URP configured properly");
            }
        }
        
        // Quality Settings kontrolü
        QualitySettings.GetQualityLevel();
        Debug.Log($"🎮 Quality Level: {QualitySettings.names[QualitySettings.GetQualityLevel()]}");
    }
    
    private void CheckMaterialsAndShaders()
    {
        Debug.Log("--- MATERIALS & SHADERS CHECK ---");
        
        // Scene'deki tüm renderer'ları kontrol et
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        Debug.Log($"🎨 Found {renderers.Length} renderers in scene");
        
        int materialIssues = 0;
        int shaderIssues = 0;
        
        foreach (Renderer renderer in renderers)
        {
            GameObject obj = renderer.gameObject;
            Material[] materials = renderer.materials;
            
            if (materials == null || materials.Length == 0)
            {
                materialIssues++;
                Debug.LogWarning($"⚠️ {obj.name}: No materials assigned", obj);
                continue;
            }
            
            foreach (Material mat in materials)
            {
                if (mat == null)
                {
                    materialIssues++;
                    Debug.LogWarning($"⚠️ {obj.name}: Null material reference", obj);
                    continue;
                }
                
                Shader shader = mat.shader;
                if (shader == null)
                {
                    shaderIssues++;
                    Debug.LogError($"❌ {obj.name}: Material '{mat.name}' has null shader", obj);
                    continue;
                }
                
                // URP shader kontrolü
                if (shader.name.Contains("Standard") && !shader.name.Contains("Universal"))
                {
                    Debug.LogWarning($"🔄 {obj.name}: Using Built-in shader '{shader.name}' - Consider switching to URP/Lit", obj);
                }
                
                // Shader compilation kontrolü
                if (!shader.isSupported)
                {
                    shaderIssues++;
                    Debug.LogError($"❌ {obj.name}: Shader '{shader.name}' not supported on current platform", obj);
                }
            }
        }
        
        Debug.Log($"📊 Material Issues: {materialIssues}");
        Debug.Log($"📊 Shader Issues: {shaderIssues}");
        
        if (materialIssues == 0 && shaderIssues == 0)
        {
            Debug.Log("✅ No material/shader issues detected");
        }
    }
    
    private void CheckLightingSettings()
    {
        Debug.Log("--- LIGHTING CHECK ---");
        
        // Lighting Settings
        Debug.Log($"🌞 Ambient Mode: {RenderSettings.ambientMode}");
        Debug.Log($"🌈 Ambient Color: {RenderSettings.ambientLight}");
        Debug.Log($"☀️ Sun Source: {(RenderSettings.sun != null ? RenderSettings.sun.name : "None")}");
        
        // Scene'deki ışıkları kontrol et
        Light[] lights = FindObjectsOfType<Light>();
        Debug.Log($"💡 Found {lights.Length} lights in scene");
        
        foreach (Light light in lights)
        {
            Debug.Log($"💡 {light.name}: Type={light.type}, Intensity={light.intensity}, Enabled={light.enabled}");
        }
        
        // Lightmap kontrolü
        if (LightmapSettings.lightmaps?.Length > 0)
        {
            Debug.Log($"🗺️ Lightmaps: {LightmapSettings.lightmaps.Length}");
        }
        else
        {
            Debug.Log("📍 No lightmaps found - using real-time lighting");
        }
    }
    
    private void CheckCameraSettings()
    {
        Debug.Log("--- CAMERA CHECK ---");
        
        Camera[] cameras = FindObjectsOfType<Camera>();
        Debug.Log($"📷 Found {cameras.Length} cameras in scene");
        
        foreach (Camera cam in cameras)
        {
            Debug.Log($"📷 {cam.name}: RenderType={cam.cameraType}, ClearFlags={cam.clearFlags}, CullingMask={cam.cullingMask}");
            
            // URP Additional Camera Data kontrolü
            var additionalCameraData = cam.GetComponent<UniversalAdditionalCameraData>();
            if (additionalCameraData != null)
            {
                Debug.Log($"📷 {cam.name}: URP Renderer Index={additionalCameraData.scriptableRenderer?.GetType().Name ?? "Default"}");
            }
        }
    }
    
    private void CheckPackageVersions()
    {
        Debug.Log("--- PACKAGE VERSIONS CHECK ---");
        
        // Bu bilgiyi runtime'da alamayız, ama Unity versiyonunu kontrol edebiliriz
        Debug.Log($"🎮 Unity Version: {Application.unityVersion}");
        Debug.Log($"📦 Graphics API: {SystemInfo.graphicsDeviceType}");
        Debug.Log($"🎯 Graphics Device: {SystemInfo.graphicsDeviceName}");
        Debug.Log($"💾 Graphics Memory: {SystemInfo.graphicsMemorySize}MB");
        
        // URP version için reflection kullan
        try
        {
            var urpAssembly = System.Reflection.Assembly.GetAssembly(typeof(UniversalRenderPipelineAsset));
            var version = urpAssembly.GetName().Version;
            Debug.Log($"📦 URP Assembly Version: {version}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Could not get URP version: {e.Message}");
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RenderingDiagnostics))]
public class RenderingDiagnosticsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        RenderingDiagnostics diagnostics = (RenderingDiagnostics)target;
        
        if (GUILayout.Button("🔍 Run Diagnostics", GUILayout.Height(30)))
        {
            diagnostics.DiagnoseRenderingIssues();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "Bu tool rendering sorunlarını tespit etmek için kullanılır.\n\n" +
            "• Render Pipeline ayarlarını kontrol eder\n" +
            "• Material ve shader uyumluluğunu test eder\n" +
            "• Lighting ayarlarını inceler\n" +
            "• Camera konfigürasyonunu doğrular\n" +
            "• Package version uyumluluğunu kontrol eder",
            MessageType.Info
        );
    }
}
#endif
