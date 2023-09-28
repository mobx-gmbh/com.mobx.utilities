//using Drawing;
using Fusion;

namespace MobX.Utilities.Fusion
{
    /// <summary>
    ///     Implements further helpful network behavior logic that can be used by network behavior classes
    /// </summary>
    public abstract class AdvancedNetworkBehaviour : NetworkBehaviour,
        IStateAuthorityChanged,
        IPlayerLeft
// #if UNITY_EDITOR
//         ,
//         IDrawGizmos
// #endif

    {
        public bool LocalHasStateAuthority { get; private set; }
        public bool LocalIsSpawned { get; private set; }
        protected bool IsInitializedAndAuthority { get; private set; }

        private PlayerRef _lastAuthority;

        protected virtual void Awake()
        {
            _lastAuthority = PlayerRef.None;
        }

        public override void Spawned()
        {
            CheckStateAuthority();
            LocalIsSpawned = true;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            LocalIsSpawned = false;
            IsInitializedAndAuthority = false;
            LocalHasStateAuthority = false;
        }

        protected T NetworkUnwrap<T>(NetworkRunner runner, NetworkBehaviourId networkId) where T : NetworkBehaviour
        {
            return NetworkUnwrap(runner, networkId) as T;
        }

        /// <summary>
        ///     updates the local hast state authority bool to detect changes
        ///     calls receive / lost state authority function when change was detected
        /// </summary>
        private void CheckStateAuthorityChanged()
        {
            if (LocalHasStateAuthority == HasStateAuthority)
            {
                return;
            }

            CheckStateAuthority();
        }

        private void CheckStateAuthority()
        {
            if (HasStateAuthority)
            {
                LocalHasStateAuthority = true;
                IsInitializedAndAuthority = true;
                ReceivedStateAuthority();
            }
            else
            {
                LocalHasStateAuthority = false;
                IsInitializedAndAuthority = false;
                LostStateAuthority();
            }
        }

        /// <summary>
        ///     Called by Spawned or FixedUpdateNetwork function when Object.HasStateAuthority
        ///     was received, requires base.FixedUpdateNetwork to be called
        /// </summary>
        protected virtual void ReceivedStateAuthority()
        {
        }

        /// <summary>
        ///     Called by Spawned or FixedUpdateNetwork function when Object.HasStateAuthority
        ///     was lost. Requires base.FixedUpdateNetwork to be called
        /// </summary>
        protected virtual void LostStateAuthority()
        {
        }

        /// <summary>
        ///     Called once the Authority of this game Object left the game
        /// </summary>
        protected virtual void AuthorityLeft()
        {
        }

        public virtual void StateAuthorityChanged()
        {
            CheckStateAuthorityChanged();
            _lastAuthority = Object.StateAuthority;
        }

        public virtual void PlayerLeft(PlayerRef player)
        {
            if (_lastAuthority != player)
            {
                return;
            }

            IsInitializedAndAuthority = false;
            AuthorityLeft();
            Object.RequestStateAuthority();
        }

        public virtual void DrawGizmos()
        {
        }

        protected AdvancedNetworkBehaviour()
        {
#if UNITY_EDITOR
            //DrawingManager.Register(this);
#endif
        }
    }
}
