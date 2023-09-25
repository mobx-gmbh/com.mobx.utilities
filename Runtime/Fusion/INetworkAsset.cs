namespace MobX.Utilities.Fusion
{
    public interface INetworkAsset
    {
        /// <summary>
        ///     Network id used to identify and synchronise assets via network. This id is unique but may can change
        ///     between versions. It represents the array index of the network asset in the network asset registry.
        /// </summary>
        public int NetworkAssetID { get; internal set; }
    }
}