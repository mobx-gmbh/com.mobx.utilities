using UnityEngine;

namespace MobX.Utilities.Editor.Helper
{
    public static class MenuItemLayout
    {
        [UnityEditor.MenuItem("MobX/HideFlags/Show Hidden GameObjects", priority = 2300)]
        private static void ShowAllHiddenObjects()
        {
            HideFlagsUtility.ShowAllHiddenObjects();
        }

        [UnityEditor.MenuItem("MobX/HideFlags/Show Hidden Inspector", priority = 2300)]
        private static void ShowAllHiddenInspector()
        {
            HideFlagsUtility.ShowAllHiddenInspector();
        }

        [UnityEditor.MenuItem("MobX/HideFlags/Validate Hide Flags", priority = 2300)]
        private static void ValidateAllObjectsHideFlags()
        {
            HideFlagsUtility.ValidateAllObjectsHideFlags();
        }

        [UnityEditor.MenuItem("MobX/EditorPrefs/Clear All EditorPrefs", priority = 280)]
        private static void ClearEditorPrefs()
        {
            UnityEditor.EditorPrefs.DeleteAll();
        }

        [UnityEditor.MenuItem("MobX/PlayerPrefs/Clear All EditorPrefs", priority = 280)]
        private static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}