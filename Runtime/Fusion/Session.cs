using Fusion;
using System;
using UnityEngine;

namespace MobX.Utilities.Fusion
{
    public class Session : NetworkSingleton<Session>
    {
        #region Public API

        /// <summary>
        ///     Check if the session is already initialized.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        ///     Shared time is updated and synchronized every tick.
        /// </summary>
        public static float SharedTime => IsInitialized ? Singleton.NetworkedSharedTime : 0;

        /// <summary>
        ///     Smooth shared time is updated every frame and synchronized every tick.
        /// </summary>
        public static float SmoothSharedTime => IsInitialized ? Singleton.NetworkedSmoothSharedTime : 0;

        /// <summary>
        ///     Called when the session is initialized. Will trigger retroactively.
        ///     You don't have to unsubscribe from this event.
        /// </summary>
        public static event Action Initialized
        {
            add
            {
                if (IsInitialized)
                {
                    value();
                }
                else
                {
                    onInitialized += value;
                }
            }
            remove => onInitialized -= value;
        }

        private static Action onInitialized;

        #endregion


        #region Networked

        [Networked]
        private float NetworkedSharedTime { get; set; }

        [Networked]
        private float NetworkedSmoothSharedTime { get; set; }

        #endregion


        #region Setup & Shutdown

        public override void Spawned()
        {
            base.Spawned();
            IsInitialized = true;
            onInitialized?.Invoke();
            onInitialized = null;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            IsInitialized = false;
        }

        #endregion


        #region Gameloop

        public override void FixedUpdateNetwork()
        {
            if (Runner.IsSharedModeMasterClient || Runner.IsSinglePlayer)
            {
                NetworkedSharedTime += Runner.DeltaTime * Time.timeScale;
                NetworkedSmoothSharedTime = SharedTime;
            }
        }

        private void Update()
        {
            NetworkedSmoothSharedTime += Time.deltaTime;
        }

        #endregion
    }
}
