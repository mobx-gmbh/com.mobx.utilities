﻿namespace MobX.Utilities.Callbacks
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

    /// <summary>
    ///     Called after a manual async initialization process has been completed. Must be manually invoked by custom code.
    /// </summary>
    public interface IOnInitializationCompleted : ICallbackInterface
    {
        /// <summary>
        ///     Called after a manual async initialization process has been completed. Must be manually invoked by custom code.
        /// </summary>
        public void OnInitializationCompleted();
    }

    /// <summary>
    ///     Called after the first scene was loaded.
    /// </summary>
    public interface IOnAfterFirstSceneLoad : ICallbackInterface
    {
        /// <summary>
        ///     Called after the first scene was loaded.
        /// </summary>
        public void OnAfterFirstSceneLoad();
    }

    /// <summary>
    ///     Called before the first scene is loaded.
    /// </summary>
    public interface IOnBeforeFirstSceneLoad : ICallbackInterface
    {
        /// <summary>
        ///     Called before the first scene is loaded.
        /// </summary>
        public void OnBeforeFirstSceneLoad();
    }

    /// <summary>
    ///     Receive a callback when the game ends.
    /// </summary>
    public interface IOnQuit : ICallbackInterface
    {
        /// <summary>
        ///     Called at runtime when the game ends.
        /// </summary>
        public void OnQuit();
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