﻿using MobX.Utilities.Editor.Helper;
using MobX.Utilities.Inspector;
using System;
using System.Reflection;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector.InspectorFields
{
    public class MethodButtonInspectorMember : InspectorMember
    {
        private readonly ButtonType _buttonType;
        private readonly Action _drawGUI;

        public MethodButtonInspectorMember(MethodInfo methodInfo, ButtonAttribute attribute, object target) : base(methodInfo, target)
        {
            var label = attribute.Label ?? methodInfo.Name.Humanize();
            var tooltip = methodInfo.TryGetCustomAttribute<TooltipAttribute>(out TooltipAttribute tooltipAttribute) ? tooltipAttribute.tooltip : null;
            Label = new GUIContent(label, tooltip);

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            var showResult = attribute.ShowResult;
            var showArguments = attribute.ShowArguments;
            var showControls = showResult || showArguments;
            var wasCalled = false;

            var arguments = new object[parameterInfos.Length];

            for (var index = 0; index < parameterInfos.Length; index++)
            {
                ParameterInfo parameterInfo = parameterInfos[index];
                Type parameterType = parameterInfo.ParameterType;
                Type underlyingParameterType = parameterType.GetElementType() ?? parameterType;

                arguments[index] = parameterInfo.HasDefaultValue ? parameterInfo.DefaultValue : null;
                arguments[index] ??= underlyingParameterType.IsValueType
                    ? Activator.CreateInstance(underlyingParameterType, true)
                    : Convert.ChangeType(arguments[index], underlyingParameterType);
            }

            if (parameterInfos.Length == 0 || !showControls)
            {
                Delegate methodCall = methodInfo.IsStatic
                    ? methodInfo.CreateMatchingDelegate()
                    : methodInfo.CreateMatchingDelegate(target);

                _drawGUI = attribute.ButtonType switch
                {
                    ButtonType.Default => () =>
                    {
                        if (!GUILayout.Button(Label))
                        {
                            return;
                        }
                        methodCall?.DynamicInvoke(arguments);
                        wasCalled = true;
                    },
                    ButtonType.Center => () =>
                    {
                        if (!GUIHelper.ButtonCenter(Label))
                        {
                            return;
                        }
                        methodCall?.DynamicInvoke(arguments);
                        wasCalled = true;
                    },
                    ButtonType.Right => () =>
                    {
                        if (!GUIHelper.ButtonRight(Label))
                        {
                            return;
                        }
                        methodCall?.DynamicInvoke(arguments);
                        wasCalled = true;
                    },
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            else
            {
                _drawGUI += GUIHelper.BeginBox;

                Delegate methodCall = methodInfo.IsStatic ?
                    methodInfo.CreateMatchingDelegate() :
                    methodInfo.CreateMatchingDelegate(target);

                var returnValue = default(object);
                var hasReturnValue = methodInfo.HasReturnValue();

                if (showArguments)
                {
                    for (var index = 0; index < parameterInfos.Length; index++)
                    {
                        ParameterInfo parameterInfo = parameterInfos[index];

                        Func<object, object> elementEditor = GUIHelper.CreateEditor(new GUIContent(parameterInfo.Name), parameterInfo.ParameterType);
                        var capturedIndex = index;

                        _drawGUI += () =>
                        {
                            arguments[capturedIndex] = elementEditor(arguments[capturedIndex]);
                        };
                    }
                }

                _drawGUI += () =>
                {
                    if (GUILayout.Button(Label))
                    {
                        returnValue = methodCall?.DynamicInvoke(arguments);
                        wasCalled = true;
                    }
                };

                if (hasReturnValue && showResult)
                {
                    _drawGUI += () =>
                    {
                        GUIHelper.BeginEnabledOverride(false);
                        UnityEditor.EditorGUILayout.LabelField("Result", wasCalled ? returnValue.ToNullString() : string.Empty);
                        GUIHelper.EndEnabledOverride();
                    };
                }

                _drawGUI += GUIHelper.EndBox;
            }
        }

        protected override void DrawGUI()
        {
            _drawGUI();
        }
    }
}
