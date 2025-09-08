using UnityEngine;

public class SoundDebugger : MonoBehaviour
{
    private bool hasPlayStarted = false;
    
    void Start()
    {
        Debug.Log("=== SoundDebugger Start ===");
        hasPlayStarted = true;
    }
    
    void Update()
    {
        // İlk frame'de ses çalıp çalmadığını kontrol et
        if (hasPlayStarted && Time.frameCount < 10)
        {
            if (SoundManager.Instance != null)
            {
                Debug.Log($"Frame {Time.frameCount}: SoundManager.Instance mevcut");
                
                if (SoundManager.Instance.ShootingChannel != null)
                {
                    if (SoundManager.Instance.ShootingChannel.isPlaying)
                    {
                        Debug.LogError($"Frame {Time.frameCount}: ⚠️ ShootingChannel SES ÇALIYOR! Clip: {SoundManager.Instance.ShootingChannel.clip?.name}");
                    }
                }
            }
        }
        
        // Manual test
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("=== SOUND TEST ===");
            
            if (SoundManager.Instance != null)
            {
                Debug.Log("✅ SoundManager.Instance var");
                
                if (SoundManager.Instance.ShootingChannel != null)
                {
                    Debug.Log($"✅ ShootingChannel var, isPlaying: {SoundManager.Instance.ShootingChannel.isPlaying}");
                    Debug.Log($"Current clip: {SoundManager.Instance.ShootingChannel.clip?.name}");
                }
                
                if (SoundManager.Instance.P1911Shot != null)
                {
                    Debug.Log("✅ P1911Shot AudioClip yüklü");
                }
                
                if (SoundManager.Instance.M16Shot != null)
                {
                    Debug.Log("✅ M16Shot AudioClip yüklü");
                }
            }
            else
            {
                Debug.LogError("❌ SoundManager.Instance NULL!");
            }
        }
    }
}
