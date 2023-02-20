using System;
using System.Runtime.CompilerServices;
using UnityEditor;

namespace MobX.Utilities.Editor
{
    public sealed class GUIToggle : IDisposable
    {
        public event Action<bool> Changed;
        public bool Value { get; private set; }

        private readonly Action _draw;

        public GUIToggle(string label, bool startValue, bool toggleLeft = false)
        {
            Value = startValue;
            _draw = toggleLeft
                ? () =>
                {
                    var value = EditorGUILayout.ToggleLeft(label, Value);
                    UpdateValue(value);
                }
                : () =>
                {
                    var value = EditorGUILayout.Toggle(label, Value);
                    UpdateValue(value);
                };
        }

        public void Draw()
        {
            _draw();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateValue(bool value)
        {
            if (Value == value)
            {
                return;
            }

            Value = value;
            Changed?.Invoke(value);
        }

        public static implicit operator bool(GUIToggle toggle)
        {
            return toggle.Value;
        }

        public void Dispose()
        {
            Changed = null;
        }

        public void SaveValue(string key)
        {
            EditorPrefs.SetBool(key, Value);
        }

        public void LoadValue(string key)
        {
            UpdateValue(EditorPrefs.GetBool(key, Value));
        }
    }
}