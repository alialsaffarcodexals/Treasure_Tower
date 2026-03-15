using UnityEditor;

public static class ReimportProjectArt
{
    [MenuItem("Tools/Assets/Reimport Managed Art Roots")]
    public static void ReimportManagedArtRoots()
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
            AssetDatabase.ImportAsset(root, ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);
        }

        AssetDatabase.Refresh();
    }
}
