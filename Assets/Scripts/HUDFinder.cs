using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDFinder : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🔍 Mevcut HUD elementleri aranıyor...");
        
        // Tüm TextMeshPro elementlerini bul
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
        Debug.Log($"Bulunan TextMeshPro sayısı: {allTexts.Length}");
        
        foreach (TextMeshProUGUI text in allTexts)
        {
            Debug.Log($"Text bulundu: {text.name}, Parent: {text.transform.parent?.name}, Text: '{text.text}'");
        }
        
        // Tüm Text (legacy) elementlerini bul
        Text[] allLegacyTexts = FindObjectsOfType<Text>();
        Debug.Log($"Bulunan Legacy Text sayısı: {allLegacyTexts.Length}");
        
        foreach (Text text in allLegacyTexts)
        {
            Debug.Log($"Legacy Text bulundu: {text.name}, Parent: {text.transform.parent?.name}, Text: '{text.text}'");
        }
        
        // Canvas'ları listele
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        Debug.Log($"Bulunan Canvas sayısı: {canvases.Length}");
        
        foreach (Canvas canvas in canvases)
        {
            Debug.Log($"Canvas bulundu: {canvas.name}, Child count: {canvas.transform.childCount}");
            
            // Canvas'ın child'larını listele
            for (int i = 0; i < canvas.transform.childCount; i++)
            {
                Transform child = canvas.transform.GetChild(i);
                Debug.Log($"  Canvas child: {child.name}");
            }
        }
        
        // HUDManager instance'ı var mı?
        if (HUDManager.Instance != null)
        {
            Debug.Log("✅ HUDManager.Instance bulundu!");
            Debug.Log($"  magazineAmmoUI: {HUDManager.Instance.magazineAmmoUI?.name ?? "NULL"}");
            Debug.Log($"  totalAmmoUI: {HUDManager.Instance.totalAmmoUI?.name ?? "NULL"}");
        }
        else
        {
            Debug.LogWarning("❌ HUDManager.Instance NULL!");
        }
    }
}
