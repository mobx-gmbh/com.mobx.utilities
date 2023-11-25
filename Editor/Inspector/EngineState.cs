namespace MobX.Utilities.Editor.Inspector
{
    internal static class EngineState
    {
        public static int Value { get; private set; }

#if UNITY_EDITOR
        static EngineState()
        {
            UnityEditor.EditorApplication.playModeStateChanged -= EditorApplicationOnplayModeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
        }

        private static void EditorApplicationOnplayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            Value = (int) state;
        }
#endif
    }
}