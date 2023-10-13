using Fusion;
using MobX.Utilities.Libraries;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MobX.Utilities.Fusion
{
    public static class NetworkExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomItem<T>(this NetworkLinkedList<T> source)
        {
            var randomIndex = Random.Range(0, source.Count);
            return source[randomIndex];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomItemWeighted<T>(this NetworkLinkedList<T> source) where T : IWeighted
        {
            var totalWeight = 0f;
            foreach (var weightedElement in source)
            {
                totalWeight += weightedElement.Weight;
            }

            var randomWeight = RNG.Float(0, totalWeight);
            var weight = 0f;
            foreach (var weightedElement in source)
            {
                weight += weightedElement.Weight;
                if (weight >= randomWeight)
                {
                    return weightedElement;
                }
            }

            return source.RandomItem();
        }

        /// <summary>
        ///     Enables or disables the interpolation of this NetworkTransform.
        ///     (Disabling it resets the local transform of the InterpolationTarget to fix leftover offsets.)
        /// </summary>
        public static void SetInterpolation(this NetworkTransform netTrans, bool enabled)
        {
            if (enabled)
            {
                netTrans.InterpolationDataSource = NetworkBehaviour.InterpolationDataSources.Auto;
            }
            else
            {
                netTrans.InterpolationDataSource = NetworkBehaviour.InterpolationDataSources.NoInterpolation;
                netTrans.InterpolationTarget.localPosition = Vector3.zero;
                netTrans.InterpolationTarget.localRotation = Quaternion.identity;
            }
        }

        /// <summary>Relays the call to the underlying Animator</summary>
        public static void SetFloat(this NetworkMecanimAnimator anim, string name, float value)
        {
            anim.Animator.SetFloat(name, value);
        }

        /// <summary>Relays the call to the underlying Animator</summary>
        public static void SetFloat(this NetworkMecanimAnimator anim, int id, float value)
        {
            anim.Animator.SetFloat(id, value);
        }

        /// <summary>Relays the call to the underlying Animator</summary>
        public static void SetInteger(this NetworkMecanimAnimator anim, int id, int value)
        {
            anim.Animator.SetInteger(id, value);
        }

        /// <summary>Relays the call to the underlying Animator</summary>
        public static void SetBool(this NetworkMecanimAnimator anim, string name, bool value)
        {
            anim.Animator.SetBool(name, value);
        }

        /// <summary>Relays the call to the underlying Animator</summary>
        public static void SetBool(this NetworkMecanimAnimator anim, int id, bool value)
        {
            anim.Animator.SetBool(id, value);
        }

        /// <summary>Sets an animation trigger such that it's correctly synced between players</summary>
        public static void SetNetAnimTrigger(this NetworkMecanimAnimator netAnim, int animHash)
        {
            if (netAnim.HasStateAuthority())
            {
                netAnim.SetTrigger(animHash);
            }
            else
            {
                netAnim.Animator.SetTrigger(animHash);
            }
        }

        /// <summary>Adds the given item to this list, if it not already exists in it</summary>
        public static void AddUnique<T>(this NetworkLinkedList<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }

        /// <summary>Semantic shortcut for this.Object != null, which is true after an object is spawned</summary>
        public static bool IsSpawned(this NetworkBehaviour behaviour)
        {
            return behaviour.Object != null;
        }

        /// <summary>Shortcut for this.Object != null &amp;&amp; this.Object.HasStateAuthority</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasStateAuthority(this NetworkBehaviour behaviour)
        {
            return behaviour.HasStateAuthority;
        }

        /// <summary>Efficient way to check state authority</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasStateAuthority(this AdvancedNetworkBehaviour behaviour)
        {
            return behaviour.LocalHasStateAuthority;
        }

        /// <summary>Shortcut for this.Object != null &amp;&amp; this.Object.HasStateAuthority</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LacksStateAuthority(this NetworkBehaviour behaviour)
        {
            return !behaviour.HasStateAuthority;
        }

        /// <summary>Efficient way to check for lack of state authority</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LacksStateAuthority(this AdvancedNetworkBehaviour behaviour)
        {
            return !behaviour.LocalHasStateAuthority;
        }

        /// <summary>Efficient way to get if the object was spawned, true after spawned false after despawned</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSpawned(this AdvancedNetworkBehaviour behaviour)
        {
            return behaviour.LocalIsSpawned;
        }
    }
}
