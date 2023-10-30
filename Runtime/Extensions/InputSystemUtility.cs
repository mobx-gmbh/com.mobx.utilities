using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace MobX.Utilities.Extensions
{
    public static class InputSystemUtility
    {
        public static event Action AnyKey;

        static InputSystemUtility()
        {
            InputSystem.onEvent +=
                (eventPtr, device) =>
                {
                    if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
                    {
                        return;
                    }
                    var controls = device.allControls;
                    var buttonPressPoint = InputSystem.settings.defaultButtonPressPoint;
                    for (var i = 0; i < controls.Count; ++i)
                    {
                        var control = controls[i] as ButtonControl;
                        if (control == null || control.synthetic || control.noisy)
                        {
                            continue;
                        }
                        if (control.ReadValueFromEvent(eventPtr, out var value) && value >= buttonPressPoint)
                        {
                            AnyKey?.Invoke();
                            break;
                        }
                    }
                };
        }
    }
}