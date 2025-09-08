using UnityEngine;

public class SimpleAmmoTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("=== AMMO TEST ===");
            
            // WeaponManager kontrolü
            if (WeaponManager.Instance != null)
            {
                Debug.Log("✅ WeaponManager.Instance bulundu");
                Debug.Log($"Pistol Ammo: {WeaponManager.Instance.totalPistolAmmo}");
                Debug.Log($"Rifle Ammo: {WeaponManager.Instance.totalRifleAmmo}");
            }
            else
            {
                Debug.LogError("❌ WeaponManager.Instance NULL!");
            }
            
            // SoundManager kontrolü
            if (SoundManager.Instance != null)
            {
                Debug.Log("✅ SoundManager.Instance bulundu");
            }
            else
            {
                Debug.LogError("❌ SoundManager.Instance NULL!");
            }
            
            // AmmoBox'ları bul
            AmmoBox[] ammoBoxes = FindObjectsOfType<AmmoBox>();
            Debug.Log($"Sahnede {ammoBoxes.Length} adet AmmoBox bulundu");
            
            foreach (AmmoBox box in ammoBoxes)
            {
                Debug.Log($"AmmoBox: {box.name}, Tip: {box.ammoType}, Miktar: {box.ammoAmount}");
            }
            
            Debug.Log("=== TEST BİTTİ ===");
        }
    }
}
