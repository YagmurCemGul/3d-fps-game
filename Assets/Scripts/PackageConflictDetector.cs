using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using System.Linq;
#endif

public class PackageConflictDetector : MonoBehaviour
{
    [Header("Package Conflict Detector")]
    [SerializeField] private bool enableDebugLogging = true;
    
    void Start()
    {
        if (enableDebugLogging)
        {
            DetectPackageConflicts();
        }
    }
    
    [ContextMenu("Detect Package Conflicts")]
    public void DetectPackageConflicts()
    {
        Debug.Log("=== PACKAGE CONFLICT DETECTION START ===");
        
        // Unity versiyonu kontrolü
        CheckUnityVersion();
        
        // Common conflict'leri kontrol et
        CheckCommonConflicts();
        
        // URP version conflicts
        CheckURPVersionConflicts();
        
        // Sample package conflicts
        CheckSamplePackageConflicts();
        
#if UNITY_EDITOR
        // Editor'da package listesi al
        CheckInstalledPackages();
#endif
        
        Debug.Log("=== PACKAGE CONFLICT DETECTION END ===");
    }
    
    private void CheckUnityVersion()
    {
        Debug.Log("--- UNITY VERSION CHECK ---");
        
        string unityVersion = Application.unityVersion;
        Debug.Log($"🎮 Unity Version: {unityVersion}");
        
        // Unity 2022.3 için önerilen package versions
        Dictionary<string, string> recommendedVersions = new Dictionary<string, string>
        {
            {"com.unity.render-pipelines.universal", "14.0.8"},
            {"com.unity.render-pipelines.core", "14.0.8"},
            {"com.unity.shadergraph", "14.0.8"},
            {"com.unity.cinemachine", "2.9.7"},
            {"com.unity.visualscripting", "1.8.0"},
            {"com.unity.textmeshpro", "3.0.6"},
            {"com.unity.timeline", "1.7.4"},
            {"com.unity.probuilder", "5.0.6"},
            {"com.unity.ide.visualstudio", "2.0.18"},
            {"com.unity.collab-proxy", "2.0.4"}
        };
        
        if (unityVersion.StartsWith("2022.3"))
        {
            Debug.Log("✅ Unity 2022.3 detected - Using LTS version");
            
            foreach (var kvp in recommendedVersions)
            {
                Debug.Log($"📦 Recommended {kvp.Key}: {kvp.Value}");
            }
        }
        else if (unityVersion.StartsWith("2022.1"))
        {
            Debug.LogWarning("⚠️ Unity 2022.1 detected - Consider upgrading to 2022.3 LTS");
        }
        else if (unityVersion.StartsWith("2021"))
        {
            Debug.LogWarning("⚠️ Unity 2021 detected - URP 12.x series required");
        }
        else
        {
            Debug.LogError($"❌ Unsupported Unity version: {unityVersion}");
        }
    }
    
    private void CheckCommonConflicts()
    {
        Debug.Log("--- COMMON CONFLICTS CHECK ---");
        
        // 1. Built-in vs URP conflict
        RenderPipelineAsset currentRP = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset;
        if (currentRP == null)
        {
            Debug.LogError("❌ CRITICAL: No Render Pipeline assigned - This causes major rendering issues!");
            Debug.LogError("🔧 FIX: Use URPAssetCreator to create URP Asset");
        }
        
        // 2. Multiple Input Systems
        bool newInputSystem = false;
        bool oldInputSystem = false;
        
        try
        {
            // Check for new Input System
            var inputSystemType = System.Type.GetType("UnityEngine.InputSystem.InputSystem, Unity.InputSystem");
            newInputSystem = inputSystemType != null;
            
            // Check for old Input Manager
            oldInputSystem = true; // Her zaman mevcut
            
            Debug.Log($"🎮 New Input System: {(newInputSystem ? "✅ Present" : "❌ Missing")}");
            Debug.Log($"🎮 Old Input Manager: {(oldInputSystem ? "✅ Present" : "❌ Missing")}");
            
            if (newInputSystem && oldInputSystem)
            {
                Debug.LogWarning("⚠️ Both Input Systems active - May cause conflicts");
                Debug.LogWarning("🔧 FIX: Go to Edit > Project Settings > Player > Other Settings > Active Input Handling");
            }
        }
        catch
        {
            Debug.LogWarning("⚠️ Could not detect Input System configuration");
        }
        
        // 3. TextMeshPro conflicts
        var tmpType = System.Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
        if (tmpType == null)
        {
            Debug.LogWarning("⚠️ TextMeshPro not found - UI text may not work");
        }
        else
        {
            Debug.Log("✅ TextMeshPro available");
        }
    }
    
    private void CheckURPVersionConflicts()
    {
        Debug.Log("--- URP VERSION CONFLICTS ---");
        
        try
        {
            var urpAssembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset));
            var version = urpAssembly.GetName().Version;
            Debug.Log($"📦 URP Assembly Version: {version}");
            
