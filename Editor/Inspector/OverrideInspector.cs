using MobX.Utilities.Inspector;
using MobX.Utilities.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Editor.Inspector
{
    public class OverrideInspector<TObject> : StyledInspector<TObject> where TObject : Object
    {
        #region Fields

        private UnityEditor.SerializedProperty _script;
        private string _filterString;
        private bool _useDefaultInspector;
        private bool _hideMonoScript;
        private bool _showFilterField;
        private bool _hasDescription;
        private GUIContent _description;
        private GUIContent _descriptionTitle;
        private readonly Dictionary<FoldoutData, List<InspectorMember>> _headerFields = new Dictionary<FoldoutData, List<InspectorMember>>(10);
        private readonly List<InspectorMember> _headerLessFields = new List<InspectorMember>(8);

        private string Filter => InspectorSearch.IsActive ? InspectorSearch.Filter : _filterString;

        #endregion


        #region Setup

        protected override void OnEnable()
        {
            if (target == null || serializedObject.targetObject == null)
            {
                _useDefaultInspector = true;
                return;
            }

            Type targetType = target.GetType();
            DefaultInspectorAttribute foldoutInspectorAttribute = targetType.GetCustomAttribute<DefaultInspectorAttribute>(true);
            _useDefaultInspector = foldoutInspectorAttribute != null;

            if (_useDefaultInspector)
            {
                return;
            }

            base.OnEnable();

            _script = serializedObject.FindProperty("m_Script");
            _hideMonoScript = targetType.HasAttribute<HideMonoScriptAttribute>(true);

            if (targetType.TryGetCustomAttribute(out DescriptionAttribute descriptionAttribute))
            {
                _description = new GUIContent(descriptionAttribute.Description, descriptionAttribute.Description);
                _descriptionTitle = new GUIContent("Description", descriptionAttribute.Description);
                _hasDescription = true;
            }

            FoldoutData activeHeader = null;

            InspectorMember[] inspectorMembers = InspectorFieldUtils.GetInspectorMembers(serializedObject);
            var count = 0;
            for (var i = 0; i < inspectorMembers.Length; i++)
            {
                InspectorMember inspectorMember = inspectorMembers[i];

                if (inspectorMember.Member.TryGetCustomAttribute(out FoldoutAttribute attribute))
                {
                    var title = attribute.FoldoutName switch
                    {
                        FoldoutName.ObjectName => Target.name,
                        FoldoutName.HumanizedObjectName => Target.name.Humanize(),
                        _ => attribute.Title
                    };

                    activeHeader = new FoldoutData(title, attribute.ToolTip);
                    var defaultState = attribute.Unfold;
                    if (!_headerFields.TryAdd(activeHeader, new List<InspectorMember>
                    {
                        inspectorMember
                    }))
                    {
                        _headerFields[activeHeader].Add(inspectorMember);
                    }

                    SetDefaultFoldoutState(activeHeader, defaultState);

                    count++;
                }
                else
                {
                    if (activeHeader.Title == null)
                    {
                        _headerLessFields.Add(inspectorMember);
                        count++;
                        continue;
                    }

                    _headerFields[activeHeader].Add(inspectorMember);
                    count++;
                }
            }

            _showFilterField =
                targetType.TryGetCustomAttribute(out SearchField searchAttribute)
                    ? searchAttribute.Enabled
                    : count > 8;
        }

        protected override void OnDisable()
        {
            if (_useDefaultInspector)
            {
                return;
            }

            base.OnDisable();
        }

        #endregion


        #region GUI

        public override void OnInspectorGUI()
        {
            if (_useDefaultInspector || targets.Length >= 2)
            {
                base.OnInspectorGUI();
                return;
            }

            GUIHelper.Space(!InspectorSearch.IsActive);

            if (_hasDescription)
            {
                DrawDescription();
                GUIHelper.Space();
            }

            if (!_hideMonoScript && !InspectorSearch.IsActive)
            {
                DrawScriptField();
                GUIHelper.Space();
            }

            if (_showFilterField && !InspectorSearch.IsActive)
            {
                _filterString = GUIHelper.SearchBar(_filterString);
                GUIHelper.Space();
            }

            DrawMember();

            if (GUI.changed)
            {
                UnityEditor.EditorUtility.SetDirty(Target);
            }
        }

        private void DrawMember()
        {
            if (!InspectorSearch.IsActiveByContextQuery() && Filter.IsNotNullOrWhitespace())
            {
                DrawFilteredMember(Filter);
                return;
            }
            DrawAllMember();
        }

        private void DrawFilteredMember(string filter)
        {
            serializedObject.Update();

            List<InspectorMember> pooledList = ListPool<InspectorMember>.Get();

            foreach (InspectorMember member in _headerLessFields)
            {
                if (member.Label.text.ContainsIgnoreCaseAndSpace(filter))
                {
                    pooledList.Add(member);
                }
            }

            Dictionary<FoldoutData, List<InspectorMember>> pooledDictionary = DictionaryPool<FoldoutData, List<InspectorMember>>.Get();

            foreach ((FoldoutData header, List<InspectorMember> list) in _headerFields)
            {
                if (header.Title.ContainsIgnoreCaseAndSpace(filter))
                {
                    if (!pooledDictionary.TryAdd(header, list))
                    {
                        pooledDictionary[header].AddRange(list);
                    }

                    continue;
                }

                foreach (InspectorMember member in list)
                {
                    if (member.Label.text.ContainsIgnoreCaseAndSpace(filter))
                    {
                        if (pooledDictionary.ContainsKey(header))
                        {
                            pooledDictionary[header].Add(member);
                            continue;
                        }

                        if (!pooledDictionary.TryAdd(header, new List<InspectorMember>
                        {
                            member
                        }))
                        {
                            pooledDictionary[header].Add(member);
                        }
                    }
                }
            }

            if (pooledList.Any() && InspectorSearch.IsActive)
            {
                GUIHelper.Space();
            }
            foreach (InspectorMember member in pooledList)
            {
                member.ProcessGUI();
            }
            if (pooledList.Any())
            {
                GUIHelper.Space();
            }

            foreach ((FoldoutData header, List<InspectorMember> list) in pooledDictionary)
            {
                Foldout.ForceHeader(header);
                if (!(list.First()?.HasHeaderAttribute ?? false))
                {
                    GUIHelper.Space();
                }
                foreach (InspectorMember member in list)
                {
                    member.ProcessGUI();
                }
                GUIHelper.Space();
            }

            DictionaryPool<FoldoutData, List<InspectorMember>>.Release(pooledDictionary);
            ListPool<InspectorMember>.Release(pooledList);

            serializedObject.ApplyModifiedProperties();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawAllMember()
        {
            serializedObject.Update();

            if (_headerLessFields.Any() && InspectorSearch.IsActive)
            {
                GUIHelper.Space();
            }

            for (var i = 0; i < _headerLessFields.Count; i++)
            {
                _headerLessFields[i].ProcessGUI();
            }

            if (_headerLessFields.Any())
            {
                GUIHelper.Space();
            }

            foreach ((FoldoutData header, List<InspectorMember> list) in _headerFields)
            {
                if (Foldout[header])
                {
                    if (!(list.First()?.HasHeaderAttribute ?? false))
                    {
                        GUIHelper.Space();
                    }
                    foreach (InspectorMember member in list)
                    {
                        member.ProcessGUI();
                    }

                    GUIHelper.Space();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawScriptField()
        {
            try
            {
                var enabled = GUI.enabled;
                GUI.enabled = false;
                if (_script != null)
                {
                    UnityEditor.EditorGUILayout.PropertyField(_script);
                }
                GUI.enabled = enabled;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void DrawDescription()
        {
            UnityEditor.EditorGUILayout.LabelField(_descriptionTitle, _description);
        }

        #endregion


        #region Save & Load State

        protected override void SaveStateData(string editorPrefsKey)
        {
            base.SaveStateData(editorPrefsKey);
            UnityEditor.EditorPrefs.SetString($"{nameof(_filterString)}{editorPrefsKey}", _filterString);
        }

        protected override void LoadStateData(string editorPrefsKey)
        {
            base.LoadStateData(editorPrefsKey);
            _filterString = UnityEditor.EditorPrefs.GetString($"{nameof(_filterString)}{editorPrefsKey}");
        }

        #endregion
    }
}
