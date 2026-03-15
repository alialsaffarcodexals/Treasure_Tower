using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class UiVectorToTextureExporter
{
    private const string InputRoot = "Assets/Art/UI/Kenney/Vector";
    private const string OutputRoot = "Assets/Art/UI/Kenney/GeneratedTextures";

    [MenuItem("Tools/Assets/Export UI Vector SVGs To Textures")]
    public static void ExportAllUiVectors()
    {
        Directory.CreateDirectory(OutputRoot);

        string[] svgPaths = Directory.GetFiles(InputRoot, "*.svg", SearchOption.AllDirectories)
            .Select(path => path.Replace("\\", "/"))
            .ToArray();

        int exportedCount = 0;
        int skippedCount = 0;

        foreach (string assetPath in svgPaths)
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            Sprite sprite = assets.OfType<Sprite>().FirstOrDefault();
            if (sprite == null)
            {
                skippedCount++;
                Debug.LogWarning($"Skipped SVG without Sprite sub-asset: {assetPath}");
                continue;
            }

            string relativeFolder = Path.GetDirectoryName(assetPath) ?? string.Empty;
            relativeFolder = relativeFolder.Replace("\\", "/").Replace(InputRoot, OutputRoot);
            Directory.CreateDirectory(relativeFolder);

            string outputPath = Path.Combine(relativeFolder, Path.GetFileNameWithoutExtension(assetPath) + ".png")
                .Replace("\\", "/");

            Texture2D outputTexture = CopySpriteToTexture(sprite);
            File.WriteAllBytes(outputPath, outputTexture.EncodeToPNG());
            Object.DestroyImmediate(outputTexture);
            exportedCount++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"UI Vector Export complete. Exported {exportedCount} textures to {OutputRoot}. Skipped {skippedCount} files.");

        if (!Application.isBatchMode)
        {
            EditorUtility.DisplayDialog("UI Vector Export", $"Exported {exportedCount} texture files to {OutputRoot}. Skipped {skippedCount} files.", "OK");
        }
    }

    [MenuItem("Tools/Assets/Export UI Vector SVGs To Textures", true)]
    private static bool CanExportAllUiVectors()
    {
        return AssetDatabase.IsValidFolder(InputRoot);
    }

    private static Texture2D CopySpriteToTexture(Sprite sprite)
    {
        Rect rect = sprite.rect;
        int width = Mathf.RoundToInt(rect.width);
        int height = Mathf.RoundToInt(rect.height);

        RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
        RenderTexture previous = RenderTexture.active;

        try
        {
            Graphics.Blit(sprite.texture, renderTexture);
            RenderTexture.active = renderTexture;

            Texture2D fullTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            fullTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            fullTexture.Apply();
            return fullTexture;
        }
        finally
        {
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTexture);
        }
    }
}
