using UnityEngine;
using UnityEditor;

public class UnityFileSystemFixer
{
    [MenuItem("Tools/Fix Unity File System")]
    static void FixUnityFileSystem()
    {
        Debug.Log("🔧 Unity File System düzeltiliyor...");
        
        // Temp klasörünü oluştur
        CreateDirectoryIfNotExists("Temp");
        CreateDirectoryIfNotExists("Temp/Bin");
        CreateDirectoryIfNotExists("Temp/CompilationPipeline");
        CreateDirectoryIfNotExists("Temp/FSTimeGet");
        
        // Library klasörlerini oluştur
        CreateDirectoryIfNotExists("Library");
        CreateDirectoryIfNotExists("Library/ScriptAssemblies");
        CreateDirectoryIfNotExists("Library/StateCache");
        CreateDirectoryIfNotExists("Library/PlayerDataCache");
        CreateDirectoryIfNotExists("Library/BuildPlayerData");
        
        // FSTimeGet dosyasını oluştur (Unity'nin ihtiyaç duyduğu)
        var fsTimeGetFile = "Temp/FSTimeGet-a6ed4119d3d0a4aaf84af93ecc30aa90";
        if (!System.IO.File.Exists(fsTimeGetFile))
        {
            System.IO.File.WriteAllText(fsTimeGetFile, System.DateTime.UtcNow.Ticks.ToString());
            Debug.Log($"📄 FSTimeGet dosyası oluşturuldu: {fsTimeGetFile}");
        }
        
        // AssetDatabase refresh tetikleyici oluştur
        var refreshFile = "Library/AssetDatabase.refresh";
        if (!System.IO.File.Exists(refreshFile))
        {
            System.IO.File.WriteAllText(refreshFile, "refresh");
            Debug.Log("📄 AssetDatabase refresh tetikleyici oluşturuldu");
        }
        
        Debug.Log("✅ Unity File System düzeltme tamamlandı!");
        
        // Asset Database'i yenile
        AssetDatabase.Refresh();
    }
    
    static void CreateDirectoryIfNotExists(string path)
    {
        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
            Debug.Log($"📁 Klasör oluşturuldu: {path}");
        }
    }
}
