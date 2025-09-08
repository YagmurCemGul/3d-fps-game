using UnityEngine;

public class GameStartMaterialFixer : MonoBehaviour
{
    [Header("Material Fix Settings")]
    public bool autoFixOnStart = true;
    public bool showFixMessage = true;
    
    void Start()
    {
        if (autoFixOnStart)
        {
            Invoke("FixMaterials", 0.5f); // Biraz gecikme ile çalıştır
        }
    }
    
    public void FixMaterials()
    {
        if (showFixMessage)
            Debug.Log("GameStartMaterialFixer: Material sorunu düzeltiliyor...");
        
        // AdvancedMaterialManager oluştur veya bul
        AdvancedMaterialManager materialManager = FindObjectOfType<AdvancedMaterialManager>();
        
        if (materialManager == null)
        {
            GameObject managerObj = new GameObject("AdvancedMaterialManager");
            materialManager = managerObj.AddComponent<AdvancedMaterialManager>();
            DontDestroyOnLoad(managerObj);
        }
        
        // Material düzeltmesini çalıştır
        materialManager.SetupAdvancedMaterials();
        
        if (showFixMessage)
            Debug.Log("GameStartMaterialFixer: Material sorunu düzeltildi! Duvarlar ve nesneler artık görünür olmalı.");
    }
    
    void Update()
    {
        // F9 tuşuna basıldığında materialleri yeniden düzelt
        if (Input.GetKeyDown(KeyCode.F9))
        {
            FixMaterials();
            Debug.Log("F9 ile materialler yeniden düzeltildi!");
        }
    }
}

// Bu scripti otomatik olarak sahneye ekleyen script
[System.Serializable]
public static class AutoMaterialFixerCreator
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void CreateMaterialFixer()
    {
        GameObject materialFixerObj = new GameObject("GameStartMaterialFixer");
        materialFixerObj.AddComponent<GameStartMaterialFixer>();
        Object.DontDestroyOnLoad(materialFixerObj);
        
        Debug.Log("AutoMaterialFixerCreator: GameStartMaterialFixer otomatik olarak oluşturuldu.");
    }
}
