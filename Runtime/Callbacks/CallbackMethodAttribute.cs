using JetBrains.Annotations;
using System;

namespace MobX.Utilities.Callbacks
{
    /// <summary>
    ///     Mark a method in a registered scriptable object or behaviour that is then called during a specific callback.
    /// </summary>
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class CallbackMethodAttribute : Attribute
    {
        public Segment Segment { get; }
        public string Custom { get; }

        /// <summary>
        ///     Mark a method in a registered scriptable object or behaviour that is then called during the <see cref="Segment" />
        /// </summary>
        /// <param name="segment">The <see cref="Segment" /> during which the method is called.</param>
        public CallbackMethodAttribute(Segment segment)
        {
            Custom = null;
            Segment = segment;
        }

        /// <summary>
        ///     Mark a method in a registered scriptable object or behaviour that is then called during the passed custom callback.
        /// </summary>
        /// <param name="callback">The callback during which the method is called.</param>
        public CallbackMethodAttribute(string callback)
        {
            Custom = callback;
            Segment = Segment.Custom;
        }
    }

    /// <summary>
    ///     Method is called when the games subsystems are initialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CallbackOnInitialization : CallbackMethodAttribute
    {
        /// <summary>
        ///     Method is called when the games subsystems are initialized.
        /// </summary>
        public CallbackOnInitialization() : base(Segment.InitializationCompleted)
        {
        }
    }

    /// <summary>
    ///     Method is called when the application is shutdown.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CallbackOnApplicationQuit : CallbackMethodAttribute
    {
        /// <summary>
        ///     Method is called when the application is shutdown.
        /// </summary>
        public CallbackOnApplicationQuit() : base(Segment.ApplicationQuit)
        {
        }
    }

    /// <summary>
    ///     Method is called every frame.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CallbackOnUpdate : CallbackMethodAttribute
    {
        /// <summary>
        ///     Method is called every frame.
        /// </summary>
        public CallbackOnUpdate() : base(Segment.Update)
        {
        }
    }

    /// <summary>
    ///     Method is called every frame during late update.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CallbackOnLateUpdate : CallbackMethodAttribute
    {
        /// <summary>
        ///     Method is called every frame during late update.
        /// </summary>
        public CallbackOnLateUpdate() : base(Segment.LateUpdate)
        {
        }
    }

    /// <summary>
    ///     Method is called every fixed physics update.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CallbackOnFixedUpdate : CallbackMethodAttribute
    {
        /// <summary>
        ///     Method is called every fixed physics update.
        /// </summary>
        public CallbackOnFixedUpdate() : base(Segment.FixedUpdate)
        {
        }
    }
}