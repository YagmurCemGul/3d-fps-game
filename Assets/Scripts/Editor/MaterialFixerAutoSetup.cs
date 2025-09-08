using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class MaterialFixerAutoSetup
{
    static MaterialFixerAutoSetup()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    
    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // Play mode'a geçildiğinde material fixer'ı oluştur
            EnsureMaterialFixerExists();
        }
    }
    
    static void EnsureMaterialFixerExists()
    {
        if (Application.isPlaying)
        {
            GameStartMaterialFixer fixer = Object.FindObjectOfType<GameStartMaterialFixer>();
            if (fixer == null)
            {
                GameObject materialFixerObj = new GameObject("GameStartMaterialFixer");
                materialFixerObj.AddComponent<GameStartMaterialFixer>();
                Object.DontDestroyOnLoad(materialFixerObj);
                
                Debug.Log("MaterialFixerAutoSetup: GameStartMaterialFixer otomatik olarak oluşturuldu.");
            }
        }
    }
    
    [MenuItem("Tools/Fix Materials Now")]
    static void FixMaterialsNow()
    {
        if (Application.isPlaying)
        {
            GameStartMaterialFixer fixer = Object.FindObjectOfType<GameStartMaterialFixer>();
            if (fixer != null)
            {
                fixer.FixMaterials();
            }
            else
            {
                EnsureMaterialFixerExists();
                fixer = Object.FindObjectOfType<GameStartMaterialFixer>();
                if (fixer != null)
                {
                    fixer.FixMaterials();
                }
            }
        }
        else
        {
            Debug.LogWarning("Material düzeltmesi sadece Play Mode'da çalışır. Oyunu başlatın ve tekrar deneyin.");
        }
    }
    
    [MenuItem("Tools/Create Material Fixer")]
    static void CreateMaterialFixer()
    {
        if (Application.isPlaying)
        {
            EnsureMaterialFixerExists();
            Debug.Log("Material Fixer oluşturuldu.");
        }
        else
        {
            Debug.Log("Play mode'a geçin, Material Fixer otomatik oluşturulacak.");
        }
    }
}
