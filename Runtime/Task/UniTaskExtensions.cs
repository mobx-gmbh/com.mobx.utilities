using Cysharp.Threading.Tasks;
using MobX.Utilities.Types;
using System;
using System.Runtime.CompilerServices;
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
    }
}
