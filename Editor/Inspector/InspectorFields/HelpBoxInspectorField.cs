using System.Reflection;
using UnityEditor;

namespace MobX.Utilities.Editor.Inspector
{
    public class HelpBoxInspectorMember : InspectorMember
    {
        private readonly string _message;
        private readonly MessageType _messageType;

        public HelpBoxInspectorMember(string message, MessageType messageType, MemberInfo memberInfo, object target) : base(memberInfo, target)
        {
            _message = message;
            _messageType = messageType;
        }

        protected override void DrawGUI()
        {
            EditorGUILayout.HelpBox(_message, _messageType);
        }
    }
}