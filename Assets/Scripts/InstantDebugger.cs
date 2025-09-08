using UnityEngine;

public class InstantDebugger : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=================== INSTANT DEBUGGER START ===================");
        
        // WeaponManager kontrolü
        if (WeaponManager.Instance != null)
        {
            Debug.Log("✅ WeaponManager.Instance BULUNDU");
            Debug.Log($"   Pistol Ammo: {WeaponManager.Instance.totalPistolAmmo}");
            Debug.Log($"   Rifle Ammo: {WeaponManager.Instance.totalRifleAmmo}");
            
            if (WeaponManager.Instance.activeWeaponSlot != null)
            {
                Debug.Log("✅ activeWeaponSlot BULUNDU");
                Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
                if (activeWeapon != null)
                {
                    Debug.Log($"✅ Aktif Silah: {activeWeapon.name}");
                    Debug.Log($"   Magazine Size: {activeWeapon.magazineSize}");
                    Debug.Log($"   Bullets Left: {activeWeapon.bulletsLeft}");
                    Debug.Log($"   Is Active: {activeWeapon.isActiveWeapon}");
                }
                else
                {
                    Debug.LogError("❌ Aktif silah bulunamadı!");
                }
            }
            else
            {
                Debug.LogError("❌ activeWeaponSlot NULL!");
            }
        }
        else
        {
            Debug.LogError("❌ WeaponManager.Instance NULL!");
        }
        
        // SoundManager kontrolü
        if (SoundManager.Instance != null)
        {
            Debug.Log("✅ SoundManager.Instance BULUNDU");
            if (SoundManager.Instance.ShootingChannel != null)
            {
                if (SoundManager.Instance.ShootingChannel.isPlaying)
                {
                    Debug.LogError("⚠️ SES ÇALIYOR! Clip: " + SoundManager.Instance.ShootingChannel.clip?.name);
                }
                else
                {
                    Debug.Log("✅ ShootingChannel var ama ses çalmıyor");
                }
            }
        }
        else
        {
            Debug.LogError("❌ SoundManager.Instance NULL!");
        }
        
        // HUDManager kontrolü
        if (HUDManager.Instance != null)
        {
            Debug.Log("✅ HUDManager.Instance BULUNDU");
        }
        else
        {
            Debug.LogError("❌ HUDManager.Instance NULL!");
        }
        
        // AmmoBox'ları bul
        AmmoBox[] ammoBoxes = FindObjectsOfType<AmmoBox>();
        Debug.Log($"Sahnede {ammoBoxes.Length} adet AmmoBox bulundu:");
        foreach (AmmoBox box in ammoBoxes)
        {
            Debug.Log($"   AmmoBox: {box.name}, Tip: {box.ammoType}, Miktar: {box.ammoAmount}");
        }
        
        // InteractionManager kontrolü
        InteractionManager interactionManager = FindObjectOfType<InteractionManager>();
        if (interactionManager != null)
        {
            Debug.Log("✅ InteractionManager BULUNDU");
        }
        else
        {
            Debug.LogError("❌ InteractionManager BULUNAMADI!");
        }
        
        Debug.Log("=================== INSTANT DEBUGGER END ===================");
    }
}
