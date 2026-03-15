using UnityEditor;
using UnityEngine;

public sealed class KenneyAssetImportPostprocessor : AssetPostprocessor
{
    private static readonly string[] TextureRoots =
    {
        "Assets/Art/Kenney/",
        "Assets/Art/UI/Kenney/",
        "Assets/Art/InputPrompts/",
        "Assets/Art/Effects/"
    };

    private static readonly string[] AudioRoots =
    {
        "Assets/Audio/SFX/Kenney/",
        "Assets/Audio/Music/Kenney/"
    };

    private void OnPreprocessTexture()
    {
        if (!StartsWithAny(assetPath, TextureRoots))
        {
            return;
        }

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.alphaIsTransparency = true;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.spriteImportMode =
            assetPath.Contains("/Spritesheets/") ||
            assetPath.Contains("/Spritesheet/") ||
            assetPath.Contains("/Tilesheet/") ||
            assetPath.Contains("_sheet_") ||
            assetPath.Contains("tilesheet")
            ? SpriteImportMode.Multiple
            : SpriteImportMode.Single;
    }

    private void OnPreprocessAudio()
    {
        if (!StartsWithAny(assetPath, AudioRoots))
        {
            return;
        }

        AudioImporter importer = (AudioImporter)assetImporter;
        AudioImporterSampleSettings settings = importer.defaultSampleSettings;
        settings.loadType = AudioClipLoadType.DecompressOnLoad;
        settings.compressionFormat = AudioCompressionFormat.PCM;
        settings.quality = 1f;
        settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
        settings.preloadAudioData = true;
        importer.defaultSampleSettings = settings;
    }

    private static bool StartsWithAny(string path, string[] roots)
    {
        foreach (string root in roots)
        {
            if (path.StartsWith(root))
            {
                return true;
            }
        }

        return false;
    }
}
