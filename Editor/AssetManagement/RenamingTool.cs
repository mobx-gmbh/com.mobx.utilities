using MobX.Utilities.Inspector;
using MobX.Utilities.Types;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Editor.AssetManagement
{
    public class AssetRenamingTool : LocalEditorConfiguration<AssetRenamingTool>
    {
        #region Type Definitions

        private enum RenameMode
        {
            None = 0,
            Humanize = 1,
            Dehumanize = 2
        }

        [Serializable]
        private struct Replacement
        {
            public string oldValue;
            public string newValue;
        }

        #endregion


        #region Shared Settings

        [Foldout("GameObjects")]
        [SerializeField] private RenameMode gameObjectRenaming = RenameMode.Humanize;
        [SerializeField] private bool gameObjectIgnoreCase = true;
        [SerializeField] private Optional<string> gameObjectPrefix;
        [SerializeField] private Optional<string> gameObjectSuffix;

        [Header("Numbering")]
        [SerializeField] private bool numberGameObjects;
        [SerializeField] private int gameObjectStartNumber = 1;
        [SerializeField] private Optional<string> gameObjectNumberSeparator = new(null, false);
        [SerializeField] private Optional<string> gameObjectNumberFormat = new("(00)", true);

        [Header("Patterns")]
        [SerializeField] private Optional<string>[] gameObjectPatternsToRemove =
        {
            @"\(\d+\)"
        };
        [SerializeField] private Optional<string>[] gameObjectStringsToRemove;
        [SerializeField] private Optional<Replacement>[] gameObjectStringsToReplace;

        [Header("Example")]
        [Foldout("GameObjects")]
        [SerializeField] private string gameObjectExampleName = "exampleName";

        [ShowInInspector]
        [Foldout("GameObjects")]
        public string GameObjectResult => FormatGameObjectName(gameObjectExampleName, gameObjectStartNumber);

        [Foldout("Assets")]
        [SerializeField] private RenameMode assetRenaming = RenameMode.Dehumanize;
        [SerializeField] private bool assetIgnoreCase = true;
        [SerializeField] private Optional<string> assetPrefix;
        [SerializeField] private Optional<string> assetSuffix;

        [Header("Numbering")]
        [SerializeField] private bool numberAssets;
        [SerializeField] private int assetStartNumber = 1;
        [SerializeField] private Optional<string> assetNumberSeparator = new("_", true);
        [SerializeField] private Optional<string> assetNumberFormat = new("00", true);

        [Header("Patterns")]
        [SerializeField] private Optional<string>[] assetPatternsToRemove =
        {
            @"\(\d+\)"
        };
        [SerializeField] private Optional<string>[] assetStringsToRemove;
        [SerializeField] private Optional<Replacement>[] assetStringsToReplace;

        [Header("Example")]
        [Foldout("Assets")]
        [SerializeField] private string assetExampleName = "exampleAssetName";

        [ShowInInspector]
        [Foldout("Assets")]
        public string AssetResult => FormatAssetName(assetExampleName, gameObjectStartNumber);

        #endregion


        #region Buttons

        [Button]
        [Foldout("Selection")]
        public void FormatSelectedObjects()
        {
            FormatGameObjectsInternal(GameObjects);
            FormatAssetsInternal(Assets);
        }

        [Button]
        [Foldout("GameObjects")]
        public void FormatSelectedGameObjects()
        {
            FormatGameObjectsInternal(GameObjects);
        }

        [Button]
        [Foldout("Assets")]
        public void FormatSelectedAssets()
        {
            FormatAssetsInternal(Assets);
        }

        #endregion


        #region GameObjects

        private void FormatGameObjectsInternal(GameObject[] gameObjects)
        {
            var index = gameObjectStartNumber;

            UnityEditor.Undo.RecordObjects(ArrayUtility.Cast<GameObject, Object>(gameObjects), "Rename Objects");

            foreach (var element in gameObjects)
            {
                var elementName = FormatGameObjectName(element.name, index++);

                UnityEditor.Undo.RegisterCompleteObjectUndo(element, "Rename Objects");
                element.name = elementName;
            }
            UnityEditor.Undo.CollapseUndoOperations(UnityEditor.Undo.GetCurrentGroup());
        }

        private string FormatGameObjectName(string currentName, int index)
        {
            try
            {
                var elementName = gameObjectPrefix.ValueOrDefault(string.Empty) + currentName + gameObjectSuffix.ValueOrDefault(string.Empty);

                foreach (var pattern in gameObjectPatternsToRemove)
                {
                    if (!pattern.Enabled)
                    {
                        continue;
                    }
                    var regex = new Regex(pattern);
                    elementName = regex.Replace(elementName, string.Empty);
                }

                foreach (var toRemove in gameObjectStringsToRemove)
                {
                    if (toRemove.TryGetValue(out var value) && value.IsNotNullOrWhitespace())
                    {
                        elementName = elementName.Replace(value, string.Empty, gameObjectIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                    }
                }

                if (gameObjectRenaming == RenameMode.Humanize)
                {
                    elementName = elementName.Humanize();
                }
                else if (gameObjectRenaming == RenameMode.Dehumanize)
                {
                    elementName = elementName.Dehumanize();
                }

                foreach (var replacement in gameObjectStringsToReplace)
                {
                    if (replacement.TryGetValue(out var value))
                    {
                        elementName = elementName.Replace(value.oldValue ?? string.Empty, value.newValue ?? string.Empty, gameObjectIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                    }
                }

                if (numberGameObjects)
                {
                    elementName = elementName.Trim();

                    var number = index.ToString(gameObjectNumberFormat.ValueOrDefault());

                    if (gameObjectNumberSeparator.TryGetValue(out var separator) && !elementName.EndsWith(separator))
                    {
                        elementName += separator;
                    }

                    elementName += number;
                }

                elementName = elementName.Trim();
                return elementName;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        #endregion


        #region Assets

        private void FormatAssetsInternal(Object[] assets)
        {
            var index = assetStartNumber;
            try
            {
                UnityEditor.Undo.RecordObjects(assets, "Rename Objects");
                UnityEditor.AssetDatabase.StartAssetEditing();

                foreach (var element in assets)
                {
                    var elementName = FormatAssetName(element.name, index++);

                    var path = UnityEditor.AssetDatabase.GetAssetPath(element);
                    UnityEditor.AssetDatabase.RenameAsset(path, elementName);
                }

                UnityEditor.AssetDatabase.StopAssetEditing();
                UnityEditor.Undo.CollapseUndoOperations(UnityEditor.Undo.GetCurrentGroup());
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogException(exception);
                UnityEditor.AssetDatabase.StopAssetEditing();
            }
        }

        private string FormatAssetName(string currentName, int index)
        {
            try
            {
                var elementName = assetPrefix.ValueOrDefault(string.Empty) + currentName + assetSuffix.ValueOrDefault(string.Empty);

                foreach (var pattern in assetPatternsToRemove)
                {
                    if (!pattern.Enabled)
                    {
                        continue;
                    }
                    var regex = new Regex(pattern);
                    elementName = regex.Replace(elementName, string.Empty);
                }

                if (assetRenaming == RenameMode.Humanize)
                {
                    elementName = elementName.Humanize();
                }
                else if (assetRenaming == RenameMode.Dehumanize)
                {
                    elementName = elementName.Dehumanize();
                }

                foreach (var toRemove in assetStringsToRemove)
                {
                    if (toRemove.Enabled)
                    {
                        elementName = elementName.Replace(toRemove.ValueOrDefault(string.Empty) ?? string.Empty, string.Empty, assetIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                    }
                }

                foreach (var replacement in assetStringsToReplace)
                {
                    if (replacement.TryGetValue(out var value))
                    {
                        elementName = elementName.Replace(value.oldValue ?? string.Empty, value.newValue ?? string.Empty, assetIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                    }
                }

                if (numberAssets)
                {
                    var number = assetRenaming == RenameMode.Humanize && !elementName.EndsWith(" ")
                        ? " " + index.ToString(assetNumberFormat)
                        : index.ToString(assetNumberFormat);

                    elementName += $"{assetNumberSeparator.ValueOrDefault(string.Empty)}{number}";
                }

                elementName = elementName.Trim();
                return elementName;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        #endregion


        #region Shortcuts

        [Foldout("Selection")]
        [ShowInInspector]
        public Object[] Selection => UnityEditor.Selection.objects;

        [Foldout("Selection")]
        [ShowInInspector]
        public GameObject[] GameObjects => UnityEditor.Selection.gameObjects;

        [Foldout("Selection")]
        [ShowInInspector]
        public Object[] Assets => UnityEditor.Selection.objects.Where(element => !element.IsGameObjectInScene()).ToArray();

        #endregion
    }
}
