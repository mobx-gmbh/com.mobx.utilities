using System;
using UnityEngine;

namespace MobX.Utilities.Callbacks
{
    [DisallowMultipleComponent]
    internal sealed class RuntimeMonoBehaviourEvents : MonoBehaviour
    {
        private Action _onFixedUpdate;
        private Action _onLateUpdate;
        private Action _onQuit;
        private Action _onUpdate;
        private Action<bool> _onFocus;
        private Action<bool> _onPause;

        private void Update()
        {
            _onUpdate();
        }

        private void FixedUpdate()
        {
            _onFixedUpdate();
        }

        private void LateUpdate()
        {
            _onLateUpdate();
        }

        private void OnApplicationQuit()
        {
            _onQuit();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            _onFocus(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            _onPause(pauseStatus);
        }

        internal static MonoBehaviour Create(Action onUpdate, Action onLateUpdate, Action onFixedUpdate, Action onQuit,
            Action<bool> onFocus, Action<bool> onPause)
        {
            var gameObject = new GameObject(nameof(RuntimeMonoBehaviourEvents));
            var instance = gameObject.AddComponent<RuntimeMonoBehaviourEvents>();
            gameObject.DontDestroyOnLoad();
            gameObject.hideFlags |= HideFlags.HideInHierarchy;

            instance._onUpdate = onUpdate;
            instance._onLateUpdate = onLateUpdate;
            instance._onFixedUpdate = onFixedUpdate;
            instance._onQuit = onQuit;
            instance._onFocus = onFocus;
            instance._onPause = onPause;
            return instance;
        }
    }
}