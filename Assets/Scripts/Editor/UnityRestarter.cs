using UnityEngine;
using UnityEditor;

public class UnityRestarter
{
    [MenuItem("Tools/Restart Unity Editor")]
    static void RestartUnity()
    {
        Debug.Log("Unity Editor yeniden başlatılıyor...");
        EditorApplication.OpenProject(System.Environment.CurrentDirectory);
    }
    
    [MenuItem("Tools/Clear Compiler Cache and Restart")]
    static void ClearCacheAndRestart()
    {
        Debug.Log("Compiler cache temizleniyor ve Unity yeniden başlatılıyor...");
        
        // Önce tüm değişiklikleri kaydet
        AssetDatabase.SaveAssets();
        
        // Unity'yi yeniden başlat
        EditorApplication.OpenProject(System.Environment.CurrentDirectory);
    }
    
    [MenuItem("Tools/Reimport All Scripts")]
    static void ReimportAllScripts()
    {
        Debug.Log("Tüm scriptler yeniden import ediliyor...");
        AssetDatabase.ImportAsset("Assets/Scripts", ImportAssetOptions.ImportRecursive);
        AssetDatabase.Refresh();
        Debug.Log("Script import tamamlandı!");
    }
}
