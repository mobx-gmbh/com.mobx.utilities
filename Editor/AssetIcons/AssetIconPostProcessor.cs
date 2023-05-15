namespace MobX.Utilities.Editor.AssetIcons
{
    public class AssetIconPostProcessor : UnityEditor.AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            AssetIconSettings.instance.ValidateAssetPaths(importedAssets);
        }
    }
}
