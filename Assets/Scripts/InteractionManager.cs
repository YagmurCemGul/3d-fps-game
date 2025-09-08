using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public Weapon hoveredWeapon = null;

    public AmmoBox hoveredAmmoBox = null;
    public static InteractionManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Debug.Log("🎯 InteractionManager Instance oluşturuldu!");
        }
    }

    private void Start()
    {
        Debug.Log("🎯 InteractionManager Start çalışıyor...");
        
        // AmmoBox'ları kontrol et
        AmmoBox[] ammoBoxes = FindObjectsOfType<AmmoBox>();
        Debug.Log($"   Sahnede {ammoBoxes.Length} adet AmmoBox bulundu:");
        foreach (AmmoBox box in ammoBoxes)
        {
            Debug.Log($"     - {box.name}: {box.ammoType}, Miktar: {box.ammoAmount}");
            
            // Collider kontrolü
            Collider col = box.GetComponent<Collider>();
            if (col != null)
            {
                Debug.Log($"       Collider: {col.GetType().Name}, IsTrigger: {col.isTrigger}");
            }
            else
            {
                Debug.LogError($"       ❌ {box.name} üzerinde Collider YOK!");
            }
        }
        
        // SoundManager kontrolü ve oluşturma
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("SoundManager.Instance null! Otomatik oluşturuluyor...");
            CreateSoundManagerFromInteraction();
        }
        
        // HUDManager kontrolü
        if (HUDManager.Instance == null)
        {
            Debug.LogWarning("HUDManager.Instance null! Canvas'ta HUDManager script'i gerekli.");
        }
    }
    
    private void CreateSoundManagerFromInteraction()
    {
        Debug.Log("InteractionManager tarafından SoundManager oluşturuluyor...");
        
        // Yeni GameObject oluştur
        GameObject soundManagerGO = new GameObject("SoundManager");
        
        // DontDestroyOnLoad ekle - Bu çok önemli!
        DontDestroyOnLoad(soundManagerGO);
        
        // SoundManager script'ini ekle
        SoundManager soundManager = soundManagerGO.AddComponent<SoundManager>();
        
        // AudioSource'ları ekle
        soundManager.ShootingChannel = soundManagerGO.AddComponent<AudioSource>();
        soundManager.reloadingSound1911 = soundManagerGO.AddComponent<AudioSource>();
        soundManager.reloadingSoundM16 = soundManagerGO.AddComponent<AudioSource>();
        soundManager.emptyMagazineSound1911 = soundManagerGO.AddComponent<AudioSource>();
        
        Debug.Log("SoundManager oluşturuldu ve DontDestroyOnLoad ile korundu!");
    }

    private void Update()
    {
        // F ve E tuşu genel kontrolü - cursor problemini test et
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F tuşu algılandı! Cursor durumu: " + Cursor.lockState);
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E tuşu algılandı! Cursor durumu: " + Cursor.lockState);
        }
        
        // Camera'nın var olup olmadığını kontrol et
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera bulunamadı!");
            return;
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Raycast mesafesini artır ve debug ekle
        float raycastDistance = 10f;
        bool raycastHit = Physics.Raycast(ray, out hit, raycastDistance);
        
        // Debug: Raycast bilgisi
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"Raycast atıldı: Hit={raycastHit}, Distance={raycastDistance}");
            if (raycastHit)
            {
                Debug.Log($"Raycast mesafe: {hit.distance}");
            }
        }

        if (raycastHit)
        {
            GameObject objectHitByRaycast = hit.transform.gameObject;
            
            // Debug: Ne yakalandığını göster
            if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log($"Raycast vuruş: {objectHitByRaycast.name}, Tag: {objectHitByRaycast.tag}");
                
                // Component'leri listele
                Component[] components = objectHitByRaycast.GetComponents<Component>();
                string componentList = "Components: ";
                foreach (Component comp in components)
                {
                    componentList += comp.GetType().Name + ", ";
                }
                Debug.Log(componentList);
            }
            
            if (objectHitByRaycast.GetComponent<Weapon>() && objectHitByRaycast.GetComponent<Weapon>().isActiveWeapon == false)
            {
                hoveredWeapon = objectHitByRaycast.gameObject.GetComponent<Weapon>();
                
                // Outline component'ini kontrol et
                if (hoveredWeapon.GetComponent<Outline>() != null)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = true;
                }
                
                Debug.Log("Silaha bakıyor: " + objectHitByRaycast.name + " - F veya E tuşuna basın!");
                
                // F veya E tuşu ile silah alma
                if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E))
                {
                    string key = Input.GetKeyDown(KeyCode.F) ? "F" : "E";
                    Debug.Log($"{key} tuşuna basıldı, silah alınıyor: " + objectHitByRaycast.name);
                    Debug.Log($"{key} tuşu öncesi Cursor.lockState: " + Cursor.lockState);
                    
                    // WeaponManager Instance kontrolü
                    if (WeaponManager.Instance != null)
                    {
                        WeaponManager.Instance.PickupWeapon(objectHitByRaycast.gameObject);
                        Debug.Log($"{key} tuşu sonrası Cursor.lockState: " + Cursor.lockState);
                    }
                    else
                    {
                        Debug.LogError("WeaponManager.Instance null!");
                        Debug.LogError("Çözüm: Hierarchy'de boş GameObject oluşturun, adını 'WeaponManager' yapın ve WeaponManager script'ini ekleyin.");
                        
                        // Alternatif: WeaponManager'ı otomatik oluştur
                        CreateWeaponManager();
                    }
                }
                
            }
            else
            {
                if (hoveredWeapon)
                {
                    // Outline component'ini kontrol et
                    if (hoveredWeapon.GetComponent<Outline>() != null)
                    {
                        hoveredWeapon.GetComponent<Outline>().enabled = false;
                    }
                    hoveredWeapon = null;
                }
            }
            // AmmoBox - hem obje hem parent'ta olabilir
            var hitAmmo = objectHitByRaycast.GetComponent<AmmoBox>() ?? objectHitByRaycast.GetComponentInParent<AmmoBox>();
            if (hitAmmo)
            {
                hoveredAmmoBox = hitAmmo;
                Debug.Log($"AmmoBox algılandı: {hoveredAmmoBox.name}, Tip: {hoveredAmmoBox.ammoType}, Miktar: {hoveredAmmoBox.ammoAmount}");
                
                // Outline component'ini kontrol et
                if (hoveredAmmoBox.GetComponent<Outline>() != null)
                {
                    hoveredAmmoBox.GetComponent<Outline>().enabled = true;
                }
                
                if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E))
                {
                    string key = Input.GetKeyDown(KeyCode.F) ? "F" : "E";
                    Debug.Log($"{key} tuşuna basıldı, mermi alınıyor: " + objectHitByRaycast.name);
                    
                    // WeaponManager Instance kontrolü
                    if (WeaponManager.Instance != null)
                    {
                        Debug.Log("WeaponManager Instance bulundu, PickupAmmo çağrılıyor...");
                        WeaponManager.Instance.PickupAmmo(hoveredAmmoBox);
                        Debug.Log("PickupAmmo tamamlandı, obje yok ediliyor...");
                        // WeaponManager içinde Destroy ediliyor artık
                    }
                    else
                    {
                        Debug.LogError("WeaponManager.Instance null!");
                        Debug.LogError("Çözüm: Hierarchy'de boş GameObject oluşturun, adını 'WeaponManager' yapın ve WeaponManager script'ini ekleyin.");
                        
                        // Alternatif: WeaponManager'ı otomatik oluştur
                        CreateWeaponManager();
                    }
                }
            }
            else
            {
                if (hoveredAmmoBox)
                {
                    // Outline component'ini kontrol et
                    if (hoveredAmmoBox.GetComponent<Outline>() != null)
                    {
                        hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                    }
                    hoveredAmmoBox = null;
                }
            }
        }
    }
    
    private void CreateWeaponManager()
    {
        Debug.Log("WeaponManager otomatik oluşturuluyor...");
        
        // Yeni GameObject oluştur
        GameObject weaponManagerGO = new GameObject("WeaponManager");
        
        // WeaponManager script'ini ekle
        WeaponManager weaponManager = weaponManagerGO.AddComponent<WeaponManager>();
        
        // SoundManager da yoksa oluştur
        if (SoundManager.Instance == null)
        {
            Debug.Log("SoundManager da oluşturuluyor...");
            GameObject soundManagerGO = new GameObject("SoundManager");
            SoundManager soundManager = soundManagerGO.AddComponent<SoundManager>();
            
            // AudioSource'ları ekle
            soundManager.ShootingChannel = soundManagerGO.AddComponent<AudioSource>();
            soundManager.reloadingSound1911 = soundManagerGO.AddComponent<AudioSource>();
            soundManager.reloadingSoundM16 = soundManagerGO.AddComponent<AudioSource>();
            
            Debug.Log("SoundManager oluşturuldu! AudioClip'leri manuel olarak atamanız gerekiyor.");
        }
        
        // Player'ı bul
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Player bulunamadı. MainCamera'dan parent'ı arıyor...");
            if (Camera.main != null)
            {
                player = Camera.main.transform.parent?.gameObject;
            }
        }
        
        if (player != null)
        {
            // Weapon slot'ları oluştur
            GameObject slot1 = new GameObject("WeaponSlot1");
            GameObject slot2 = new GameObject("WeaponSlot2");
            
            slot1.transform.SetParent(player.transform);
            slot2.transform.SetParent(player.transform);
            
            // WeaponManager'a slot'ları ata
            weaponManager.weaponSlots = new List<GameObject> { slot1, slot2 };
            weaponManager.activeWeaponSlot = slot1;
            
            Debug.Log("WeaponManager başarıyla oluşturuldu ve kuruldu!");
        }
        else
        {
            Debug.LogError("Player GameObject'i bulunamadı! Manuel olarak weapon slot'ları atamanız gerekecek.");
        }
    }
}
