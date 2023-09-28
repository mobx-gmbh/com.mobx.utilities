namespace MobX.Utilities.Cooldown
{
    public interface ICooldownCallback
    {
        /// <summary>
        ///     Called on the callback receiver when the cooldown is started.
        /// </summary>
        public void OnBeginCooldown();

        /// <summary>
        ///     Called on the callback receiver when the cooldown has completed.
        /// </summary>
        public void OnEndCooldown();

        /// <summary>
        ///     Called on the callback receiver every update while the cooldown is running.
        /// </summary>
        /// <param name="remainingTime">seconds remaining</param>
        public void OnCooldownUpdate(float remainingTime)
        {
        }
    }

    /// <summary>
    ///     Optional cooldown interface to receive a callback when the cooldown is cancelled.
    /// </summary>
    public interface IOnCooldownCancelled
    {
        /// <summary>
        ///     Called on the receiver object when the cooldown is cancelled.
        /// </summary>
        public void OnCooldownCancelled();
    }
}
