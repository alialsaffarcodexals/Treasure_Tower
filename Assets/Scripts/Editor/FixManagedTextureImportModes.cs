using UnityEditor;
using UnityEngine;

public static class FixManagedTextureImportModes
{
    [MenuItem("Tools/Assets/Fix Managed Texture Import Modes")]
    public static void Run()
    {
        string[] roots =
        {
            "Assets/Art/Kenney",
            "Assets/Art/UI/Kenney",
            "Assets/Art/InputPrompts",
            "Assets/Art/Effects"
        };

        foreach (string root in roots)
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture", new[] { root });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                {
                    continue;
                }

                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.spriteImportMode = IsSheet(path) ? SpriteImportMode.Multiple : SpriteImportMode.Single;
                importer.SaveAndReimport();
            }
        }
    }

    private static bool IsSheet(string path)
    {
        return path.Contains("/Spritesheets/") ||
               path.Contains("/Spritesheet/") ||
               path.Contains("/Tilesheet/") ||
               path.Contains("_sheet_") ||
               path.Contains("tilesheet");
    }
}
