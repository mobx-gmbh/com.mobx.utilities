using UnityEngine;

namespace MobX.Utilities.Callbacks
{
    #region Editor Callbacks

    public interface IOnEnterPlayMode : ICallbackInterface
    {
        /// <summary>
        ///     Called in the editor when play mode is entered.
        /// </summary>
        void OnEnterPlayMode();
    }

    public interface IOnExitPlayMode : ICallbackInterface
    {
        /// <summary>
        ///     Called in the editor when play mode is exited.
        /// </summary>
        void OnExitPlayMode();
    }

    public interface IOnEnterEditMode : ICallbackInterface
    {
        /// <summary>
        ///     Called in the editor when edit mode is entered.
        /// </summary>
        void OnEnterEditMode();
    }

    public interface IOnExitEditMode : ICallbackInterface
    {
        /// <summary>
        ///     Called in the editor when edit mode is exited.
        /// </summary>
        void OnExitEditMode();
    }

    #endregion


    #region Runtime Callbacks

    public interface IOnAfterLoad : ICallbackInterface
    {
        public void OnAfterLoad();
    }

    /// <summary>
    ///     Receive a callback when the game starts.
    /// </summary>
    public interface IOnBeginPlay : ICallbackInterface
    {
        /// <summary>
        ///     Called when the game loads during <see cref="RuntimeInitializeLoadType.BeforeSceneLoad" />
        ///     Registering a receiver during an active session will trigger an immediate callback.
        /// </summary>
        public void OnBeginPlay();
    }

    /// <summary>
    ///     Receive a callback when the game ends.
    /// </summary>
    public interface IOnEndPlay : ICallbackInterface
    {
        /// <summary>
        ///     Called at runtime when the game ends.
        /// </summary>
        public void OnEndPlay();
    }

    #endregion


    #region Update Callbacks

    public interface IOnUpdate : ICallbackInterface
    {
        public void OnUpdate(float deltaTime);
    }

    public interface IOnLateUpdate : ICallbackInterface
    {
        public void OnLateUpdate(float deltaTime);
    }

    public interface IOnFixedUpdate : ICallbackInterface
    {
        public void OnFixedUpdate(float fixedDeltaTime);
    }

    #endregion
}