using Fusion;
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
