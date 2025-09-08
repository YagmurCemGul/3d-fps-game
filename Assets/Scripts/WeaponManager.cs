using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; set; }

    public List<GameObject> weaponSlots;
    public GameObject activeWeaponSlot;

    [Header("Ammo")] 
    public int totalRifleAmmo = 0;
    public int totalPistolAmmo = 0;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Debug.Log("🔫 WeaponManager Instance oluşturuldu!");
            
            // Weapon slot'ları hemen oluştur
            Debug.Log("WeaponManager: Awake()'de weapon slots oluşturuluyor...");
            CreateWeaponSlots();
            
            // SoundManager yoksa oluştur
            if (SoundManager.Instance == null)
            {
                Debug.Log("WeaponManager: SoundManager yok, oluşturuluyor...");
                CreateSoundManagerFromWeaponManager();
            }
            
            // HUDManager yoksa sadece uyarı ver (otomatik oluşturma!)
            if (HUDManager.Instance == null)
            {
                Debug.LogWarning("WeaponManager: HUDManager yok! Mevcut HUD'a HUDManager script'i ekleyin.");
            }
        }
    }
    
    private void CreateSoundManagerFromWeaponManager()
    {
        Debug.Log("WeaponManager tarafından SoundManager oluşturuluyor...");
        
        // Yeni GameObject oluştur
        GameObject soundManagerGO = new GameObject("SoundManager");
        
        // DontDestroyOnLoad ekle
        DontDestroyOnLoad(soundManagerGO);
        
        // SoundManager script'ini ekle
        SoundManager soundManager = soundManagerGO.AddComponent<SoundManager>();
        
        // AudioSource'ları ekle - PlayOnAwake = FALSE
        AudioSource shootingChannel = soundManagerGO.AddComponent<AudioSource>();
        shootingChannel.playOnAwake = false;
        soundManager.ShootingChannel = shootingChannel;
        
        AudioSource reload1911 = soundManagerGO.AddComponent<AudioSource>();
        reload1911.playOnAwake = false;
        soundManager.reloadingSound1911 = reload1911;
        
        AudioSource reloadM16 = soundManagerGO.AddComponent<AudioSource>();
        reloadM16.playOnAwake = false;
        soundManager.reloadingSoundM16 = reloadM16;
        
        AudioSource emptyMag = soundManagerGO.AddComponent<AudioSource>();
        emptyMag.playOnAwake = false;
        soundManager.emptyMagazineSound1911 = emptyMag;
        
        Debug.Log("✅ SoundManager WeaponManager tarafından oluşturuldu - PlayOnAwake = FALSE!");
    }
    
    private void CreateHUDManagerFromWeaponManager()
    {
        Debug.Log("WeaponManager tarafından HUDManager oluşturuluyor...");
        
        // Canvas'ı bul
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas bulunamadı! UI için Canvas oluşturun.");
            return;
        }
        
        Debug.Log($"Canvas bulundu: {canvas.name}, HUDManager ekleniyor...");
        
        // HUDManager component'ini Canvas'a ekle
        HUDManager hudManager = canvas.gameObject.AddComponent<HUDManager>();
        
        // Temel UI elementlerini dummy olarak oluştur (gerçek UI yoksa)
        CreateBasicUIElements(canvas, hudManager);
        
        Debug.Log("✅ HUDManager WeaponManager tarafından oluşturuldu!");
    }
    
    private void CreateBasicUIElements(Canvas canvas, HUDManager hudManager)
    {
        Debug.Log("Temel UI elementleri oluşturuluyor...");
        
        // Ammo UI container - sağ alt köşe
        GameObject ammoContainer = new GameObject("AmmoContainer");
        ammoContainer.transform.SetParent(canvas.transform, false);
        
        // RectTransform ekle ve sağ alt köşeye konumlandır
        RectTransform ammoRect = ammoContainer.AddComponent<RectTransform>();
        ammoRect.anchorMin = new Vector2(1, 0); // Sağ alt
        ammoRect.anchorMax = new Vector2(1, 0); // Sağ alt
        ammoRect.anchoredPosition = new Vector2(-100, 100); // Kenarda biraz içerde
        ammoRect.sizeDelta = new Vector2(200, 100);
        
        // Magazine ammo text - üstte
        GameObject magazineTextGO = new GameObject("MagazineAmmo");
        magazineTextGO.transform.SetParent(ammoContainer.transform, false);
        var magazineText = magazineTextGO.AddComponent<TMPro.TextMeshProUGUI>();
        magazineText.text = "30";
        magazineText.fontSize = 36;
        magazineText.color = UnityEngine.Color.white;
        magazineText.alignment = TMPro.TextAlignmentOptions.Center;
        
        RectTransform magRect = magazineTextGO.GetComponent<RectTransform>();
        magRect.anchorMin = new Vector2(0, 0.5f);
        magRect.anchorMax = new Vector2(1, 1);
        magRect.anchoredPosition = Vector2.zero;
        magRect.sizeDelta = Vector2.zero;
        
        hudManager.magazineAmmoUI = magazineText;
        
        // Total ammo text - altta
        GameObject totalTextGO = new GameObject("TotalAmmo");
        totalTextGO.transform.SetParent(ammoContainer.transform, false);
        var totalText = totalTextGO.AddComponent<TMPro.TextMeshProUGUI>();
        totalText.text = "120";
        totalText.fontSize = 24;
        totalText.color = UnityEngine.Color.gray;
        totalText.alignment = TMPro.TextAlignmentOptions.Center;
        
        RectTransform totalRect = totalTextGO.GetComponent<RectTransform>();
        totalRect.anchorMin = new Vector2(0, 0);
        totalRect.anchorMax = new Vector2(1, 0.5f);
        totalRect.anchoredPosition = Vector2.zero;
        totalRect.sizeDelta = Vector2.zero;
        
        hudManager.totalAmmoUI = totalText;
        
        // Middle dot - ekran ortası
        GameObject middleDotGO = new GameObject("MiddleDot");
        middleDotGO.transform.SetParent(canvas.transform, false);
        
        RectTransform dotRect = middleDotGO.AddComponent<RectTransform>();
        dotRect.anchorMin = new Vector2(0.5f, 0.5f); // Orta
        dotRect.anchorMax = new Vector2(0.5f, 0.5f); // Orta
        dotRect.anchoredPosition = Vector2.zero;
        dotRect.sizeDelta = new Vector2(4, 4);
        
        // Beyaz bir nokta ekle
        var dotImage = middleDotGO.AddComponent<UnityEngine.UI.Image>();
        dotImage.color = UnityEngine.Color.white;
        
        hudManager.middleDot = middleDotGO;
        
        Debug.Log("✅ UI elementleri sağ alt köşeye yerleştirildi.");
    }

    private void Start()
    {
        Debug.Log("🔫 WeaponManager Start çalışıyor...");
        Debug.Log($"   Pistol Ammo: {totalPistolAmmo}");
        Debug.Log($"   Rifle Ammo: {totalRifleAmmo}");
        Debug.Log($"   Weapon Slots: {weaponSlots?.Count ?? 0}");
        
        // Weapon slot'ları kontrol et (Awake'de oluşturuldu olmalı)
        if (weaponSlots == null || weaponSlots.Count == 0)
        {
            Debug.LogError("WeaponManager: Weapon slots Awake'de oluşturulamamış! Tekrar deneyin...");
            CreateWeaponSlots();
        }
        
        Debug.Log($"   Active Weapon Slot: {activeWeaponSlot?.name ?? "NULL"}");
        
        if (activeWeaponSlot != null)
        {
            Weapon activeWeapon = activeWeaponSlot.GetComponentInChildren<Weapon>();
            if (activeWeapon != null)
            {
                Debug.Log($"   Aktif Silah: {activeWeapon.name}, Mermi: {activeWeapon.bulletsLeft}");
            }
            else
            {
                Debug.Log("   Aktif silah yok (normal - henüz silah alınmamış)");
            }
        }
        else
        {
            Debug.LogError("   ❌ Active Weapon Slot hala NULL!");
        }
    }
    
    private void CreateWeaponSlots()
    {
        Debug.Log("WeaponManager: Weapon slot'ları oluşturuluyor...");
        
        // weaponSlots listesini başlat
        if (weaponSlots == null)
        {
            weaponSlots = new List<GameObject>();
        }
        
        // 2 slot oluştur (primary ve secondary)
        for (int i = 0; i < 2; i++)
        {
            GameObject slot = new GameObject($"WeaponSlot_{i + 1}");
            slot.transform.SetParent(transform);
            slot.transform.localPosition = Vector3.zero;
            slot.transform.localRotation = Quaternion.identity;
            weaponSlots.Add(slot);
            
            Debug.Log($"WeaponSlot_{i + 1} oluşturuldu");
        }
        
        // İlk slot'u aktif yap
        if (weaponSlots.Count > 0)
        {
            activeWeaponSlot = weaponSlots[0];
            Debug.Log($"ActiveWeaponSlot set edildi: {activeWeaponSlot.name}");
        }
    }

    private void InitializeWeapons()
    {
        // Weapon slot'ları kontrol et
        if (weaponSlots != null && weaponSlots.Count > 0)
        {
            activeWeaponSlot = weaponSlots[0];
            
            // Başlangıçta tüm silahları pasif yap
            foreach (GameObject weaponSlot in weaponSlots)
            {
                if (weaponSlot != null && weaponSlot.transform.childCount > 0)
                {
                    for (int i = 0; i < weaponSlot.transform.childCount; i++)
                    {
                        GameObject weapon = weaponSlot.transform.GetChild(i).gameObject;
                        Weapon weaponScript = weapon.GetComponent<Weapon>();
                        if (weaponScript != null)
                        {
                            weaponScript.isActiveWeapon = false;
                            Debug.Log($"Başlangıçta silah pasif yapıldı: {weapon.name}");
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("WeaponManager: weaponSlots listesi boş veya null!");
        }
    }

    private void Update()
    {
        foreach (GameObject weaponSlot in weaponSlots)
        {
            if (weaponSlot == activeWeaponSlot)
            {
                weaponSlot.SetActive(true);
            }
            else
            {
                weaponSlot.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchActiveSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchActiveSlot(1);
        }
    }

    public void PickupWeapon(GameObject pickedupWeapon)
    {
        Debug.Log("PickupWeapon çağrıldı: " + pickedupWeapon.name);
        
        // Null kontrolları
        if (pickedupWeapon == null)
        {
            Debug.LogError("PickupWeapon: pickedupWeapon null!");
            return;
        }
        
        if (activeWeaponSlot == null)
        {
            Debug.LogError("PickupWeapon: activeWeaponSlot null!");
            Debug.Log("Weapon slot'ları otomatik oluşturuluyor...");
            CreateWeaponSlots();
            
            if (activeWeaponSlot == null)
            {
                Debug.LogError("Weapon slot'ları oluşturulamadı!");
                return;
            }
        }
        
        AddWeaponIntoActiveSlot(pickedupWeapon);
    }

    private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    {
        DropCurrentWeapon(pickedupWeapon);
        pickedupWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        Weapon weapon = pickedupWeapon.GetComponent<Weapon>();
        
        // Weapon component kontrolü
        if (weapon == null)
        {
            Debug.LogError("AddWeaponIntoActiveSlot: Weapon component bulunamadı!");
            return;
        }

        // Position ve Rotation ayarlama
        pickedupWeapon.transform.localPosition = weapon.spawnPosition;
        pickedupWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation); 
        
        weapon.isActiveWeapon = true;
        
        // Animator kontrolü
        if (weapon.animator != null)
        {
            weapon.animator.enabled = true;
        }
        else
        {
            Debug.LogWarning("AddWeaponIntoActiveSlot: Weapon animator null!");
        }

        // HUD güncelle
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateAmmoUI(weapon);
        }
    }

    private void DropCurrentWeapon(GameObject pickedupWeapon)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;
           
            weaponToDrop.GetComponent<Weapon>().isActiveWeapon = false;
            weaponToDrop.GetComponent<Weapon>().animator.enabled = false;
            
            weaponToDrop.transform.SetParent(pickedupWeapon.transform.parent);
            weaponToDrop.transform.localPosition = pickedupWeapon.transform.localPosition;
            weaponToDrop.transform.localRotation = pickedupWeapon.transform.localRotation;

        }
    }

    internal void PickupAmmo(AmmoBox ammo)
    {
        Debug.Log($"Ammo alınıyor: {ammo.ammoType}, Miktar: {ammo.ammoAmount}");
        
        switch (ammo.ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammo.ammoAmount;
                Debug.Log($"Pistol ammo güncellendi: {totalPistolAmmo}");
                break;
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += ammo.ammoAmount;
                Debug.Log($"Rifle ammo güncellendi: {totalRifleAmmo}");
                break;
        }
        
        // AmmoBox objesini yok et
        Destroy(ammo.gameObject);
        Debug.Log("AmmoBox objesi yok edildi.");

        // HUD güncelle (aktif silaha göre total değişir)
        if (HUDManager.Instance != null && activeWeaponSlot != null && activeWeaponSlot.transform.childCount > 0)
        {
            var activeWeapon = activeWeaponSlot.GetComponentInChildren<Weapon>();
            if (activeWeapon != null)
            {
                HUDManager.Instance.UpdateAmmoUI(activeWeapon);
            }
        }
    }

    public void SwitchActiveSlot(int slotNumber)
    {
        // Mevcut silahı pasif yap
        if (activeWeaponSlot != null && activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            if (currentWeapon != null)
            {
                currentWeapon.isActiveWeapon = false;
                Debug.Log($"Silah pasif yapıldı: {currentWeapon.gameObject.name}");
            }
        }

        // Yeni slot'a geç
        if (weaponSlots != null && slotNumber < weaponSlots.Count)
        {
            activeWeaponSlot = weaponSlots[slotNumber];
            
            // Yeni silahı aktif yap
            if (activeWeaponSlot.transform.childCount > 0)
            {
                Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
                if (newWeapon != null)
                {
                    newWeapon.isActiveWeapon = true;
                    Debug.Log($"Silah aktif yapıldı: {newWeapon.gameObject.name}");

                    // HUD güncelle
                    if (HUDManager.Instance != null)
                    {
                        HUDManager.Instance.UpdateAmmoUI(newWeapon);
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"SwitchActiveSlot: Geçersiz slot numarası: {slotNumber}");
        }
    }

    internal void DecreaseTotalAmmo(int bulletsToDecrease, Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.M16:
                totalRifleAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.Pistol1911:
                totalPistolAmmo -= bulletsToDecrease;
                break;
        }
    }
    
    public int CheckAmmoLeftFor(Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.M16:
                return totalRifleAmmo;
            case Weapon.WeaponModel.Pistol1911:
                return totalPistolAmmo;
            
            default:
                return 0;
        }
    }
}
