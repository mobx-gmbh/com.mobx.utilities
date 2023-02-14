using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MobX.Utilities.Fmod
{
    public static class FmodExtensions
    {
        /// <summary>
        /// Starts playback if not already playing
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Play(this EventInstance eventInstance)
        {
            eventInstance.getPlaybackState(out var state);
            if (state != PLAYBACK_STATE.PLAYING)
            {
                eventInstance.start();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShotAttached(this EventReference eventReference, Transform target, float volume = 1f)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstance.set3DAttributes(target.To3DAttributes());
            eventInstance.setVolume(volume);
            eventInstance.start();
            eventInstance.release();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShot(this EventReference eventReference, Vector3 position = default)
        {
            RuntimeManager.PlayOneShot(eventReference, position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShot<TComponent>(this EventReference eventReference, TComponent component) where TComponent : Component
        {
            RuntimeManager.PlayOneShot(eventReference, component.transform.position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShotIf(this EventReference eventReference, bool condition, Vector3 position = default)
        {
            if (condition)
            {
                RuntimeManager.PlayOneShot(eventReference, position);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShotIf(this EventReference eventReference, Func<bool> condition, Vector3 position = default)
        {
            if (condition())
            {
                RuntimeManager.PlayOneShot(eventReference, position);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShotIfNotNull(this EventReference eventReference)
        {
            if (eventReference.IsNull)
            {
                return;
            }
            RuntimeManager.PlayOneShot(eventReference);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShotAttachedIfNotNull(this EventReference eventReference, Transform target)
        {
            if (eventReference.IsNull)
            {
                return;
            }
            RuntimeManager.PlayOneShotAttached(eventReference.Guid, target.gameObject);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PlayOneShotAttachedIfNotNull(this EventReference eventReference, Vector3 pos)
        {
            if (eventReference.IsNull)
            {
                return;
            }
            RuntimeManager.PlayOneShot(eventReference.Guid, pos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EventInstance PlayAt(this EventReference eventReference, Transform target)
        {
            if (eventReference.IsNull)
            {
                Debug.LogError("Provided Event Instance for Target " + target + " was null");
                return new EventInstance();
            }
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            eventInstance.set3DAttributes(target.To3DAttributes());
            eventInstance.start();
            return eventInstance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EventInstance PlayAttached(this EventReference eventReference, Transform target)
        {
            if (eventReference.IsNull)
            {
                Debug.LogError("Provided Event Instance for Target " + target + " was null");
                return new EventInstance();
            }
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            RuntimeManager.AttachInstanceToGameObject(eventInstance, target);
            eventInstance.start();
            return eventInstance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EventInstance CreateAttached(this EventReference eventReference, Transform target)
        {
            if (eventReference.IsNull)
            {
                Debug.LogError("Provided Event Instance for Target " + target + " was null");
                return new EventInstance();
            }
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            RuntimeManager.AttachInstanceToGameObject(eventInstance, target);
            return eventInstance;
        }
    }
}
