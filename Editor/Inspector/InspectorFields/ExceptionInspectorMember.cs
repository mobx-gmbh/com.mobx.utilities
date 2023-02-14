using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector
{
    public class ExceptionInspectorMember : InspectorMember
    {
        private readonly Exception _exception;
        private bool _foldout = false;

        public ExceptionInspectorMember(Exception exception, MemberInfo memberInfo, object target) : base(memberInfo, target)
        {
            _exception = exception;
        }

        protected override void DrawGUI()
        {
            EditorGUILayout.HelpBox(_exception.Message, MessageType.Error);
            _foldout = EditorGUILayout.Foldout(_foldout, "Stacktrace", true);
            if (_foldout)
            {
                GUIHelper.BeginBox();
                GUILayout.TextArea(_exception.StackTrace, new GUIStyle("CN Message"));
                GUIHelper.EndBox();
            }
        }
    }
}