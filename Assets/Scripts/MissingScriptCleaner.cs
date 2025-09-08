using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
public class MissingScriptCleaner : EditorWindow
{
    [MenuItem("Tools/Clean Missing Scripts")]
    public static void ShowWindow()
    {
        GetWindow<MissingScriptCleaner>("Missing Script Cleaner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Missing Script Cleaner", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Clean Missing Scripts in Current Scene"))
        {
            CleanMissingScriptsInScene();
        }
        
        if (GUILayout.Button("Clean Missing Scripts in All Prefabs"))
        {
            CleanMissingScriptsInPrefabs();
        }
        
        if (GUILayout.Button("Clean Everything"))
        {
            CleanMissingScriptsInScene();
            CleanMissingScriptsInPrefabs();
        }
    }

    private static void CleanMissingScriptsInScene()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int removedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            Component[] components = obj.GetComponents<Component>();
            for (int i = components.Length - 1; i >= 0; i--)
            {
                if (components[i] == null)
                {
                    Debug.Log($"Removing missing script from: {obj.name}");
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
                    removedCount++;
                    break;
                }
            }
        }

        Debug.Log($"Cleaned {removedCount} missing scripts from scene objects.");
        EditorUtility.SetDirty(SceneManager.GetActiveScene().GetRootGameObjects()[0]);
    }

    private static void CleanMissingScriptsInPrefabs()
    {
        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab");
        int removedCount = 0;

        foreach (string path in prefabPaths)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(path);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            
            if (prefab != null)
            {
                bool hasMissingScript = false;
                Component[] components = prefab.GetComponentsInChildren<Component>(true);
                
                foreach (Component comp in components)
                {
                    if (comp == null)
                    {
                        hasMissingScript = true;
                        break;
                    }
                }
                
                if (hasMissingScript)
                {
                    Debug.Log($"Cleaning missing scripts from prefab: {assetPath}");
                    GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    
                    // Clean all missing scripts from the instance and its children
                    CleanMissingScriptsFromGameObject(instance);
                    
                    // Save back to prefab
                    PrefabUtility.SaveAsPrefabAsset(instance, assetPath);
                    DestroyImmediate(instance);
                    removedCount++;
                }
            }
        }

        Debug.Log($"Cleaned missing scripts from {removedCount} prefabs.");
        AssetDatabase.Refresh();
    }

    private static void CleanMissingScriptsFromGameObject(GameObject obj)
    {
        // Clean this object
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
        
        // Clean all children recursively
        foreach (Transform child in obj.transform)
        {
            CleanMissingScriptsFromGameObject(child.gameObject);
        }
    }
}
#endif
