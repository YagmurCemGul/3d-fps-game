using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

public class PackageUpdater
{
    static AddRequest addRequest;
    static RemoveRequest removeRequest;
    
    [MenuItem("Tools/Update URP to 14.x")]
    static void UpdateURPTo14()
    {
        Debug.Log("📦 URP 14.x'e güncelleniyor...");
        
        // Önce eski URP'yi kaldır
        removeRequest = Client.Remove("com.unity.render-pipelines.universal");
        EditorApplication.update += RemoveProgress;
    }
    
    static void RemoveProgress()
    {
        if (removeRequest.IsCompleted)
        {
            EditorApplication.update -= RemoveProgress;
            
            if (removeRequest.Status == StatusCode.Success)
            {
                Debug.Log("✅ Eski URP kaldırıldı, yeni versiyon yükleniyor...");
                
                // Yeni URP versiyonunu yükle
                addRequest = Client.Add("com.unity.render-pipelines.universal@14.0.11");
                EditorApplication.update += AddProgress;
            }
            else
            {
                Debug.LogError($"❌ URP kaldırma hatası: {removeRequest.Error.message}");
            }
        }
    }
    
    static void AddProgress()
    {
        if (addRequest.IsCompleted)
        {
            EditorApplication.update -= AddProgress;
            
            if (addRequest.Status == StatusCode.Success)
            {
                Debug.Log("✅ URP 14.x başarıyla yüklendi!");
                Debug.Log("🔄 Unity'yi yeniden başlatmanız önerilir.");
                
                // Package Manager'ı yenile
                Client.Resolve();
            }
            else
            {
                Debug.LogError($"❌ URP yükleme hatası: {addRequest.Error.message}");
            }
        }
    }
    
    [MenuItem("Tools/Refresh Package Manager")]
    static void RefreshPackageManager()
    {
        Debug.Log("🔄 Package Manager yenileniyor...");
        Client.Resolve();
        AssetDatabase.Refresh();
        Debug.Log("✅ Package Manager yenilendi!");
    }
    
    [MenuItem("Tools/Check Package Versions")]
    static void CheckPackageVersions()
    {
        Debug.Log("📋 Paket versiyonları kontrol ediliyor...");
        
        var listRequest = Client.List(true, false);
        
        EditorApplication.update += () =>
        {
            if (listRequest.IsCompleted)
            {
                EditorApplication.update -= CheckPackageVersions;
                
                if (listRequest.Status == StatusCode.Success)
                {
                    foreach (var package in listRequest.Result)
                    {
                        if (package.name.Contains("render-pipelines") || 
                            package.name.Contains("cinemachine") ||
                            package.name.Contains("textmeshpro") ||
                            package.name.Contains("timeline"))
                        {
                            Debug.Log($"📦 {package.name}: {package.version}");
                        }
                    }
                }
                else
                {
                    Debug.LogError($"❌ Paket listesi alınamadı: {listRequest.Error.message}");
                }
            }
        };
    }
}
