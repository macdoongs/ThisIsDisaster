using UnityEngine;

public static class AnimatorUtil
{
    public static bool HasParameter(Animator animator, string paramName)
    {
        foreach (var v in animator.parameters)
        {
            if (v.name == paramName)
            {
                return true;
            }
        }
        return false;
    }

    public static void SetInteger(Animator animator, string paramName, int value)
    {
        if (animator == null) return;
        if (HasParameter(animator, paramName))
        {
            animator.SetInteger(paramName, value);
        }
        else
        {
#if UNITY_EDITOR
            string log = string.Format("Animator of [{0}] Doesn't has Parameter [{1}]", animator.gameObject.name, paramName);
            Debug.LogError(log);
#endif
        }
    }

    public static void SetBool(Animator animator, string paramName, bool value)
    {
        if (animator == null) return;
        if (HasParameter(animator, paramName))
        {
            animator.SetBool(paramName, value);
        }
        else
        {
#if UNITY_EDITOR
            string log = string.Format("Animator of [{0}] Doesn't has Parameter [{1}]", animator.gameObject.name, paramName);
            Debug.LogError(log);
#endif
        }
    }

    public static void SetTrigger(Animator animator, string paramName)
    {
        if (animator == null) return;
        if (HasParameter(animator, paramName))
        {
            animator.SetTrigger(paramName);
        }
        else
        {
#if UNITY_EDITOR
            string log = string.Format("Animator of [{0}] Doesn't has Parameter [{1}]", animator.gameObject.name, paramName);
            Debug.LogError(log);
#endif
        }
    }
}