            // Unity 2022.3 için URP 14.x gerekli
            if (Application.unityVersion.StartsWith("2022.3"))
            {
                if (version.Major < 14)
                {
                    Debug.LogError($"❌ URP Version Conflict! Unity 2022.3 requires URP 14.x, but found URP {version.Major}.x");
                    Debug.LogError("🔧 FIX: Update URP package to 14.0.8 or newer");
                }
                else
                {
                    Debug.Log($"✅ URP version {version.Major}.x compatible with Unity 2022.3");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Could not check URP version: {e.Message}");
        }
    }
    
    private void CheckSamplePackageConflicts()
    {
        Debug.Log("--- SAMPLE PACKAGE CONFLICTS ---");
        
        // Sample package'ları kontrol et
        List<string> potentialConflicts = new List<string>();
        
        // URP Samples conflict
        if (System.IO.Directory.Exists("Assets/Samples/Universal RP"))
        {
            potentialConflicts.Add("URP Samples - May contain old shaders/materials");
        }
        
        // Cinemachine Samples conflict
        if (System.IO.Directory.Exists("Assets/Samples/Cinemachine"))
        {
            potentialConflicts.Add("Cinemachine Samples - May contain conflicting scripts");
        }
        
        // Timeline Samples conflict
        if (System.IO.Directory.Exists("Assets/Samples/Timeline"))
        {
            potentialConflicts.Add("Timeline Samples - May contain old timeline assets");
        }
        
        if (potentialConflicts.Count > 0)
        {
            Debug.LogWarning($"⚠️ Found {potentialConflicts.Count} potential sample conflicts:");
            foreach (string conflict in potentialConflicts)
            {
                Debug.LogWarning($"  📦 {conflict}");
            }
            Debug.LogWarning("🔧 FIX: Consider removing old sample folders if experiencing issues");
        }
        else
        {
            Debug.Log("✅ No sample package conflicts detected");
        }
    }

#if UNITY_EDITOR
    private ListRequest packageListRequest;
    
    private void CheckInstalledPackages()
    {
        Debug.Log("--- INSTALLED PACKAGES CHECK ---");
        
        // Package Manager'dan liste al
        packageListRequest = Client.List(true);
        EditorApplication.update += CheckPackageListProgress;
    }
    
    private void CheckPackageListProgress()
    {
        if (packageListRequest.IsCompleted)
        {
            EditorApplication.update -= CheckPackageListProgress;
            
            if (packageListRequest.Status == StatusCode.Success)
            {
                Debug.Log($"📦 Found {packageListRequest.Result.Count()} packages");
                
                var problematicPackages = new Dictionary<string, string>();
                
                foreach (var package in packageListRequest.Result)
                {
                    // URP version check
                    if (package.name == "com.unity.render-pipelines.universal")
                    {
                        if (Application.unityVersion.StartsWith("2022.3") && !package.version.StartsWith("14."))
                        {
                            problematicPackages.Add(package.name, $"Version {package.version} incompatible with Unity 2022.3 (need 14.x)");
                        }
                    }
                    
                    // Cinemachine version check
                    if (package.name == "com.unity.cinemachine")
                    {
                        if (Application.unityVersion.StartsWith("2022.3") && !package.version.StartsWith("2.9"))
                        {
                            problematicPackages.Add(package.name, $"Version {package.version} may be outdated for Unity 2022.3 (recommend 2.9.x)");
                        }
                    }
                    
                    // Visual Scripting version check
                    if (package.name == "com.unity.visualscripting")
                    {
                        if (Application.unityVersion.StartsWith("2022.3") && !package.version.StartsWith("1.8"))
                        {
                            problematicPackages.Add(package.name, $"Version {package.version} may be outdated for Unity 2022.3 (recommend 1.8.x)");
                        }
                    }
                }
                
                if (problematicPackages.Count > 0)
                {
                    Debug.LogError($"❌ Found {problematicPackages.Count} problematic packages:");
                    foreach (var kvp in problematicPackages)
                    {
                        Debug.LogError($"  📦 {kvp.Key}: {kvp.Value}");
                    }
                }
                else
                {
                    Debug.Log("✅ All packages appear to be compatible");
                }
            }
            else
            {
                Debug.LogError($"❌ Failed to get package list: {packageListRequest.Error.message}");
            }
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(PackageConflictDetector))]
public class PackageConflictDetectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        PackageConflictDetector detector = (PackageConflictDetector)target;
        
        if (GUILayout.Button("🔍 Detect Package Conflicts", GUILayout.Height(30)))
        {
            detector.DetectPackageConflicts();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "Bu tool package çakışmalarını tespit eder:\n\n" +
            "• Unity version uyumluluğu\n" +
            "• URP version conflicts\n" +
            "• Input System conflicts\n" +
            "• Sample package problems\n" +
            "• Render pipeline issues",
            MessageType.Info
        );
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("🔧 Quick Fix: Update Packages"))
        {
            Debug.Log("🔧 Opening Package Manager for manual updates...");
            EditorApplication.ExecuteMenuItem("Window/Package Manager");
        }
    }
}
#endif
