﻿using System;
using Object = UnityEngine.Object;

namespace MobX.Utilities.Callbacks
{
#if UNITY_EDITOR
    public sealed partial class Gameloop : UnityEditor.AssetModificationProcessor
    {
        #region Asset Handling

        public delegate void WillDeleteAssetCallback(string assetPath, Object asset);
        public static event WillDeleteAssetCallback BeforeDeleteAsset;

        private static UnityEditor.AssetDeleteResult OnWillDeleteAsset(string assetPath,
            UnityEditor.RemoveAssetOptions options)
        {
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            try
            {
                BeforeDeleteAsset?.Invoke(assetPath, asset);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }

            UnregisterInternal(asset);

            return UnityEditor.AssetDeleteResult.DidNotDelete;
        }

        #endregion
    }
#endif
}