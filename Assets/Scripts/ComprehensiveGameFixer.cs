using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
#endif

public class ComprehensiveGameFixer : MonoBehaviour
{
    [Header("Comprehensive Game Fixer")]
    [SerializeField] private bool autoFixOnStart = false;
    [SerializeField] private bool enableDebugLogging = true;
    
    void Start()
    {
        if (autoFixOnStart)
        {
            FixAllIssues();
        }
    }
    
    [ContextMenu("Fix All Issues")]
    public void FixAllIssues()
    {
        if (!enableDebugLogging)
        {
            Debug.Log("=== COMPREHENSIVE GAME FIXER START ===");
        }
        
        // 1. Rendering Issues
        FixRenderingIssues();
        
        // 2. Manager Issues
        FixManagerIssues();
        
        // 3. Material Issues
        FixMaterialIssues();
        
        // 4. Audio Issues
        FixAudioIssues();
        
        // 5. UI/HUD Issues
        FixUIIssues();
        
        // 6. Input Issues
        FixInputIssues();
        
        if (!enableDebugLogging)
        {
            Debug.Log("=== COMPREHENSIVE GAME FIXER END ===");
        }
    }
    
    private void FixRenderingIssues()
    {
        if (enableDebugLogging) Debug.Log("--- FIXING RENDERING ISSUES ---");
        
        // URP Asset kontrol et
        RenderPipelineAsset currentRP = GraphicsSettings.renderPipelineAsset;
        if (currentRP == null)
        {
            if (enableDebugLogging) Debug.LogError("❌ No Render Pipeline Asset! Creating URP Asset...");
            
#if UNITY_EDITOR
            CreateDefaultURPAsset();
#endif
        }
        else
        {
            if (enableDebugLogging) Debug.Log($"✅ Render Pipeline: {currentRP.name}");
        }
        
        // Camera'ları kontrol et
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            var additionalCameraData = cam.GetComponent<UniversalAdditionalCameraData>();
            if (additionalCameraData == null)
            {
                cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
                if (enableDebugLogging) Debug.Log($"✅ Added URP Camera Data to {cam.name}");
            }
        }
    }
    
    private void FixManagerIssues()
    {
        if (enableDebugLogging) Debug.Log("--- FIXING MANAGER ISSUES ---");
        
        // WeaponManager kontrolü
        WeaponManager weaponManager = FindObjectOfType<WeaponManager>();
        if (weaponManager == null)
        {
            GameObject managerObj = new GameObject("WeaponManager");
            managerObj.AddComponent<WeaponManager>();
            if (Application.isPlaying)
                DontDestroyOnLoad(managerObj);
            if (enableDebugLogging) Debug.Log("✅ Created WeaponManager");
        }
        
        // SoundManager kontrolü
        SoundManager soundManager = FindObjectOfType<SoundManager>();
        if (soundManager == null)
        {
            GameObject soundObj = new GameObject("SoundManager");
            soundObj.AddComponent<SoundManager>();
            if (Application.isPlaying)
                DontDestroyOnLoad(soundObj);
            if (enableDebugLogging) Debug.Log("✅ Created SoundManager");
        }
        
        // HUDManager kontrolü
        HUDManager hudManager = FindObjectOfType<HUDManager>();
        if (hudManager == null)
        {
            GameObject hudObj = new GameObject("HUDManager");
            hudObj.AddComponent<HUDManager>();
            if (enableDebugLogging) Debug.Log("✅ Created HUDManager");
        }
        
        // InteractionManager kontrolü
        InteractionManager interactionManager = FindObjectOfType<InteractionManager>();
        if (interactionManager == null)
        {
            GameObject interactionObj = new GameObject("InteractionManager");
            interactionObj.AddComponent<InteractionManager>();
            if (enableDebugLogging) Debug.Log("✅ Created InteractionManager");
        }
    }
    
    private void FixMaterialIssues()
    {
        if (enableDebugLogging) Debug.Log("--- FIXING MATERIAL ISSUES ---");
        
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        int fixedMaterials = 0;
        
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            bool needsUpdate = false;
            
            for (int i = 0; i < materials.Length; i++)
            {
                Material mat = materials[i];
                if (mat == null)
                {
                    // Default material ata
                    materials[i] = CreateDefaultMaterial();
                    needsUpdate = true;
                    fixedMaterials++;
                }
                else if (mat.shader == null || !mat.shader.isSupported)
                {
                    // URP/Lit shader'a geç
                    Shader urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
                    if (urpLitShader != null)
                    {
                        mat.shader = urpLitShader;
                        fixedMaterials++;
                    }
                }
                else if (mat.shader.name.Contains("Standard") && !mat.shader.name.Contains("Universal"))
                {
                    // Built-in Standard'dan URP Lit'e geç
                    Shader urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
                    if (urpLitShader != null)
                    {
                        // Texture'ları koru
                        Texture mainTex = mat.mainTexture;
                        Color color = mat.color;
                        
                        mat.shader = urpLitShader;
                        
                        if (mainTex != null)
                            mat.mainTexture = mainTex;
                        mat.color = color;
                        
                        fixedMaterials++;
                    }
                }
            }
            
            if (needsUpdate)
            {
                renderer.materials = materials;
            }
        }
        
        if (enableDebugLogging) Debug.Log($"✅ Fixed {fixedMaterials} material issues");
    }
    
    private void FixAudioIssues()
    {
        if (enableDebugLogging) Debug.Log("--- FIXING AUDIO ISSUES ---");
        
        // AudioListener kontrolü
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        if (listeners.Length == 0)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.gameObject.AddComponent<AudioListener>();
                if (enableDebugLogging) Debug.Log("✅ Added AudioListener to Main Camera");
            }
        }
        else if (listeners.Length > 1)
        {
            // Birden fazla AudioListener varsa, sadece birini bırak
            for (int i = 1; i < listeners.Length; i++)
            {
                DestroyImmediate(listeners[i]);
            }
            if (enableDebugLogging) Debug.Log($"✅ Removed {listeners.Length - 1} duplicate AudioListeners");
        }
        
        // AudioSource'ları kontrol et
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in audioSources)
        {
            if (source.clip == null)
            {
                if (enableDebugLogging) Debug.LogWarning($"⚠️ {source.name}: AudioSource has no clip assigned");
            }
        }
    }
    
    private void FixUIIssues()
    {
        if (enableDebugLogging) Debug.Log("--- FIXING UI ISSUES ---");
        
        // Canvas kontrolü
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // UI Camera ataması varsa kaldır
                if (canvas.worldCamera != null)
                {
                    canvas.worldCamera = null;
                    if (enableDebugLogging) Debug.Log($"✅ Fixed {canvas.name} camera assignment");
                }
            }
            
            // CanvasScaler kontrolü
            var scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvas.gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
                if (enableDebugLogging) Debug.Log($"✅ Added CanvasScaler to {canvas.name}");
            }
        }
        
        // EventSystem kontrolü
        var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventObj = new GameObject("EventSystem");
            eventObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            if (enableDebugLogging) Debug.Log("✅ Created EventSystem");
        }
    }
    
    private void FixInputIssues()
    {
        if (enableDebugLogging) Debug.Log("--- FIXING INPUT ISSUES ---");
        
        // Cursor state'ini kontrol et
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (enableDebugLogging) Debug.Log("✅ Fixed cursor lock state");
        }
        
        // Player controller kontrolü
        var player = FindObjectOfType<FPSPlayerMovement>();
        if (player == null)
        {
            // Eski PlayerMovement'ı ara
            var oldPlayer = GameObject.Find("Player");
            if (oldPlayer != null)
            {
                var oldMovement = oldPlayer.GetComponent<MonoBehaviour>();
                if (oldMovement != null && oldMovement.GetType().Name == "PlayerMovement")
                {
                    if (enableDebugLogging) Debug.LogWarning("⚠️ Found old PlayerMovement script - Consider replacing with FPSPlayerMovement");
                }
            }
        }
    }
    
    private Material CreateDefaultMaterial()
    {
        Material defaultMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        defaultMat.name = "Default URP Material";
        defaultMat.color = Color.white;
        return defaultMat;
    }

