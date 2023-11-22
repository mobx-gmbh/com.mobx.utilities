namespace MobX.Utilities.Callbacks
{
#if UNITY_EDITOR
    public class BuildPreprocessor : UnityEditor.Build.IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            Gameloop.RaiseCallback("PreprocessBuild");
        }
    }
#endif
}
