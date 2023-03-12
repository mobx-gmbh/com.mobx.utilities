using MobX.Utilities.Inspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Editor
{
    public static partial class GUIHelper
    {
        /*
         * Size
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetSizeOfLabel(GUIContent label)
        {
            return UnityEditor.EditorStyles.label.CalcSize(label);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetIndent()
        {
            return UnityEditor.EditorGUI.indentLevel;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect GetControlRect()
        {
            return UnityEditor.EditorGUILayout.GetControlRect();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect GetLastRect()
        {
            return GUILayoutUtility.GetLastRect();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect GetLastRectIndented()
        {
            return UnityEditor.EditorGUI.IndentedRect(GUILayoutUtility.GetLastRect());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetViewWidth()
        {
            return UnityEditor.EditorGUIUtility.currentViewWidth;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetLabelWidth()
        {
            return UnityEditor.EditorGUIUtility.labelWidth;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetLabelWidthIndented()
        {
            return UnityEditor.EditorGUIUtility.labelWidth - UnityEditor.EditorGUI.indentLevel * 15f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetFieldWidth()
        {
            return UnityEditor.EditorGUIUtility.fieldWidth;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetLineHeight()
        {
            return UnityEditor.EditorGUIUtility.singleLineHeight;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetIndentLength(Rect sourceRect)
        {
            Rect indentRect = UnityEditor.EditorGUI.IndentedRect(sourceRect);
            var indentLength = indentRect.x - sourceRect.x;

            return indentLength;
        }

        /*
         * Object Specific
         */

        public static GUIContent GetContentForObject(Object target, Type type = null)
        {
            type ??= target.GetType();
            var name = type.GetCustomAttribute<AddComponentMenu>()?.componentMenu?.Split('/').Last() ??
                type.Name.Humanize();
            return new GUIContent(" " + name, UnityEditor.AssetPreview.GetMiniThumbnail(target));
        }

        /*
         * Box
         */

        public static void BeginBox()
        {
            UnityEditor.EditorGUILayout.BeginVertical(HelpBoxStyle);
        }

        public static void BeginBox(BoxStyle style)
        {
            switch (style)
            {
                case Utilities.Inspector.BoxStyle.GrayBox:
                    UnityEditor.EditorGUILayout.BeginVertical(BoxStyle);
                    break;
                case Utilities.Inspector.BoxStyle.HelpBox:
                    UnityEditor.EditorGUILayout.BeginVertical(HelpBoxStyle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }

        public static void EndBox()
        {
            UnityEditor.EditorGUILayout.EndVertical();
        }

        /*
         * Drag & Drop
         */

        public static Object[] DropZone(string title, params GUILayoutOption[] options)
        {
            BeginBox();

            GUILayout.Box(new GUIContent(title), options);

            EventType eventType = Event.current.type;
            var isAccepted = false;

            if (eventType is EventType.DragUpdated or EventType.DragPerform)
            {
                UnityEditor.DragAndDrop.visualMode = UnityEditor.DragAndDropVisualMode.Copy;

                if (eventType == EventType.DragPerform)
                {
                    UnityEditor.DragAndDrop.AcceptDrag();
                    isAccepted = true;
                }

                Event.current.Use();
            }

            EndBox();
            return isAccepted ? UnityEditor.DragAndDrop.objectReferences : null;
        }

        /*
         * Async
         */

        public async static Task CompleteGUI()
        {
            await Task.Delay(25);
        }

        /*
         * Indent
         */

        private static readonly Stack<int> indentOverrides = new Stack<int>(4);

        public static void BeginIndentOverride(int indent)
        {
            var current = UnityEditor.EditorGUI.indentLevel;
            indentOverrides.Push(current);
            UnityEditor.EditorGUI.indentLevel = indent;
        }

        public static void EndIndentOverride()
        {
            if (indentOverrides.TryPop(out var cached))
            {
                UnityEditor.EditorGUI.indentLevel = cached;
            }
            else
            {
                UnityEngine.Debug.LogError(
                    $"Mismatched calls of {nameof(BeginIndentOverride)} & {nameof(EndIndentOverride)}!");
            }
        }

        public static void IncreaseIndent()
        {
            UnityEditor.EditorGUI.indentLevel++;
        }

        public static void DecreaseIndent()
        {
            UnityEditor.EditorGUI.indentLevel--;
        }

        private static readonly Stack<bool> enabledOverrides = new Stack<bool>(4);

        public static void BeginEnabledOverride(bool enabledState)
        {
            var current = GUI.enabled;
            enabledOverrides.Push(current);
            GUI.enabled = enabledState;
        }

        public static void EndEnabledOverride()
        {
            if (enabledOverrides.TryPop(out var cached))
            {
                GUI.enabled = cached;
            }
            else
            {
                UnityEngine.Debug.LogError(
                    $"Mismatched calls of {nameof(BeginEnabledOverride)} & {nameof(EndEnabledOverride)}!");
            }
        }
    }
}
