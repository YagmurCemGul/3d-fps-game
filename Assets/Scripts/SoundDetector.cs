using UnityEngine;

public class SoundDetector : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== SES DETEKTÖR BAŞLATILDI ===");
        
        // Tüm AudioSource'ları bul
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        Debug.Log($"Sahnede toplam {allAudioSources.Length} AudioSource bulundu:");
        
        foreach (AudioSource audioSource in allAudioSources)
        {
            Debug.Log($"AudioSource: {audioSource.name}");
            Debug.Log($"  Parent: {audioSource.transform.parent?.name ?? "ROOT"}");
            Debug.Log($"  IsPlaying: {audioSource.isPlaying}");
            Debug.Log($"  Clip: {audioSource.clip?.name ?? "NULL"}");
            Debug.Log($"  PlayOnAwake: {audioSource.playOnAwake}");
            Debug.Log($"  Volume: {audioSource.volume}");
            Debug.Log($"  Mute: {audioSource.mute}");
            Debug.Log("---");
            
            // Eğer çalıyorsa durdur ve log
            if (audioSource.isPlaying)
            {
                Debug.LogError($"⚠️ SES ÇALIYOR: {audioSource.name} - {audioSource.clip?.name}");
                Debug.LogError($"   Bu AudioSource durduruluyor!");
                audioSource.Stop();
            }
        }
        
        Debug.Log("=== SES DETEKTÖR BİTTİ ===");
    }
    
    void Update()
    {
        // Her frame kontrol et (ilk 5 saniye)
        if (Time.time < 5f)
        {
            AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in allAudioSources)
            {
                if (audioSource.isPlaying)
                {
                    Debug.LogError($"⚠️ FRAME {Time.frameCount}: {audioSource.name} SES ÇALIYOR!");
                    Debug.LogError($"   Clip: {audioSource.clip?.name}");
                    Debug.LogError($"   Time: {audioSource.time}");
                    audioSource.Stop();
                }
            }
        }
    }
}
