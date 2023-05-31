﻿using MobX.Utilities.Editor.Helper;
using MobX.Utilities.Editor.Inspector.PropertyDrawer;
using MobX.Utilities.Inspector;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.InspectorFields
{
    public abstract class InspectorMember
    {
        /*
         * Fields & Properties
         */

        public GUIContent Label { get; protected set; }
        public MemberInfo Member { get; }
        public bool HasHeaderAttribute { get; }
        protected object Target { get; }

        private readonly bool _hasConditional;
        private readonly Action _preDraw;
        private readonly Action _postDraw;

        protected static string[] Prefixes { get; } =
        {
            "_", "m_"
        };

        /*
         * Ctor
         */

        protected InspectorMember(MemberInfo member, object target)
        {
            Target = target;
            Member = member;
            try
            {
                HasHeaderAttribute = Member.HasAttribute<HeaderAttribute>();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            var isSerialized = member.HasAttribute<SerializeField>() || member is FieldInfo {IsPublic: true};

            var attributes = member.GetCustomAttributes<PropertyAttribute>().ToArray();

            for (var i = 0; i < attributes.Length; i++)
            {
                var propertyAttribute = attributes[i];

                switch (propertyAttribute)
                {
                    case SpaceBeforeAttribute spaceBeforeAttribute:
                        _preDraw += () => GUIHelper.Space(spaceBeforeAttribute.height);
                        break;

                    case SpaceAfterAttribute spaceAfterAttribute:
                        _postDraw += () => GUIHelper.Space(spaceAfterAttribute.Height);
                        break;

                    case SpaceAttribute spaceAttribute when !isSerialized:
                        _preDraw += () => GUIHelper.Space(spaceAttribute.height);
                        break;

                    case HeaderAttribute headerAttribute when !isSerialized:
                        _preDraw += () =>
                        {
                            GUIHelper.Space();
                            GUIHelper.BoldLabel(headerAttribute.header);
                        };
                        break;

                    case DrawLineAttribute drawLineAttribute:
                        if (drawLineAttribute.SpaceBefore > -1)
                        {
                            var space = drawLineAttribute.SpaceBefore;
                            _preDraw += () => GUIHelper.Space(space);
                        }

                        _preDraw += GUIHelper.DrawLine;

                        if (drawLineAttribute.SpaceAfter > -1)
                        {
                            var space = drawLineAttribute.SpaceBefore;
                            _preDraw += () => GUIHelper.Space(space);
                        }

                        break;

                    case AnnotationAttribute annotationAttribute:
                        _preDraw += () => UnityEditor.EditorGUILayout.HelpBox(annotationAttribute.Annotation,
                            (UnityEditor.MessageType) annotationAttribute.MessageType);
                        break;

                    case ConditionalShowAttribute conditionalAttribute:
                        _displayMode = () =>
                        {
                            var result = ConditionalShowValidator.ValidateComparison(conditionalAttribute.Condition,
                                Target.CreateGetDelegateForMember(conditionalAttribute.Member), false);

                            return result ? DisplayMode.Show :
                                conditionalAttribute.ReadOnly ? DisplayMode.Readonly : DisplayMode.Hide;
                        };
                        break;

                    case ConditionalHideAttribute conditionalAttribute:
                        _displayMode = () =>
                        {
                            var result = ConditionalShowValidator.ValidateComparison(conditionalAttribute.Condition,
                                Target.CreateGetDelegateForMember(conditionalAttribute.Member), true);

                            return result ? DisplayMode.Show :
                                conditionalAttribute.ReadOnly ? DisplayMode.Readonly : DisplayMode.Hide;
                        };
                        break;
                }
            }

            _displayMode ??= () => DisplayMode.Show;
        }

        private enum DisplayMode
        {
            Show,
            Hide,
            Readonly
        }

        private readonly Func<DisplayMode> _displayMode;

        public void ProcessGUI()
        {
            switch (_displayMode())
            {
                case DisplayMode.Show:
                    _preDraw?.Invoke();
                    DrawGUI();
                    _postDraw?.Invoke();
                    return;
                case DisplayMode.Hide:
                    return;
                case DisplayMode.Readonly:
                    var enabled = GUI.enabled;
                    GUI.enabled = false;
                    _preDraw?.Invoke();
                    GUI.enabled = false;
                    DrawGUI();
                    GUI.enabled = enabled;
                    _postDraw?.Invoke();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void DrawGUI();
    }
}