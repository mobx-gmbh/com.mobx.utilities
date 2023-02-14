using MobX.Utilities.Editor.Inspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MobX.Utilities.Editor.Windows
{
    public abstract class ConfigurationsEditorWindow : EditorWindow
    {
        #region Settings

        // Window State
        private FoldoutHandler Foldout { get; set; }
        private Vector2 _scrollPosition;
        private readonly List<Action> _headerInstructions = new List<Action>(8);
        private readonly List<Action> _footerInstructions = new List<Action>(8);
        private readonly List<Action> _instructions = new List<Action>(32);
        private readonly List<UnityEditor.Editor> _editorCache = new List<UnityEditor.Editor>(32);
        private bool _initialized;
        private bool? _drawOptions;
        private bool? _drawTitles;
        private bool? _saveSearchQuery;
        private string _searchQuery;
        private readonly List<(GUIContent name, string key)> _options = new();

        #endregion


        #region Settings

        private bool DrawOptions
        {
            get => _drawOptions ??= SessionState.GetBool(nameof(_drawOptions), false);
            set
            {
                _drawOptions = value;
                SessionState.SetBool(nameof(_drawOptions), value);
            }
        }

        private bool DrawTitles
        {
            get => _drawTitles ??= EditorPrefs.GetBool(nameof(_drawTitles), true);
            set
            {
                _drawTitles = value;
                EditorPrefs.SetBool(nameof(_drawTitles), value);
            }
        }

        private bool SaveSearchQuery
        {
            get => _saveSearchQuery ??= EditorPrefs.GetBool(nameof(_saveSearchQuery), true);
            set
            {
                _saveSearchQuery = value;
                EditorPrefs.SetBool(nameof(_saveSearchQuery), value);
            }
        }

        private string SearchQuery
        {
            get
            {
                _searchQuery ??= SaveSearchQuery ? EditorPrefs.GetString(nameof(_searchQuery), null) : null;
                return _searchQuery;
            }
            set
            {
                _searchQuery = value;
                if (SaveSearchQuery)
                {
                    EditorPrefs.SetString(nameof(_searchQuery), _searchQuery);
                }
            }
        }

        #endregion


        #region Setup

        protected virtual void OnEnable()
        {
            InitializeEditor();
        }

        protected virtual void OnDisable()
        {
            Foldout?.SaveState();
        }

        private void InitializeEditor()
        {
            foreach (var editor in _editorCache)
            {
                DestroyImmediate(editor);
            }

            Foldout = new FoldoutHandler(GetType().Name);

            _options.Clear();
            _instructions.Clear();
            _headerInstructions.Clear();
            SetupEditor();
            _initialized = true;
        }

        protected abstract void SetupEditor();

        #endregion


        #region GUI

        protected void OnGUI()
        {
            if (!_initialized)
            {
                return;
            }

            var foldoutStyle = FoldoutHandler.Style;
            DrawHeader();
            InspectorSearch.ResetContextQuery();
            DrawBody();
            InspectorSearch.ResetContextQuery();
            DrawFooter(foldoutStyle);
            InspectorSearch.ResetContextQuery();
        }

        private void DrawHeader()
        {
            FoldoutHandler.Style = FoldoutStyle.Dark;

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Space(8);
            SearchQuery = InspectorSearch.BeginSearchContext(SearchQuery);
            GUILayout.EndVertical();

            if (GUIHelper.ClearButton())
            {
                SearchQuery = string.Empty;
                InspectorSearch.EndSearchContext(true);
                GUI.FocusControl(null);
            }

            if (GUIHelper.OptionsButton())
            {
                DrawOptions = !DrawOptions;
            }

            if (GUIHelper.RefreshButton())
            {
                InitializeEditor();
            }

            GUILayout.EndHorizontal();

            if (DrawOptions)
            {
                DrawTitles = EditorGUILayout.ToggleLeft("Show Titles", DrawTitles);
                SaveSearchQuery = EditorGUILayout.ToggleLeft("Save Search Filter", SaveSearchQuery);

                foreach (var option in _options)
                {
                    var current = EditorPrefs.GetBool(option.key);
                    var result = EditorGUILayout.ToggleLeft(option.name, current);
                    EditorPrefs.SetBool(option.key, result);
                }
            }

            foreach (var instruction in _headerInstructions)
            {
                try
                {
                    instruction();
                }
                catch (Exception exception)
                {
                    GUIHelper.DrawException(exception);
                }
            }

            GUIHelper.DrawLine();
        }

        private void DrawBody()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (var instruction in _instructions)
            {
                instruction();
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndScrollView();
        }

        private void DrawFooter(FoldoutStyle foldoutStyle)
        {
            foreach (var instruction in _footerInstructions)
            {
                instruction();
            }

            InspectorSearch.EndSearchContext();
            FoldoutHandler.Style = foldoutStyle;
        }

        #endregion


        #region Editor Setup & Cache

        protected void AddHeaderInstruction(Action instruction)
        {
            _headerInstructions.AddUnique(instruction);
        }

        protected void AddFooterInstruction(Action instruction)
        {
            _footerInstructions.AddUnique(instruction);
        }

        protected void AddEditorGroup<T>(List<T> group, string editorTitle) where T : ScriptableObject
        {
            var editors = new (UnityEditor.Editor editor, string name)[group.Count];
            for (var i = 0; i < group.Count; i++)
            {
                var editor = UnityEditor.Editor.CreateEditor(group[i]);
                _editorCache.Add(editor);
                editors[i] = (editor, editor.target.name);
            }

            var foldout = new FoldoutHandler(typeof(T).Name);

            _instructions.Add(() =>
            {
                var foldoutStyle = FoldoutHandler.Style;
                FoldoutHandler.Style = FoldoutStyle.Dark;
                if (Foldout[editorTitle])
                {
                    //EditorGUI.indentLevel++;
                    EditorGUIUtility.wideMode = false;
                    EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth * 0.4f;
                    foreach (var (editor, displayName) in editors)
                    {
                        FoldoutHandler.Style = FoldoutStyle.Default;
                        if (foldout[displayName])
                        {
                            if (editor.serializedObject.targetObject == null)
                            {
                                EditorGUILayout.HelpBox("Target is null!", MessageType.Error);
                                return;
                            }

                            EditorGUI.indentLevel += 2;
                            FoldoutHandler.Style = FoldoutStyle.DarkGradient;
                            editor.serializedObject.Update();
                            editor.OnInspectorGUI();
                            editor.serializedObject.ApplyModifiedProperties();
                            FoldoutHandler.Style = foldoutStyle;
                            EditorGUI.indentLevel -= 2;
                        }
                    }

                    //EditorGUI.indentLevel--;
                    GUIHelper.Space(!InspectorSearch.IsActive);
                }

                foldout.SaveState();
            });
        }

        protected void AddTitle(string titleName, bool drawLine = true)
        {
            _instructions.Add(() =>
            {
                InspectorSearch.SetContextQuery(titleName);
                if (!InspectorSearch.IsValid(titleName))
                {
                    return;
                }

                if (!DrawTitles)
                {
                    return;
                }

                if (drawLine)
                {
                    var lastRect = GUILayoutUtility.GetLastRect();
                    EditorGUI.DrawRect(new Rect(0, lastRect.y, EditorGUIUtility.currentViewWidth, 1),
                        new Color(0f, 0f, 0f, 0.3f));
                }

                EditorGUILayout.Space();
                GUILayout.Label(titleName, new GUIStyle(GUI.skin.label) {fontSize = 16});
                EditorGUILayout.Space(3);
            });
        }

        protected void AddEditor<T>(T target, string editorTitle) where T : ScriptableObject
        {
            if (target == null)
            {
                return;
            }

            var editor = UnityEditor.Editor.CreateEditor(target);
            _editorCache.Add(editor);

            if (editor != null)
            {
                _instructions.Add(() =>
                {
                    var foldoutStyle = FoldoutHandler.Style;
                    FoldoutHandler.Style = FoldoutStyle.Dark;
                    if (Foldout[editorTitle])
                    {
                        if (editor.serializedObject.targetObject == null)
                        {
                            EditorGUILayout.HelpBox("Target is null!", MessageType.Error);
                            return;
                        }

                        FoldoutHandler.Style = FoldoutStyle.DarkGradient;
                        EditorGUIUtility.wideMode = false;
                        EditorGUI.indentLevel++;
                        EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth * 0.4f;
                        editor.serializedObject.Update();
                        editor.OnInspectorGUI();
                        editor.serializedObject.ApplyModifiedProperties();
                        EditorGUI.indentLevel--;
                        GUIHelper.Space(!InspectorSearch.IsActive);
                        FoldoutHandler.Style = foldoutStyle;
                    }
                });
            }
            else
            {
                _instructions.Add(() =>
                {
                    if (Foldout[editorTitle])
                    {
                        EditorGUIUtility.wideMode = false;
                        EditorGUI.indentLevel++;
                        EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth * 0.4f;
                        EditorGUILayout.HelpBox(
                            $"{editorTitle} is null! Did you forget to assign the variable in the Configurations Prefab in the Resources folder?",
                            MessageType.Error);
                        EditorGUI.indentLevel--;
                        GUIHelper.Space();
                    }
                });
            }
        }

        protected void AddOptionalEditor<T>(T target, string editorTitle, string optionName, string tooltip = null,
            bool showByDefault = false) where T : ScriptableObject
        {
            var editor = UnityEditor.Editor.CreateEditor(target);
            _editorCache.Add(editor);

            var optionKey = $"custom_editor_{optionName}";
            AddOptions((new GUIContent(optionName, tooltip), optionKey));
            if (!EditorPrefs.HasKey(optionKey))
            {
                EditorPrefs.SetBool(optionKey, showByDefault);
            }

            if (editor != null)
            {
                _instructions.Add(() =>
                {
                    if (!EditorPrefs.GetBool(optionKey, showByDefault))
                    {
                        return;
                    }

                    var foldoutStyle = FoldoutHandler.Style;
                    FoldoutHandler.Style = FoldoutStyle.Dark;
                    if (Foldout[editorTitle])
                    {
                        if (editor.serializedObject.targetObject == null)
                        {
                            EditorGUILayout.HelpBox("Target is null!", MessageType.Error);
                            return;
                        }

                        FoldoutHandler.Style = FoldoutStyle.DarkGradient;
                        EditorGUIUtility.wideMode = false;
                        EditorGUI.indentLevel++;
                        EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth * 0.4f;
                        editor.serializedObject.Update();
                        editor.OnInspectorGUI();
                        editor.serializedObject.ApplyModifiedProperties();
                        EditorGUI.indentLevel--;
                        GUIHelper.Space(!InspectorSearch.IsActive);
                        FoldoutHandler.Style = foldoutStyle;
                    }
                });
            }
            else
            {
                _instructions.Add(() =>
                {
                    if (!EditorPrefs.GetBool(optionKey, showByDefault))
                    {
                        return;
                    }

                    if (Foldout[editorTitle])
                    {
                        EditorGUIUtility.wideMode = false;
                        EditorGUI.indentLevel++;
                        EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth * 0.4f;
                        EditorGUILayout.HelpBox(
                            $"{editorTitle} is null! Did you forget to assign the variable in the Configurations Prefab in the Resources folder?",
                            MessageType.Error);
                        EditorGUI.indentLevel--;
                        GUIHelper.Space();
                    }
                });
            }
        }

        protected void AddOptionalTitle(string titleName, string optionName, string tooltip = null,
            bool showByDefault = false, bool drawLine = true)
        {
            var optionKey = $"custom_editor_{optionName}";
            AddOptions((new GUIContent(optionName, tooltip), optionKey));
            if (!EditorPrefs.HasKey(optionKey))
            {
                EditorPrefs.SetBool(optionKey, showByDefault);
            }

            _instructions.Add(() =>
            {
                if (!EditorPrefs.GetBool(optionKey, showByDefault))
                {
                    return;
                }

                InspectorSearch.SetContextQuery(titleName);
                if (!InspectorSearch.IsValid(titleName))
                {
                    return;
                }

                if (!DrawTitles)
                {
                    return;
                }

                if (drawLine)
                {
                    var lastRect = GUILayoutUtility.GetLastRect();
                    EditorGUI.DrawRect(new Rect(0, lastRect.y, EditorGUIUtility.currentViewWidth, 1),
                        new Color(0f, 0f, 0f, 0.3f));
                }

                EditorGUILayout.Space();
                GUILayout.Label(titleName, new GUIStyle(GUI.skin.label) {fontSize = 16});
                EditorGUILayout.Space(3);
            });
        }

        protected void AddInstruction(Action instruction)
        {
            _instructions.Add(instruction);
        }

        protected void AddInstruction(Action instruction, string editorTitle)
        {
            _instructions.Add(() =>
            {
                if (Foldout[editorTitle])
                {
                    EditorGUILayout.Space();
                    EditorGUIUtility.wideMode = false;
                    EditorGUI.indentLevel++;
                    instruction();
                    EditorGUI.indentLevel--;
                    GUIHelper.Space();
                }
            });
        }

        private void AddOptions((GUIContent content, string key) option)
        {
            var key = option.key;
            foreach (var (_, elementKey) in _options)
            {
                if (key == elementKey)
                {
                    return;
                }
            }

            _options.Add(option);
        }

        #endregion
    }
}