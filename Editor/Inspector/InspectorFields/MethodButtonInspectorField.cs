using MobX.Utilities.Editor.Helper;
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
        private readonly bool _runtimeOnly;
        private readonly bool _editorOnly;

        public MethodButtonInspectorMember(MethodInfo methodInfo, ButtonAttribute attribute, object target) : base(
            methodInfo, target)
        {
            var label = attribute.Label ?? methodInfo.Name.Humanize();
            var tooltip = methodInfo.TryGetCustomAttribute(out TooltipAttribute tooltipAttribute)
                ? tooltipAttribute.tooltip
                : null;
            Label = new GUIContent(label, tooltip);

            _runtimeOnly = attribute.RuntimeOnly;
            _editorOnly = attribute.EditorOnly;
            var confirmationRequired = attribute.ConfirmationText != null;
            var confirmationText = attribute.ConfirmationText;

            var parameterInfos = methodInfo.GetParameters();
            var showResult = attribute.ShowResult;
            var showArguments = attribute.ShowArguments;
            var showControls = showResult || showArguments;
            var wasCalled = false;

            var arguments = new object[parameterInfos.Length];

            for (var index = 0; index < parameterInfos.Length; index++)
            {
                var parameterInfo = parameterInfos[index];
                var parameterType = parameterInfo.ParameterType;
                var underlyingParameterType = parameterType.GetElementType() ?? parameterType;

                arguments[index] = parameterInfo.HasDefaultValue ? parameterInfo.DefaultValue : null;
                arguments[index] ??= underlyingParameterType.IsValueType
                    ? Activator.CreateInstance(underlyingParameterType, true)
                    : Convert.ChangeType(arguments[index], underlyingParameterType);
            }

            if (parameterInfos.Length == 0 || !showControls)
            {
                var methodCall = methodInfo.IsStatic
                    ? methodInfo.CreateMatchingDelegate()
                    : methodInfo.CreateMatchingDelegate(target);

                _drawGUI = attribute.ButtonType switch
                {
                    ButtonType.Default => () =>
                    {
                        if (GUILayout.Button(Label))
                        {
                            if (confirmationRequired)
                            {
                                var result = UnityEditor.EditorUtility.DisplayDialog(
                                    "Confirm",
                                    confirmationText,
                                    "Confirm",
                                    "Cancel");

                                if (result == false)
                                {
                                    return;
                                }
                            }

                            methodCall?.DynamicInvoke(arguments);
                            wasCalled = true;
                        }
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

                var methodCall = methodInfo.IsStatic
                    ? methodInfo.CreateMatchingDelegate()
                    : methodInfo.CreateMatchingDelegate(target);

                var returnValue = default(object);
                var hasReturnValue = methodInfo.HasReturnValue();

                if (showArguments)
                {
                    for (var index = 0; index < parameterInfos.Length; index++)
                    {
                        var parameterInfo = parameterInfos[index];

                        var elementEditor = GUIHelper.CreateEditor(new GUIContent(parameterInfo.Name),
                            parameterInfo.ParameterType);
                        var capturedIndex = index;

                        _drawGUI += () => { arguments[capturedIndex] = elementEditor(arguments[capturedIndex]); };
                    }
                }

                _drawGUI += () =>
                {
                    if (GUILayout.Button(Label))
                    {
                        if (confirmationRequired)
                        {
                            var result = UnityEditor.EditorUtility.DisplayDialog(
                                "Confirm",
                                confirmationText,
                                "Confirm",
                                "Cancel");

                            if (result == false)
                            {
                                return;
                            }
                        }
                        returnValue = methodCall?.DynamicInvoke(arguments);
                        wasCalled = true;
                    }
                };

                if (hasReturnValue && showResult)
                {
                    _drawGUI += () =>
                    {
                        GUIHelper.BeginEnabledOverride(false);
                        UnityEditor.EditorGUILayout.LabelField("Result",
                            wasCalled ? returnValue.ToNullString() : string.Empty);
                        GUIHelper.EndEnabledOverride();
                    };
                }

                _drawGUI += GUIHelper.EndBox;
            }
        }

        protected override void DrawGUI()
        {
            var disable = (_runtimeOnly && Application.isPlaying is false) || (_editorOnly && Application.isPlaying);
            if (disable)
            {
                GUIHelper.BeginEnabledOverride(false);
            }

            _drawGUI();

            if (disable)
            {
                GUIHelper.EndEnabledOverride();
            }
        }
    }
}