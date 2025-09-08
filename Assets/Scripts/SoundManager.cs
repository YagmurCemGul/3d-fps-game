using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }
    
    public AudioSource ShootingChannel;
    

    public AudioClip P1911Shot;
    public AudioClip M16Shot;
    
    
    public AudioSource reloadingSoundM16;
    public AudioSource reloadingSound1911;
    public AudioSource emptyMagazineSound1911;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // SoundManager'ı sahne değişikliğinde yok etme
            DontDestroyOnLoad(gameObject);
            Debug.Log("SoundManager Instance oluşturuldu ve DontDestroyOnLoad eklendi");
        }
    }
    
    private void Start()
    {
        // SoundManager kurulum kontrolleri
        Debug.Log("SoundManager başlatılıyor...");
        
        // Ses dosyalarını otomatik yükle
        LoadAudioClips();
        
        // AudioSource kontrolleri
        if (ShootingChannel == null)
        {
            Debug.LogError("SoundManager: ShootingChannel AudioSource atanmamış!");
        }
        
        if (reloadingSound1911 == null)
        {
            Debug.LogWarning("SoundManager: reloadingSound1911 AudioSource atanmamış!");
        }
        
        if (reloadingSoundM16 == null)
        {
            Debug.LogWarning("SoundManager: reloadingSoundM16 AudioSource atanmamış!");
        }
        
        // AudioClip kontrolleri (artık otomatik yüklendiler)
        if (P1911Shot == null)
        {
            Debug.LogError("SoundManager: P1911Shot AudioClip bulunamadı!");
        }
        
        if (M16Shot == null)
        {
            Debug.LogError("SoundManager: M16Shot AudioClip bulunamadı!");
        }
        
        Debug.Log("SoundManager kontrolü tamamlandı.");
    }
    
    public void LoadAudioClips()
    {
        Debug.Log("Ses dosyaları yükleniyor... (SES ÇALMAYACAK)");
        
        // Önce tüm AudioSource'ları durdur ve playOnAwake=false yap
        if (ShootingChannel != null)
        {
            ShootingChannel.playOnAwake = false;
            ShootingChannel.Stop();
        }
        if (reloadingSound1911 != null)
        {
            reloadingSound1911.playOnAwake = false;
            reloadingSound1911.Stop();
        }
        if (reloadingSoundM16 != null)
        {
            reloadingSoundM16.playOnAwake = false;
            reloadingSoundM16.Stop();
        }
        if (emptyMagazineSound1911 != null)
        {
            emptyMagazineSound1911.playOnAwake = false;
            emptyMagazineSound1911.Stop();
        }
        
        // Resources klasöründen yüklemeyi dene
        if (P1911Shot == null)
        {
            P1911Shot = Resources.Load<AudioClip>("Sounds/colt1911_shot");
            if (P1911Shot == null)
            {
                // Alternatif path dene
                P1911Shot = Resources.Load<AudioClip>("colt1911_shot");
            }
            if (P1911Shot != null)
            {
                Debug.Log("P1911Shot ses dosyası yüklendi!");
            }
        }
        
        if (M16Shot == null)
        {
            M16Shot = Resources.Load<AudioClip>("Sounds/M16_Shot");
            if (M16Shot == null)
            {
                M16Shot = Resources.Load<AudioClip>("M16_Shot");
            }
            if (M16Shot != null)
            {
                Debug.Log("M16Shot ses dosyası yüklendi!");
            }
        }
        
        // Reload sesleri için AudioClip'leri de yükle
        LoadReloadSounds();
    }
    
    private void LoadReloadSounds()
    {
        // Reload sesleri Resources'tan yükle ve AudioSource'lara ata
        AudioClip reload1911Clip = Resources.Load<AudioClip>("Sounds/reload_1911");
        if (reload1911Clip == null)
        {
            reload1911Clip = Resources.Load<AudioClip>("reload_1911");
        }
        
        AudioClip reloadM16Clip = Resources.Load<AudioClip>("Sounds/M16_Reload");
        if (reloadM16Clip == null)
        {
            reloadM16Clip = Resources.Load<AudioClip>("M16_Reload");
        }
        
        AudioClip emptyMagClip = Resources.Load<AudioClip>("Sounds/empty_magazine");
        if (emptyMagClip == null)
        {
            emptyMagClip = Resources.Load<AudioClip>("empty_magazine");
        }
        
        // AudioSource'lara clip'leri ata ve playOnAwake=false yap
        if (reloadingSound1911 != null && reload1911Clip != null)
        {
            reloadingSound1911.playOnAwake = false;
            reloadingSound1911.Stop(); // Eğer çalıyorsa durdur
            reloadingSound1911.clip = reload1911Clip;
            Debug.Log("Reload 1911 ses dosyası yüklendi (playOnAwake=false)!");
        }
        
        if (reloadingSoundM16 != null && reloadM16Clip != null)
        {
            reloadingSoundM16.playOnAwake = false;
            reloadingSoundM16.Stop(); // Eğer çalıyorsa durdur
            reloadingSoundM16.clip = reloadM16Clip;
            Debug.Log("Reload M16 ses dosyası yüklendi (playOnAwake=false)!");
        }
        
        if (emptyMagazineSound1911 != null && emptyMagClip != null)
        {
            emptyMagazineSound1911.playOnAwake = false;
            emptyMagazineSound1911.Stop(); // Eğer çalıyorsa durdur
            emptyMagazineSound1911.clip = emptyMagClip;
            Debug.Log("Empty magazine ses dosyası yüklendi (playOnAwake=false)!");
        }
        
        Debug.Log("LoadAudioClips tamamlandı - HİÇBİR SES ÇALINMADI!");
    }

    public void PlayShootingSound(WeaponModel weapon)
    {
        Debug.Log("PlayShootingSound çağrıldı: " + weapon);
        
        // ShootingChannel kontrolü
        if (ShootingChannel == null)
        {
            Debug.LogError("SoundManager: ShootingChannel AudioSource atanmamış!");
            return;
        }
        
        switch (weapon)
        {
            case WeaponModel.Pistol1911:
                if (P1911Shot != null)
                {
                    ShootingChannel.PlayOneShot(P1911Shot);
                    Debug.Log("P1911 silah sesi çalınıyor");
                }
                else
                {
                    Debug.LogError("SoundManager: P1911Shot AudioClip atanmamış!");
                }
                break;
            case WeaponModel.M16:
                if (M16Shot != null)
                {
                    ShootingChannel.PlayOneShot(M16Shot);
                    Debug.Log("M16 silah sesi çalınıyor");
                }
                else
                {
                    Debug.LogError("SoundManager: M16Shot AudioClip atanmamış!");
                }
                break;
            default:
                Debug.LogWarning("SoundManager: Bilinmeyen silah tipi: " + weapon);
                break;
        }
    }
    public void PlayReloadSound(WeaponModel weapon)
    {
        Debug.Log("PlayReloadSound çağrıldı: " + weapon);
        
        switch (weapon)
        {
            case WeaponModel.Pistol1911:
                if (reloadingSound1911 != null)
                {
                    reloadingSound1911.Play();
                    Debug.Log("P1911 reload sesi çalınıyor");
                }
                else
                {
                    Debug.LogError("SoundManager: reloadingSound1911 AudioSource atanmamış!");
                }
                break;
            case WeaponModel.M16:
                if (reloadingSoundM16 != null)
                {
                    reloadingSoundM16.Play();
                    Debug.Log("M16 reload sesi çalınıyor");
                }
                else
                {
                    Debug.LogError("SoundManager: reloadingSoundM16 AudioSource atanmamış!");
                }
                break;
            default:
                Debug.LogWarning("SoundManager: Bilinmeyen silah tipi (reload): " + weapon);
                break;
        }
    }
    
}
