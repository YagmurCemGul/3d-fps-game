using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDAutoSetup : MonoBehaviour
{
    void Start()
    {
        // HUDManager yoksa Canvas'ta oluştur
        if (HUDManager.Instance == null)
        {
            Debug.Log("HUDManager bulunamadı, Canvas'ta aranıyor...");
            
            // Canvas'ı bul
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            Canvas gameCanvas = null;
            
            foreach (Canvas canvas in canvases)
            {
                if (canvas.gameObject.name.Contains("UI") || 
                    canvas.gameObject.name.Contains("HUD") || 
                    canvas.gameObject.name.Contains("Canvas"))
                {
                    gameCanvas = canvas;
                    break;
                }
            }
            
            if (gameCanvas == null && canvases.Length > 0)
            {
                gameCanvas = canvases[0]; // İlk Canvas'ı kullan
            }
            
            if (gameCanvas != null)
            {
                Debug.Log($"HUDManager {gameCanvas.name} Canvas'ına ekleniyor...");
                
                // HUDManager component'ini ekle
                HUDManager hudManager = gameCanvas.gameObject.AddComponent<HUDManager>();
                
                // UI elementlerini otomatik bul ve ata
                AutoAssignUIElements(hudManager);
                
                Debug.Log("HUDManager otomatik kuruldu!");
            }
            else
            {
                Debug.LogError("Canvas bulunamadı! UI için Canvas oluşturun.");
            }
        }
    }
    
    private void AutoAssignUIElements(HUDManager hudManager)
    {
        // TextMeshPro elementlerini bul
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
        
        foreach (TextMeshProUGUI text in allTexts)
        {
            string name = text.name.ToLower();
            
            if (name.Contains("magazine") && name.Contains("ammo"))
            {
                hudManager.magazineAmmoUI = text;
                Debug.Log($"magazineAmmoUI atandı: {text.name}");
            }
            else if (name.Contains("total") && name.Contains("ammo"))
            {
                hudManager.totalAmmoUI = text;
                Debug.Log($"totalAmmoUI atandı: {text.name}");
            }
        }
        
        // Image elementlerini bul
        Image[] allImages = FindObjectsOfType<Image>();
        
        foreach (Image image in allImages)
        {
            string name = image.name.ToLower();
            
            if (name.Contains("ammo") && name.Contains("type"))
            {
                hudManager.ammoTypeUI = image;
                Debug.Log($"ammoTypeUI atandı: {image.name}");
            }
            else if (name.Contains("active") && name.Contains("weapon"))
            {
                hudManager.activeWeaponUI = image;
                Debug.Log($"activeWeaponUI atandı: {image.name}");
            }
            else if (name.Contains("inactive") || (name.Contains("weapon") && name.Contains("2")))
            {
                hudManager.UnActiveWeaponUI = image;
                Debug.Log($"UnActiveWeaponUI atandı: {image.name}");
            }
        }
        
        // Middle dot'u bul
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            string name = obj.name.ToLower();
            if (name.Contains("middle") || name.Contains("dot") || name.Contains("crosshair"))
            {
                hudManager.middleDot = obj;
                Debug.Log($"middleDot atandı: {obj.name}");
                break;
            }
        }
        
        Debug.Log("HUDManager UI element atamaları tamamlandı.");
    }
}
