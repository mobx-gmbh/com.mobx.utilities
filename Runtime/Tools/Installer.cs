using MobX.Utilities.Callbacks;
using MobX.Utilities.Types;
using System;
using UnityEngine;

namespace MobX.Utilities.Tools
{
    public class Installer : RuntimeAsset
    {
        [SerializeField] private bool autoInstall;
        [SerializeField] private Optional<GameObject>[] systems;

        [NonSerialized] private bool _installed;

        [CallbackOnInitialization]
        private void Install()
        {
            if (_installed || autoInstall is false)
            {
                return;
            }
            _installed = true;
            foreach (var system in systems)
            {
                if (system.TryGetValue(out var prefab))
                {
                    var instance = Instantiate(prefab);
                    instance.DontDestroyOnLoad();
                }
            }
        }

        [CallbackOnApplicationQuit]
        private void OnQuit()
        {
            _installed = false;
        }
    }
}