using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 500f;

    float xRotation = 0f;
    float yRotation = 0f;

    public float topClamp = -90f;
    public float bottomClamp = 90f;
    
    private bool gameIsPaused = false;
    
    void Start()
    {
        LockCursor();
    }

    void Update()
    {
        HandleInput();
    }
    
    void FixedUpdate()
    {
        // FixedUpdate'de de cursor kontrolü (physics ile çakışmaları önler)
        if (!gameIsPaused && Cursor.lockState != CursorLockMode.Locked)
        {
            Debug.LogWarning("FixedUpdate: Cursor unlock! Düzeltiliyor...");
            LockCursor();
        }
    }
    
    void LateUpdate()
    {
        // Her frame'in sonunda cursor'ı kontrol et - daha agresif
        if (!gameIsPaused)
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Debug.LogWarning("LateUpdate: Cursor unlock edildi! Tekrar kilitliyorum...");
                LockCursor();
            }
            
            if (Cursor.visible)
            {
                Debug.LogWarning("LateUpdate: Cursor görünür! Gizliyorum...");
                Cursor.visible = false;
            }
        }
    }
    
    void HandleInput()
    {
        // F tuşu debug - InteractionManager ile çakışma kontrolü
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("MouseMovement: F tuşu algılandı - Cursor durumu: " + Cursor.lockState);
        }
        
        // Mouse left click debug
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("MouseMovement: Sol mouse tuşu algılandı - Cursor durumu: " + Cursor.lockState);
        }
        
        // ESC tuşu ile oyunu duraklat/devam ettir
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        
        // Mouse tıklaması ile oyuna dön (SADECE oyun durduğunda)
        if (Input.GetMouseButtonDown(0) && gameIsPaused)
        {
            Debug.Log("MouseMovement: Oyun pause'dan devam ediyor");
            ResumeGame();
        }
        
        // Sadece oyun durumda değilse mouse kontrolü çalışsın
        if (!gameIsPaused && Cursor.lockState == CursorLockMode.Locked)
        {
            HandleMouseMovement();
        }
    }
    
    void HandleMouseMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);
        yRotation += mouseX;
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
    
    void LockCursor()
    {
        Debug.Log("LockCursor çağrıldı - mevcut durum: " + Cursor.lockState);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("LockCursor tamamlandı - yeni durum: " + Cursor.lockState);
    }
    
    void PauseGame()
    {
        gameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f; // Oyunu duraklat
    }
    
    void ResumeGame()
    {
        gameIsPaused = false;
        Time.timeScale = 1f; // Oyunu devam ettir
        LockCursor();
    }
    
    // Unity Editor'da focus değiştiğinde cursor problemini önle
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && !gameIsPaused)
        {
            Invoke("LockCursor", 0.1f); // Biraz gecikme ile cursor'ı kilitle
        }
    }
    
    // Unity Editor'da pause/unpause durumlarında cursor kontrolü
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && !gameIsPaused)
        {
            Invoke("LockCursor", 0.1f);
        }
    }
}
