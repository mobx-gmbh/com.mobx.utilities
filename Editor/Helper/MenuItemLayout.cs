namespace MobX.Utilities.Editor.Helper
{
    public static class MenuItemLayout
    {
        [UnityEditor.MenuItem("Tools/HideFlags/Show Hidden GameObjects", priority = 2300)]
        private static void ShowAllHiddenObjects()
        {
            HideFlagsUtility.ShowAllHiddenObjects();
        }

        [UnityEditor.MenuItem("Tools/HideFlags/Show Hidden Inspector", priority = 2300)]
        private static void ShowAllHiddenInspector()
        {
            HideFlagsUtility.ShowAllHiddenInspector();
        }

        [UnityEditor.MenuItem("Tools/HideFlags/Validate Hide Flags", priority = 2300)]
        private static void ValidateAllObjectsHideFlags()
        {
            HideFlagsUtility.ValidateAllObjectsHideFlags();
        }

        [UnityEditor.MenuItem("Edit/Clear All EditorPrefs", priority = 280)]
        private static void ClearEditorPrefs()
        {
            UnityEditor.EditorPrefs.DeleteAll();
        }
    }
}