using UnityEngine;

namespace MobX.Utilities
{
    public static class AnimatorExtensions
    {
        /// <summary>Returns true if the provided Animator contains the provided parameter</summary>
        public static bool HasParameter(this Animator animator, string paramName)
        {
            foreach (var parameter in animator.parameters)
            {
                if (parameter.name == paramName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Returns true if the provided Animator contains the provided parameter</summary>
        public static bool HasParameter(this Animator animator, int paramNameHash)
        {
            foreach (var parameter in animator.parameters)
            {
                if (parameter.nameHash == paramNameHash)
                {
                    return true;
                }
            }

            return false;
        }
    }
}