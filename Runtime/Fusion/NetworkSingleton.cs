using Fusion;

namespace MobX.Utilities.Fusion
{
    public class NetworkSingleton<T> : AdvancedNetworkBehaviour where T : NetworkSingleton<T>
    {
        public static T Singleton { get; private set; }

        public override void Spawned()
        {
            this.DontDestroyOnLoad();
            Singleton = (T) this;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            Singleton = null;
        }
    }
}
