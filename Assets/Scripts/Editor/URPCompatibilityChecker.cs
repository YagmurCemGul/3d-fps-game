using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class URPCompatibilityChecker
{
    [MenuItem("Tools/Check URP Compatibility")]
    static void CheckURPCompatibility()
    {
        Debug.Log("🔍 URP Uyumluluk Kontrolü Başlıyor...");
        
        // Unity versiyonunu kontrol et
        var unityVersion = Application.unityVersion;
        Debug.Log($"📱 Unity Versiyonu: {unityVersion}");
        
        // URP Asset'ini kontrol et
        var urpAsset = GraphicsSettings.renderPipelineAsset;
        if (urpAsset != null)
        {
            Debug.Log($"🎨 URP Asset Bulundu: {urpAsset.name}");
            
            if (urpAsset is UniversalRenderPipelineAsset)
            {
                var universalAsset = urpAsset as UniversalRenderPipelineAsset;
                Debug.Log("✅ Universal Render Pipeline aktif");
                
                // URP versiyonunu kontrol et
                var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.render-pipelines.universal");
                if (packageInfo != null)
                {
                    Debug.Log($"📦 URP Paket Versiyonu: {packageInfo.version}");
                    
                    // Unity 2022.3 için doğru versiyon kontrolü
                    if (unityVersion.Contains("2022.3"))
                    {
                        if (packageInfo.version.StartsWith("14."))
                        {
                            Debug.Log("✅ URP versiyonu Unity 2022.3 ile uyumlu (14.x)");
                        }
                        else
                        {
                            Debug.LogWarning($"⚠️ URP versiyonu uyumsuz! Unity 2022.3 için URP 14.x gerekli, mevcut: {packageInfo.version}");
                        }
                    }
                    else if (unityVersion.Contains("2022.1"))
                    {
                        if (packageInfo.version.StartsWith("13."))
                        {
                            Debug.Log("✅ URP versiyonu Unity 2022.1 ile uyumlu (13.x)");
                        }
                        else
                        {
                            Debug.LogWarning($"⚠️ URP versiyonu uyumsuz! Unity 2022.1 için URP 13.x gerekli, mevcut: {packageInfo.version}");
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("⚠️ Universal Render Pipeline aktif değil!");
            }
        }
        else
        {
            Debug.LogError("❌ Render Pipeline Asset bulunamadı!");
        }
        
        Debug.Log("🔍 URP Uyumluluk Kontrolü Tamamlandı!");
    }
    
    [MenuItem("Tools/Fix URP Settings")]
    static void FixURPSettings()
    {
        Debug.Log("🔧 URP Ayarları Düzeltiliyor...");
        
        // URP Asset'ini bul
        var urpAssetGuids = AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset");
        
        if (urpAssetGuids.Length > 0)
        {
            var urpAssetPath = AssetDatabase.GUIDToAssetPath(urpAssetGuids[0]);
            var urpAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(urpAssetPath);
            
            if (urpAsset != null)
            {
                // Graphics Settings'e ata
                GraphicsSettings.renderPipelineAsset = urpAsset;
                
                // Quality Settings'e de ata
                var qualityLevels = QualitySettings.names;
                for (int i = 0; i < qualityLevels.Length; i++)
                {
                    QualitySettings.SetQualityLevel(i);
                    QualitySettings.renderPipeline = urpAsset;
                }
                
                Debug.Log($"✅ URP Asset atandı: {urpAsset.name}");
                Debug.Log("✅ Tüm Quality Level'lara URP atandı");
                
                // Ayarları kaydet
                AssetDatabase.SaveAssets();
                EditorUtility.SetDirty(urpAsset);
                
                Debug.Log("💾 URP ayarları kaydedildi!");
            }
        }
        else
        {
            Debug.LogError("❌ URP Asset bulunamadı! Yeni bir tane oluşturun.");
        }
        
        Debug.Log("🔧 URP Ayarları Düzeltme Tamamlandı!");
    }
    
    [MenuItem("Tools/Create URP Asset")]
    static void CreateURPAsset()
    {
        Debug.Log("🎨 Yeni URP Asset Oluşturuluyor...");
        
        // URP Asset oluştur
        var urpAsset = UniversalRenderPipelineAsset.Create();
        
        // Settings klasörüne kaydet
        var settingsPath = "Assets/Settings";
        if (!AssetDatabase.IsValidFolder(settingsPath))
        {
            AssetDatabase.CreateFolder("Assets", "Settings");
        }
        
        var assetPath = $"{settingsPath}/UniversalRenderPipelineAsset.asset";
        AssetDatabase.CreateAsset(urpAsset, assetPath);
        
        // Graphics Settings'e ata
        GraphicsSettings.renderPipelineAsset = urpAsset;
        
        Debug.Log($"✅ URP Asset oluşturuldu: {assetPath}");
        Debug.Log("✅ Graphics Settings'e atandı");
        
        // Ayarları kaydet
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("🎨 URP Asset Oluşturma Tamamlandı!");
    }
}
