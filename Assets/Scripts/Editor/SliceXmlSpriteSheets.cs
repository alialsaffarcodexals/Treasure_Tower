using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public static class SliceXmlSpriteSheets
{
    [MenuItem("Tools/Assets/Slice XML Sprite Sheets")]
    public static void Run()
    {
        string[] roots =
        {
            "Assets/Art/Kenney",
            "Assets/Art/InputPrompts"
        };

        foreach (string root in roots)
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture", new[] { root });
            foreach (string guid in guids)
            {
                string texturePath = AssetDatabase.GUIDToAssetPath(guid);
                if (!IsAtlasCandidate(texturePath))
                {
                    continue;
                }

                string xmlPath = Path.ChangeExtension(texturePath, ".xml");
                if (!File.Exists(xmlPath))
                {
                    continue;
                }

                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
                if (texture == null || importer == null)
                {
                    continue;
                }

                SpriteMetaData[] metas = BuildMetaData(texture.height, xmlPath);
                if (metas.Length == 0)
                {
                    continue;
                }

#pragma warning disable CS0618
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.spritesheet = metas;
#pragma warning restore CS0618
                importer.SaveAndReimport();
            }
        }
    }

    private static bool IsAtlasCandidate(string texturePath)
    {
        return texturePath.Contains("/Spritesheet/") ||
               texturePath.Contains("/Spritesheets/") ||
               texturePath.Contains("/Tilesheet/") ||
               texturePath.Contains("_sheet");
    }

    private static SpriteMetaData[] BuildMetaData(int textureHeight, string xmlPath)
    {
        XDocument doc = XDocument.Load(xmlPath);
        var nodes = doc.Descendants("SubTexture");
        var list = new System.Collections.Generic.List<SpriteMetaData>();

        foreach (XElement node in nodes)
        {
            string name = node.Attribute("name")?.Value;
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            int x = ParseInt(node.Attribute("x")?.Value);
            int y = ParseInt(node.Attribute("y")?.Value);
            int width = ParseInt(node.Attribute("width")?.Value);
            int height = ParseInt(node.Attribute("height")?.Value);

            SpriteMetaData meta = new SpriteMetaData
            {
                name = Path.GetFileNameWithoutExtension(name),
                alignment = (int)SpriteAlignment.Center,
                pivot = new Vector2(0.5f, 0.5f),
                rect = new Rect(x, textureHeight - y - height, width, height)
            };

            list.Add(meta);
        }

        return list.ToArray();
    }

    private static int ParseInt(string value)
    {
        return int.TryParse(value, out int parsed) ? parsed : 0;
    }
}
