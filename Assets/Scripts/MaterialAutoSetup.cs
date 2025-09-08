using UnityEngine;

public class MaterialAutoSetup : MonoBehaviour
{
    void Start()
    {
        SetupMaterials();
    }
    
    [ContextMenu("Setup Materials")]
    public void SetupMaterials()
    {
        Debug.Log("MaterialAutoSetup: Material kurulumu başlatılıyor...");
        
        // MaterialAssignmentManager oluştur veya bul
        MaterialAssignmentManager materialManager = FindObjectOfType<MaterialAssignmentManager>();
        
        if (materialManager == null)
        {
            GameObject managerObj = new GameObject("MaterialAssignmentManager");
            materialManager = managerObj.AddComponent<MaterialAssignmentManager>();
            DontDestroyOnLoad(managerObj);
            Debug.Log("MaterialAssignmentManager oluşturuldu.");
        }
        
        // Default materialleri oluştur
        materialManager.CreateDefaultMaterials();
        
        // Materialleri otomatik ata
        materialManager.AutoAssignMaterials();
        
        Debug.Log("MaterialAutoSetup: Material kurulumu tamamlandı!");
    }
}
