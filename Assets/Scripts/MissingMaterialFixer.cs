using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class MissingMaterialFixer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void FixMissingMaterials()
    {
        // Fallback materyalleri Resources'tan yükle
        var diffuse = Resources.Load<Material>("Materials/Fallback/Diffuse_01_Fallback");
        var glass = Resources.Load<Material>("Materials/Fallback/Glass_Fallback");
        var neon = Resources.Load<Material>("Materials/Fallback/Neon_01_Fallback");

        // Görsel sorunları önlemek için varsayılanı OPAQUE yap
        const bool ForceOpaqueFallback = true; // cam/ışık özel durumları hariç her şeyi diffuse yap

        if (diffuse == null)
        {
            Debug.LogWarning("MissingMaterialFixer: Diffuse_01_Fallback bulunamadı (Resources).");
        }

        var allRenderers = Object.FindObjectsOfType<MeshRenderer>(true);
        foreach (var r in allRenderers)
        {
            bool updated = false;
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                var m = mats[i];
                if (m == null || m.name == "Default-Material")
                {
                    var name = r.gameObject.name.ToLower();

                    // Cam atamasını sadece çok net isim eşleşmelerinde yap
                    bool looksLikeGlass = false;
                    if (!ForceOpaqueFallback)
                    {
                        var tokens = Regex.Split(name, @"[^a-z0-9]+");
                        looksLikeGlass = tokens.Any(t => t == "glass" || t == "window" || t == "glasspanel" || t == "windowpane");
                    }

                    if (!ForceOpaqueFallback && glass != null && looksLikeGlass)
                    {
                        mats[i] = glass;
                    }
                    else if (!ForceOpaqueFallback && neon != null && (name.Contains("neon_") || name.Contains("light_strip") || name.Contains("lightwall")))
                    {
                        mats[i] = neon;
                    }
                    else if (diffuse != null)
                    {
                        mats[i] = diffuse;
                    }
                    updated = true;
                }
            }
            if (updated)
            {
                r.sharedMaterials = mats;
            }
        }

        // SkinnedMeshRenderer ihtimali
        var skinned = Object.FindObjectsOfType<SkinnedMeshRenderer>(true);
        foreach (var r in skinned)
        {
            bool updated = false;
            var mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                var m = mats[i];
                if (m == null || m.name == "Default-Material")
                {
                    if (diffuse != null) mats[i] = diffuse;
                    updated = true;
                }
            }
            if (updated) r.sharedMaterials = mats;
        }
    }
}
