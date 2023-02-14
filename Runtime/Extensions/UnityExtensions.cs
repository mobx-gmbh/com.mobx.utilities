using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MobX.Utilities
{
    public static class UnityExtensions
    {
        #region Transform

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AnchorX(this RectTransform rectTransform)
        {
            return rectTransform.anchoredPosition.x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AnchorY(this RectTransform rectTransform)
        {
            return rectTransform.anchoredPosition.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void KillChildObjects(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                Object.Destroy(child.gameObject);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPositionAndRotation(this Transform transform, Transform target)
        {
            transform.SetPositionAndRotation(target.position, target.rotation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetLocalPositionAndRotation(this Transform transform, Transform target)
        {
            transform.localPosition = target.localPosition;
            transform.localRotation = target.localRotation;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetPositionAndViewDirection(this Transform transform, Vector3 position, Vector3 viewDirection)
        {
            if (viewDirection == Vector3.zero)
            {
                transform.position = position;
            }
            else
            {
                transform.SetPositionAndRotation(position, Quaternion.LookRotation(viewDirection));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MoveTowards(this Transform transform, Transform target, float maxDistanceDelta, float maxDegreesDelta)
        {
            var position = Vector3.MoveTowards(transform.position, target.position, maxDistanceDelta);
            var rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, maxDegreesDelta);
            transform.SetPositionAndRotation(position, rotation);
        }

        #endregion


        #region Component

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetActive<TComponent>(this TComponent component, bool active) where TComponent : Component
        {
            component.gameObject.SetActive(active);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetParent<TComponent>(this TComponent component, Transform parent)
            where TComponent : Component
        {
            component.transform.SetParent(parent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DontDestroyOnLoad<TComponent>(this TComponent component)
            where TComponent : Component
        {
            component.SetParent(null);
            Object.DontDestroyOnLoad(component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActive<TComponent>(this TComponent component) where TComponent : Component
        {
            return component.gameObject.activeSelf;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActiveInHierarchy<TComponent>(this TComponent component) where TComponent : Component
        {
            return component.gameObject.activeInHierarchy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotActive<TComponent>(this TComponent component) where TComponent : Component
        {
            return !component.gameObject.activeSelf;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotActiveInHierarchy<TComponent>(this TComponent component) where TComponent : Component
        {
            return !component.gameObject.activeInHierarchy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyGameObject<TComponent>(this TComponent component) where TComponent : Component
        {
            Object.Destroy(component.gameObject);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyGameObject<TComponent>(this TComponent component, float secondsDelay) where TComponent : Component
        {
            Object.Destroy(component.gameObject, secondsDelay);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponents<TComponent>(this GameObject gameObject, out TComponent[] components) where TComponent : Component
        {
            components = gameObject.GetComponents<TComponent>();
            return components.IsNotNullOrEmpty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetComponentInChildren<T>(this Component target, out T component, bool includeInactive = false) where T : Component
        {
            component = target.GetComponentInChildren<T>(includeInactive);
            return component != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrCreateComponent<T>(this Component target) where T : Component
        {
            return target.gameObject.GetOrCreateComponent<T>();
        }

        #endregion


        #region GameObject

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DontDestroyOnLoad(this GameObject gameObject, bool setParent = true)
        {
            if (setParent)
            {
                gameObject.transform.SetParent(null);
            }

            Object.DontDestroyOnLoad(gameObject);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrefab(this GameObject gameObject)
        {
            return gameObject.scene.rootCount == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrCreateComponent<T>(this GameObject target) where T : Component
        {
            if (!target.TryGetComponent<T>(out var component))
            {
                component = target.AddComponent<T>();
            }

            return component;
        }

        #endregion


        #region Object

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSelected(this Object obj)
        {
#if UNITY_EDITOR
            return UnityEditor.Selection.activeObject == obj;
#else
            return false;
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void SetObjectDirty(this Object target)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(target);
#endif
        }

        #endregion
    }
}
