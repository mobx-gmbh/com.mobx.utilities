using Cysharp.Threading.Tasks;
using MobX.Utilities.Types;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MobX.Utilities.Task
{
    public static class UniTaskExtensions
    {
        #region Ducktyping

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask.Awaiter GetAwaiter(this TimeSpan timeSpan)
        {
            return UniTask.Delay(timeSpan).GetAwaiter();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask.Awaiter GetAwaiter(this Seconds seconds)
        {
            return UniTask.Delay(seconds).GetAwaiter();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask.Awaiter GetAwaiter(this float seconds)
        {
            return UniTask.Delay(TimeSpan.FromSeconds(seconds)).GetAwaiter();
        }

        #endregion


        #region Particle System

        public static void FadeOut(this ParticleSystem particleSystem)
        {
            particleSystem.FadeOutAsync().Forget();
        }

        public static async UniTask FadeOutAsync(this ParticleSystem particleSystem)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            await UniTask.WaitWhile(() => particleSystem && particleSystem.particleCount > 0);
            if (particleSystem == null)
            {
                return;
            }
            particleSystem.SetActive(false);
        }

        public static async UniTask<T> AsUniTask<T>(this AsyncOperationHandle<T> operationHandle)
        {
            var result = await operationHandle.Task;
            return result;
        }

        #endregion


        #region Timeout

        /// <summary>
        ///     Set a timout for the completion of the target <see cref="Task" />. A
        ///     <exception cref="TimeoutException"></exception>
        ///     is thrown if the target task does not complete within the set timeframe.
        /// </summary>
        public static async void Timeout(this System.Threading.Tasks.Task mightTimeout, int timeoutMillisecond,
            CancellationToken ct = default)
        {
            await System.Threading.Tasks.Task.WhenAny(mightTimeout, TimeoutInternalTaskAsync(timeoutMillisecond, ct));
        }

        /// <summary>
        ///     Set a timout for the completion of the target <see cref="Task" />. A
        ///     <exception cref="TimeoutException"></exception>
        ///     is thrown if the target task does not complete within the set timeframe.
        /// </summary>
        public static async void Timeout(this System.Threading.Tasks.Task mightTimeout, TimeSpan timeoutTimeSpan,
            CancellationToken ct = default)
        {
            await TimeoutAsync(mightTimeout, timeoutTimeSpan.Milliseconds, ct);
        }

        /// <summary>
        ///     Set a timout for the completion of the target <see cref="Task" />. A
        ///     <exception cref="TimeoutException"></exception>
        ///     is thrown if the target task does not complete within the set timeframe.
        /// </summary>
        private static async System.Threading.Tasks.Task TimeoutInternalTaskAsync(int millisecondsTimeout,
            CancellationToken ct = default)
        {
            await System.Threading.Tasks.Task.Delay(millisecondsTimeout, ct);
            throw new TimeoutException($"Timeout after {millisecondsTimeout}ms while awaiting a task!");
        }

        #endregion


        #region Timeout Async

        /// <summary>
        ///     Set a timout for the completion of the target <see cref="Task" />. A
        ///     <exception cref="TimeoutException"></exception>
        ///     is thrown if the target task does not complete within the set timeframe.
        /// </summary>
        public static System.Threading.Tasks.Task TimeoutAsync(System.Threading.Tasks.Task mightTimeout,
            int timeoutMillisecond, CancellationToken ct = default)
        {
            return System.Threading.Tasks.Task.WhenAny(mightTimeout, TimeoutInternalAsync(timeoutMillisecond, ct));
        }

        /// <summary>
        ///     Set a timout for the completion of the target <see cref="Task" />. A
        ///     <exception cref="TimeoutException"></exception>
        ///     is thrown if the target task does not complete within the set timeframe.
        /// </summary>
        public static System.Threading.Tasks.Task TimeoutAsync(this System.Threading.Tasks.Task mightTimeout,
            TimeSpan timeoutTimeSpan, CancellationToken ct = default)
        {
            return TimeoutAsync(mightTimeout, timeoutTimeSpan.Milliseconds, ct);
        }

        /// <summary>
        ///     Set a timout for the completion of the target <see cref="Task" />. A
        ///     <exception cref="TimeoutException"></exception>
        ///     is thrown if the target task does not complete within the set timeframe.
        /// </summary>
        public static async Task<TResult> TimeoutAsync<TResult>(this Task<TResult> mightTimeout, int timeoutMillisecond,
            CancellationToken ct = default)
        {
            return await await System.Threading.Tasks.Task.WhenAny(mightTimeout,
                TimeoutInternalAsync<TResult>(timeoutMillisecond, ct));
        }

        /// <summary>
        ///     Set a timout for the completion of the target <see cref="Task" />. A
        ///     <exception cref="TimeoutException"></exception>
        ///     is thrown if the target task does not complete within the set timeframe.
        /// </summary>
        public static async Task<TResult> TimeoutAsync<TResult>(this Task<TResult> mightTimeout,
            TimeSpan timeoutTimeSpan, CancellationToken ct = default)
        {
            return await TimeoutAsync(mightTimeout, timeoutTimeSpan.Milliseconds, ct);
        }

        private static async Task<TResult> TimeoutInternalAsync<TResult>(int millisecondsTimeout,
            CancellationToken ct = default)
        {
            await System.Threading.Tasks.Task.Delay(millisecondsTimeout, ct);
            throw new TimeoutException($"Timeout after {millisecondsTimeout}ms while awaiting a task!");
        }

        private static async System.Threading.Tasks.Task TimeoutInternalAsync(int millisecondsTimeout,
            CancellationToken ct = default)
        {
            await System.Threading.Tasks.Task.Delay(millisecondsTimeout, ct);
            throw new TimeoutException($"Timeout after {millisecondsTimeout}ms while awaiting a task!");
        }

        #endregion


        #region Exception Handling

        public static async UniTask Catch<TException>(this UniTask uniTask, Action<TException> handle)
            where TException : Exception
        {
            try
            {
                await uniTask;
            }
            catch (TException exception)
            {
                handle(exception);
            }
        }

        public static async UniTask<T> Catch<T, TException>(this UniTask<T> uniTask, Func<TException, T> handle)
            where TException : Exception
        {
            try
            {
                return await uniTask;
            }
            catch (TException exception)
            {
                return handle(exception);
            }
        }

        public static async UniTask Catch<TException>(this UniTask uniTask, Action<UniTask, TException> handle)
            where TException : Exception
        {
            try
            {
                await uniTask;
            }
            catch (TException exception)
            {
                handle(uniTask, exception);
            }
        }

        public static async UniTask<T> Catch<T, TException>(this UniTask<T> uniTask,
            Func<UniTask, TException, T> handle)
            where TException : Exception
        {
            try
            {
                return await uniTask;
            }
            catch (TException exception)
            {
                return handle(uniTask, exception);
            }
        }

        public static async UniTask Catch<TException1, TException2>(this UniTask uniTask, Action<TException1> handle1,
            Action<TException2> handle2)
            where TException1 : Exception where TException2 : Exception
        {
            try
            {
                await uniTask;
            }
            catch (TException1 exception)
            {
                handle1(exception);
            }
            catch (TException2 exception)
            {
                handle2(exception);
            }
        }

        public static async UniTask<T> Catch<T, TException1, TException2>(this UniTask<T> uniTask,
            Func<TException1, T> handle1, Func<TException2, T> handle2)
            where TException1 : Exception where TException2 : Exception
        {
            try
            {
                return await uniTask;
            }
            catch (TException1 exception)
            {
                return handle1(exception);
            }
            catch (TException2 exception)
            {
                return handle2(exception);
            }
        }

        public static async UniTask Catch<TException1, TException2>(this UniTask uniTask,
            Action<UniTask, TException1> handle1, Action<UniTask, TException2> handle2)
            where TException1 : Exception where TException2 : Exception
        {
            try
            {
                await uniTask;
            }
            catch (TException1 exception)
            {
                handle1(uniTask, exception);
            }
            catch (TException2 exception)
            {
                handle2(uniTask, exception);
            }
        }

        public static async UniTask<T> Catch<T, TException1, TException2>(this UniTask<T> uniTask,
            Func<UniTask, TException1, T> handle1, Func<UniTask, TException2, T> handle2)
            where TException1 : Exception where TException2 : Exception
        {
            try
            {
                return await uniTask;
            }
            catch (TException1 exception)
            {
                return handle1(uniTask, exception);
            }
            catch (TException2 exception)
            {
                return handle2(uniTask, exception);
            }
        }

        #endregion
    }
}