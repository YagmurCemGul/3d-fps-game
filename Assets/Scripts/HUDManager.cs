using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    
    public static HUDManager Instance { get; set; }

    [Header("Ammo")] 
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image ammoTypeUI;

    [Header("Weapon")] 
    public Image activeWeaponUI;
    public Image UnActiveWeaponUI;

    [Header("Throwables")] 
    public Image lethalUI;
    public TextMeshProUGUI lethalAmountUI;

    public Image tacticalUI;
    public TextMeshProUGUI tacticalAmountUI;

    public Sprite emptySlot;

    public GameObject middleDot;

    // Sahne yüklendikten sonra HUDManager yoksa otomatik ekle
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureHUDExists()
    {
        if (Instance != null) return;
        try
        {
            var canvases = Resources.FindObjectsOfTypeAll<Canvas>()
                .Where(c => c != null && c.gameObject.scene.IsValid())
                .ToArray();
            if (canvases != null && canvases.Length > 0)
            {
                // En makul Canvas'a ekle
                var targetCanvas = canvases[0];
                foreach (var c in canvases)
                {
                    if (c.gameObject.name.ToLower().Contains("ui") || c.gameObject.name.ToLower().Contains("hud"))
                    {
                        targetCanvas = c; break;
                    }
                }
                targetCanvas.gameObject.AddComponent<HUDManager>();
                Debug.Log("📱 HUDManager otomatik eklendi (bootstrap).");
            }
            else
            {
                Debug.LogWarning("📱 Canvas bulunamadı, HUDManager eklenemedi.");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"HUD bootstrap hata: {e.Message}");
        }
    }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Debug.Log("📱 HUDManager Instance oluşturuldu!");
        }
    }

    private void Start()
    {
        Debug.Log("📱 HUDManager Start çalışıyor...");
        // Eksik referanslar varsa otomatik ata
        AutoAssignUIElementsIfMissing();

        Debug.Log($"   magazineAmmoUI: {magazineAmmoUI?.name ?? "NULL"}");
        Debug.Log($"   totalAmmoUI: {totalAmmoUI?.name ?? "NULL"}");
        Debug.Log($"   ammoTypeUI: {ammoTypeUI?.name ?? "NULL"}");
    }



    private void Update()
    {
        // WeaponManager null kontrolü
        if (WeaponManager.Instance == null)
        {
            Debug.LogWarning("HUDManager: WeaponManager.Instance null!");
            return;
        }
        
        if (WeaponManager.Instance.activeWeaponSlot == null)
        {
            Debug.LogWarning("HUDManager: activeWeaponSlot null!");
            return;
        }
        
        Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon unActiveWeapon = GetUnActiveWeaponSlot()?.GetComponentInChildren<Weapon>();

        if (activeWeapon)
        {
            // Doğru magazine ammo gösterimi
            if (magazineAmmoUI != null)
            {
                magazineAmmoUI.text = $"{activeWeapon.bulletsLeft}";
            }
            if (totalAmmoUI != null)
            {
                totalAmmoUI.text = $"{WeaponManager.Instance.CheckAmmoLeftFor(activeWeapon.thisWeaponModel)}";
            }
            
            Debug.Log($"HUD güncellendi - Magazine: {activeWeapon.bulletsLeft}, Total: {WeaponManager.Instance.CheckAmmoLeftFor(activeWeapon.thisWeaponModel)}");

            Weapon.WeaponModel model = activeWeapon.thisWeaponModel;
            
            // Sprite atamaları güvenli hale getir
            if (ammoTypeUI != null)
            {
                var sprite = GetAmmoSprite(model);
                if (sprite != null)
                {
                    ammoTypeUI.sprite = sprite;
                }
            }

            if (activeWeaponUI != null)
            {
                var sprite = GetWeaponSprite(model);
                if (sprite != null)
                {
                    activeWeaponUI.sprite = sprite;
                }
            }
            
            if (unActiveWeapon && UnActiveWeaponUI != null)
            {
                var sprite = GetWeaponSprite(unActiveWeapon.thisWeaponModel);
                if (sprite != null)
                {
                    UnActiveWeaponUI.sprite = sprite;
                }
            }
        }
        else
        {
            if (magazineAmmoUI != null) magazineAmmoUI.text = "";
            if (totalAmmoUI != null) totalAmmoUI.text = "";

            if (ammoTypeUI != null && emptySlot != null) ammoTypeUI.sprite = emptySlot;
            if (activeWeaponUI != null && emptySlot != null) activeWeaponUI.sprite = emptySlot;
            if (UnActiveWeaponUI != null && emptySlot != null) UnActiveWeaponUI.sprite = emptySlot;
        }
    }
    private GameObject GetUnActiveWeaponSlot()
    {
        foreach (GameObject weaponSlot in WeaponManager.Instance.weaponSlots)
        {
            if (weaponSlot != WeaponManager.Instance.activeWeaponSlot)
            {
                return weaponSlot;
            }
        }

        return null;
    }

    // Diğer scriptler tarafından anlık çağrılabilen güvenli UI güncelleme
    public void UpdateAmmoUI(Weapon activeWeapon)
    {
        if (WeaponManager.Instance == null || activeWeapon == null)
        {
            return;
        }

        // Eğer referanslar eksikse yeniden bulmayı dene
        if (magazineAmmoUI == null || totalAmmoUI == null)
        {
            AutoAssignUIElementsIfMissing();
        }

        if (magazineAmmoUI != null)
        {
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft}";
        }

        if (totalAmmoUI != null)
        {
            totalAmmoUI.text = $"{WeaponManager.Instance.CheckAmmoLeftFor(activeWeapon.thisWeaponModel)}";
        }
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.Pistol1911:
                return SafeLoadSpriteFromPrefab("Pistol_Ammo");
            case Weapon.WeaponModel.M16:
                return SafeLoadSpriteFromPrefab("Rifle_Ammo");
            default:
                return null;
        }
    }
    
    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.Pistol1911:
                return SafeLoadSpriteFromPrefab("Pistol1911_Weapon");
            case Weapon.WeaponModel.M16:
                return SafeLoadSpriteFromPrefab("M16_Weapon");
            default:
                return null;
        }
    }

    // Prefab'den güvenli şekilde Sprite alma ve cache'leme
    private readonly Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();
    private Sprite SafeLoadSpriteFromPrefab(string resourceName)
    {
        if (_spriteCache.TryGetValue(resourceName, out var cached))
        {
            return cached;
        }

        var prefab = Resources.Load<GameObject>(resourceName);
        if (prefab == null)
        {
            Debug.LogWarning($"HUDManager: Resources '{resourceName}' bulunamadı.");
            return null;
        }

        var sr = prefab.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            // Bazı durumlarda prefab içindeki child'da olabilir
            sr = prefab.GetComponentInChildren<SpriteRenderer>();
        }
        if (sr == null)
        {
            Debug.LogWarning($"HUDManager: '{resourceName}' üzerinde SpriteRenderer bulunamadı.");
            return null;
        }
        _spriteCache[resourceName] = sr.sprite;
        return sr.sprite;
    }

    // Eksik UI referanslarını isimlerine göre otomatik ata
    private void AutoAssignUIElementsIfMissing()
    {
        try
        {
            if (magazineAmmoUI == null || totalAmmoUI == null || ammoTypeUI == null || activeWeaponUI == null || UnActiveWeaponUI == null || middleDot == null)
            {
                var allTexts = FindObjectsOfType<TMPro.TextMeshProUGUI>(true);
                foreach (var text in allTexts)
                {
                    var n = text.name.ToLower();
                    if (magazineAmmoUI == null && n.Contains("magazine") && n.Contains("ammo"))
                        magazineAmmoUI = text;
                    else if (totalAmmoUI == null && n.Contains("total") && n.Contains("ammo"))
                        totalAmmoUI = text;
                    else if (magazineAmmoUI == null && (n.Contains("mermi") || n.Contains("sarj") || n.Contains("sarjor")))
                        magazineAmmoUI = text; // Türkçe isimler için
                }

                var allImages = FindObjectsOfType<UnityEngine.UI.Image>(true);
                foreach (var img in allImages)
                {
                    var n = img.name.ToLower();
                    if (ammoTypeUI == null && n.Contains("ammo") && n.Contains("type"))
                        ammoTypeUI = img;
                    else if (activeWeaponUI == null && n.Contains("active") && n.Contains("weapon"))
                        activeWeaponUI = img;
                    else if (UnActiveWeaponUI == null && (n.Contains("inactive") || (n.Contains("weapon") && n.Contains("2"))))
                        UnActiveWeaponUI = img;
                    else if (middleDot == null && (n.Contains("crosshair") || n.Contains("dot") || n.Contains("middle")))
                        middleDot = img.gameObject;
                }

                // WeaponPanel prefabı kullananlar için yaygın isimler
                if (magazineAmmoUI == null)
                {
                    var obj = GameObject.Find("MagazineAmmo");
                    if (obj) magazineAmmoUI = obj.GetComponent<TMPro.TextMeshProUGUI>();
                }
                if (totalAmmoUI == null)
                {
                    var obj = GameObject.Find("TotalAmmo");
                    if (obj) totalAmmoUI = obj.GetComponent<TMPro.TextMeshProUGUI>();
                }
                if (ammoTypeUI == null)
                {
                    var obj = GameObject.Find("AmmoType");
                    if (obj) ammoTypeUI = obj.GetComponent<UnityEngine.UI.Image>();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"HUDManager AutoAssign hata: {e.Message}");
        }
    }
}
