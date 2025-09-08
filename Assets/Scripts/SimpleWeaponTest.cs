using UnityEngine;

public class SimpleWeaponTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("=== WEAPON TEST ===");
            
            // Aktif silahları bul
            Weapon[] weapons = FindObjectsOfType<Weapon>();
            Debug.Log($"Sahnede {weapons.Length} adet Weapon bulundu");
            
            foreach (Weapon weapon in weapons)
            {
                Debug.Log($"Weapon: {weapon.name}");
                Debug.Log($"  Active: {weapon.isActiveWeapon}");
                Debug.Log($"  Magazine Size: {weapon.magazineSize}");
                Debug.Log($"  Bullets Left: {weapon.bulletsLeft}");
                Debug.Log($"  Ready to Shoot: {weapon.readyToShoot}");
                Debug.Log($"  Weapon Model: {weapon.thisWeaponModel}");
            }
            
            Debug.Log("=== TEST BİTTİ ===");
        }
        
        // Basit ateş etme testi
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("=== ATEŞ ETME TEST ===");
            
            Weapon[] weapons = FindObjectsOfType<Weapon>();
            foreach (Weapon weapon in weapons)
            {
                if (weapon.isActiveWeapon)
                {
                    Debug.Log($"Aktif silah: {weapon.name}, Mermi: {weapon.bulletsLeft}");
                    
                    if (weapon.bulletsLeft > 0)
                    {
                        Debug.Log("✅ Mermi var, ateş edebilir");
                    }
                    else
                    {
                        Debug.Log("❌ Mermi yok, ateş edemez");
                    }
                }
            }
            
            Debug.Log("=== TEST BİTTİ ===");
        }
    }
}