#if UNITY_EDITOR
    private void CreateDefaultURPAsset()
    {
        // URPAssetCreator scriptini kullan
        GameObject temp = new GameObject("Temp URP Creator");
        var creator = temp.AddComponent<URPAssetCreator>();
        creator.CheckAndCreateURPAsset();
        DestroyImmediate(temp);
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(ComprehensiveGameFixer))]
public class ComprehensiveGameFixerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        ComprehensiveGameFixer fixer = (ComprehensiveGameFixer)target;
        
        if (GUILayout.Button("🔧 Fix All Issues", GUILayout.Height(40)))
        {
            fixer.FixAllIssues();
        }
        
        EditorGUILayout.Space();
        
        // Individual fix buttons
        EditorGUILayout.LabelField("Individual Fixes:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🎨 Fix Rendering"))
        {
            fixer.GetType().GetMethod("FixRenderingIssues", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(fixer, null);
        }
        if (GUILayout.Button("⚙️ Fix Managers"))
        {
            fixer.GetType().GetMethod("FixManagerIssues", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(fixer, null);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🎭 Fix Materials"))
        {
            fixer.GetType().GetMethod("FixMaterialIssues", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(fixer, null);
        }
        if (GUILayout.Button("🔊 Fix Audio"))
        {
            fixer.GetType().GetMethod("FixAudioIssues", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(fixer, null);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🖥️ Fix UI"))
        {
            fixer.GetType().GetMethod("FixUIIssues", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(fixer, null);
        }
        if (GUILayout.Button("🎮 Fix Input"))
        {
            fixer.GetType().GetMethod("FixInputIssues", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(fixer, null);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "Bu tool tüm oyun sorunlarını otomatik olarak çözer:\n\n" +
            "🎨 Rendering: URP Asset, Camera Data\n" +
            "⚙️ Managers: WeaponManager, SoundManager, HUDManager\n" +
            "🎭 Materials: URP Shader conversion, null checks\n" +
            "🔊 Audio: AudioListener, AudioSource validation\n" +
            "🖥️ UI: Canvas, EventSystem, CanvasScaler\n" +
            "🎮 Input: Cursor lock, PlayerMovement validation",
            MessageType.Info
        );
    }
}
#endif
