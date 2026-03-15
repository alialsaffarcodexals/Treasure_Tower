using UnityEditor;
using UnityEngine;

public sealed class PlayerSpriteImportPostprocessor : AssetPostprocessor
{
    private const string PlayerSpriteRoot = "Assets/Art/Sprites/Player/";

    private void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(PlayerSpriteRoot))
        {
            return;
        }

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
    }
}
