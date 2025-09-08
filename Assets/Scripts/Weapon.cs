using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;

    [Header("Shooting")]
    public bool isShooting, readyToShoot;
    private bool allowReset = true;
    public float shootingDelay = 0.2f;

    [Header("Brust")]
    public int bulletsPerBrust = 3;
    public int burstBulletsLeft;

    [Header("Spread")]
    public float spreadIntensity;
    public float hipspreadIntensity;
    public float adsspreadIntensity;
    

    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBrust;
        
        // Animator component'ini al
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning($"Weapon '{gameObject.name}' için Animator component bulunamadı!");
        }

        bulletsLeft = magazineSize;
        spreadIntensity = hipspreadIntensity;
        
        Debug.Log($"Weapon başlatıldı: {gameObject.name}, MagazineSize: {magazineSize}, BulletsLeft: {bulletsLeft}");
        
        // Kritik component'ler için uyarılar
        if (bulletPrefab == null)
        {
            Debug.LogError($"Weapon '{gameObject.name}' için BulletPrefab atanmamış!");
        }
        
        if (bulletSpawn == null)
        {
            Debug.LogError($"Weapon '{gameObject.name}' için BulletSpawn atanmamış!");
        }
    }

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 500;
    public float bulletPrefabLifeTime = 3f;

    public GameObject muzzleEffect;
    internal Animator animator;

    [Header("Loading")]
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    bool isADS;
    
    public enum WeaponModel
    {
        Pistol1911,
        M16
    }

    public WeaponModel thisWeaponModel;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    // Update is called once per frame
    void Update()
    {
        // Sadece aktif silahlar işlem yapabilir
        if (!isActiveWeapon)
        {
            return;
        }

        // WeaponManager eksikse ateş etme
        if (WeaponManager.Instance == null)
        {
            return;
        }

        // Mouse sağ tık için ADS
        if (Input.GetMouseButtonDown(1))
        {
            EnterADS();
        }
        if (Input.GetMouseButtonUp(1))
        {
            ExitADS();
        }
        
        // Outline'ı kapat (aktif silahta outline olmamalı)
        if (GetComponent<Outline>() != null)
        {
            GetComponent<Outline>().enabled = false;
        }
        
        // Boş şarjör sesi
        if (bulletsLeft == 0 && isShooting)
        {
            if (SoundManager.Instance != null && SoundManager.Instance.emptyMagazineSound1911 != null)
            {
                SoundManager.Instance.emptyMagazineSound1911.Play();
            }
            else if (SoundManager.Instance == null)
            {
                Debug.LogWarning("SoundManager.Instance null (EmptyMag)! Otomatik oluşturuluyor...");
                CreateSoundManager();
            }
        }
        
        // Ateş etme modlarına göre input kontrolü
        if (currentShootingMode == ShootingMode.Auto)
        {
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }
        
        // Mouse left click debug
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log($"Weapon: Sol mouse tuşu algılandı - Cursor durumu: {Cursor.lockState}");
        }
        
        // Ateş etme kontrolü - DEBUG ekleyelim
        if (readyToShoot && isShooting && bulletsLeft > 0)
        {
            Debug.Log($"Silah ateş ediyor: {gameObject.name}, Mermi: {bulletsLeft}, Cursor: {Cursor.lockState}");
            burstBulletsLeft = bulletsPerBrust;
            FireWeapon();
            
            // Ateş ettikten sonra cursor kontrolü
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Debug.LogWarning("Weapon: Ateş ettikten sonra cursor unlock oldu! Düzeltiliyor...");
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        else if (isShooting && bulletsLeft <= 0)
        {
            Debug.Log($"Ateş edemez - Mermi yok: {bulletsLeft}");
        }
        else if (isShooting && !readyToShoot)
        {
            Debug.Log($"Ateş edemez - Ready değil: readyToShoot={readyToShoot}");
        }
        
        // Reload kontrolü
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false && WeaponManager.Instance != null && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
        {
            Reload();
        }
        
        if (readyToShoot && isShooting == false && isReloading == false && bulletsLeft <= 0)
        {
            //Reload();
        }
    }

    private void EnterADS()
    {
        animator.SetTrigger("enterADS");
        isADS = true;
        HUDManager.Instance.middleDot.SetActive(false);
        spreadIntensity = adsspreadIntensity;
    }

    private void ExitADS()
    {
        animator.SetTrigger("exitADS");
        isADS = false;
        HUDManager.Instance.middleDot.SetActive(true);
        spreadIntensity = hipspreadIntensity;
    }

    private void FireWeapon()
    {
        bulletsLeft--;

        // Muzzle effect kontrolü
        if (muzzleEffect != null)
        {
            ParticleSystem muzzlePS = muzzleEffect.GetComponent<ParticleSystem>();
            if (muzzlePS != null)
            {
                muzzlePS.Play();
            }
        }
        else
        {
            Debug.LogWarning("Muzzle effect null!");
        }

        // Animator kontrolü
        if (animator != null)
        {
            if (isADS)
            {
                animator.SetTrigger("RECOIL_ADS");
            }
            else
            {
                animator.SetTrigger("RECOIL");
            }
        }
        else
        {
            Debug.LogWarning("Weapon animator null!");
        }

        // SoundManager kontrolü
        if (SoundManager.Instance != null)
        {
            Debug.Log($"SoundManager bulundu, ses çalınıyor: {thisWeaponModel}");
            SoundManager.Instance.PlayShootingSound(thisWeaponModel);
        }
        else
        {
            Debug.Log("SoundManager.Instance null! Otomatik oluşturuluyor...");
            CreateSoundManager();
            // SoundManager oluşturulduktan sonra ses çalmayı dene
            return; // Bu frame'de ses çalmayı atla, sonraki frame'de deneyecek
        }
        
        readyToShoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;
        
        // BulletPrefab ve BulletSpawn kontrolü
        if (bulletPrefab != null && bulletSpawn != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

            bullet.transform.forward = shootingDirection;
            
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
            }
            
            StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
        }
        else
        {
            Debug.LogError("BulletPrefab veya BulletSpawn null! Inspector'da atayın.");
        }

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
        
        // FireWeapon sonunda cursor kontrolü
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Debug.LogWarning("FireWeapon: Ateş sonrası cursor unlock! Düzeltiliyor...");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // HUD güncelle
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateAmmoUI(this);
        }
    }

    private void Reload()
    {
        // SoundManager kontrolü
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayReloadSound(thisWeaponModel);
        }
        else
        {
            Debug.LogWarning("SoundManager.Instance null (Reload)! Otomatik oluşturuluyor...");
            CreateSoundManager();
        }
        
        // Animator kontrolü
        if (animator != null)
        {
            animator.SetTrigger("RELOAD");
        }
        
        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        // WeaponManager kontrolü
        if (WeaponManager.Instance == null)
        {
            Debug.LogError("WeaponManager.Instance null during reload!");
            isReloading = false;
            return;
        }
        
        // Kalan mermileri koru: sadece eksik kadarını rezerve'den doldur
        int reserve = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
        int needed = Mathf.Clamp(magazineSize - bulletsLeft, 0, magazineSize);

        if (reserve > 0 && needed > 0)
        {
            int toLoad = Mathf.Min(needed, reserve);
            bulletsLeft += toLoad;
            WeaponManager.Instance.DecreaseTotalAmmo(toLoad, thisWeaponModel);
        }
        isReloading = false;

        // HUD güncelle
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateAmmoUI(this);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, .5f, 0));
        RaycastHit hit;

        Vector3 targetpoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetpoint = hit.point;
        }
        else
        {
            targetpoint = ray.GetPoint(100);
        }

        Vector3 direction = targetpoint - bulletSpawn.position;

        float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(0, y, z);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
    
    private void CreateSoundManager()
    {
        Debug.Log("Weapon tarafından SoundManager oluşturuluyor...");
        
        // Eğer SoundManager GameObject'i varsa ama Instance null ise
        GameObject existingSM = GameObject.Find("SoundManager");
        if (existingSM != null)
        {
            Debug.Log("Mevcut SoundManager GameObject bulundu, siliniyor...");
            Destroy(existingSM);
        }
        
        // Yeni GameObject oluştur
        GameObject soundManagerGO = new GameObject("SoundManager");
        
        // DontDestroyOnLoad ekle
        DontDestroyOnLoad(soundManagerGO);
        
        // SoundManager script'ini ekle
        SoundManager soundManager = soundManagerGO.AddComponent<SoundManager>();
        
        // AudioSource'ları ekle
        soundManager.ShootingChannel = soundManagerGO.AddComponent<AudioSource>();
        soundManager.reloadingSound1911 = soundManagerGO.AddComponent<AudioSource>();
        soundManager.reloadingSoundM16 = soundManagerGO.AddComponent<AudioSource>();
        soundManager.emptyMagazineSound1911 = soundManagerGO.AddComponent<AudioSource>();
        
        Debug.Log("SoundManager oluşturuldu ve DontDestroyOnLoad eklendi! Ses dosyaları otomatik yüklenecek...");
        
        // Manuel LoadAudioClips KALDIRILDI - Start()'ta otomatik çağrılacak
        // soundManager.LoadAudioClips(); // BU SATIR SES ÇIKARTIYORDU!
        
        Debug.Log("SoundManager initialize edildi. LoadAudioClips Start() metodunda çağrılacak.");
        
        // Invoke kaldırıldı - ses sadece gerçekten ateş ettiğimizde çalacak
    }
}
