﻿using MobX.Utilities.Inspector;
using System;
using System.Reflection;
using UnityEngine;

namespace MobX.Utilities.Editor.Inspector
{
    public class NonSerializedReadonlyFieldInspector : InspectorMember
    {
        private readonly FieldInfo _fieldInfo;
        private readonly object _target;
        private readonly Action<object> _drawer;


        public NonSerializedReadonlyFieldInspector(FieldInfo fieldInfo, object target) : base(fieldInfo, target)
        {
            _fieldInfo = fieldInfo;
            _target = target;
            var label = fieldInfo.TryGetCustomAttribute<LabelAttribute>(out var labelAttribute)
                ? labelAttribute.Label
                : fieldInfo.Name.Humanize(Prefixes);

            var tooltip = fieldInfo.TryGetCustomAttribute<TooltipAttribute>(out var tooltipAttribute) ? tooltipAttribute.tooltip : null;

            Label = new GUIContent(label, tooltip);
            _drawer = GUIHelper.CreateDrawer(Label, _fieldInfo.FieldType);
        }

        protected override void DrawGUI()
        {
            GUIHelper.BeginEnabledOverride(false);
            _drawer(_fieldInfo.GetValue(_target));
            GUIHelper.EndEnabledOverride();
        }
    }
}