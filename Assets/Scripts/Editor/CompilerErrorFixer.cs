using UnityEngine;
using UnityEditor;

public class CompilerErrorFixer
{
    [MenuItem("Tools/Force Recompile All Scripts")]
    static void ForceRecompileAll()
    {
        Debug.Log("🔄 Tüm scriptler zorla yeniden derleniyor...");
        
        // 1. Önce tüm değişiklikleri kaydet
        AssetDatabase.SaveAssets();
        
        // 2. Tüm import işlemlerini durdur
        AssetDatabase.StopAssetEditing();
        
        // 3. Script compile'ını zorla başlat
        AssetDatabase.StartAssetEditing();
        
        // 4. Scripts klasörünü yeniden import et
        AssetDatabase.ImportAsset("Assets/Scripts", ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);
        
        // 5. Tüm asset'leri yenile
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        
        // 6. Asset editing'i bitir
        AssetDatabase.StopAssetEditing();
        
        // 7. Son bir refresh daha
        AssetDatabase.Refresh();
        
        Debug.Log("✅ Script yeniden derleme tamamlandı!");
    }
    
    [MenuItem("Tools/Fix Compiler Errors")]
    static void FixCompilerErrors()
    {
        Debug.Log("🔧 Compiler hataları düzeltiliyor...");
        
        // Temp klasörünün varlığını kontrol et ve oluştur
        var tempPath = "Temp";
        if (!System.IO.Directory.Exists(tempPath))
        {
            System.IO.Directory.CreateDirectory(tempPath);
            Debug.Log("📁 Temp klasörü oluşturuldu");
        }
        
        // Library cache'ini temizle
        var libraryPath = "Library";
        if (System.IO.Directory.Exists(libraryPath))
        {
            try
            {
                // ScriptAssemblies klasörünü sil ve yeniden oluştur
                var scriptAssembliesPath = System.IO.Path.Combine(libraryPath, "ScriptAssemblies");
                if (System.IO.Directory.Exists(scriptAssembliesPath))
                {
                    System.IO.Directory.Delete(scriptAssembliesPath, true);
                }
                System.IO.Directory.CreateDirectory(scriptAssembliesPath);
                Debug.Log("🗑️ ScriptAssemblies klasörü yenilendi");
                
                // StateCache klasörünü sil ve yeniden oluştur
                var stateCachePath = System.IO.Path.Combine(libraryPath, "StateCache");
                if (System.IO.Directory.Exists(stateCachePath))
                {
                    System.IO.Directory.Delete(stateCachePath, true);
                }
                System.IO.Directory.CreateDirectory(stateCachePath);
                Debug.Log("🗑️ StateCache klasörü yenilendi");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Cache temizleme hatası: {e.Message}");
            }
        }
        
        // Scriptleri yeniden derle
        ForceRecompileAll();
        
        Debug.Log("✅ Compiler hata düzeltme işlemi tamamlandı!");
    }
}
