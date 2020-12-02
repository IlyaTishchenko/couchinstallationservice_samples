using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Unified popup UI allowing to proceed or cancel performing action
public class AreYouSureUI : MonoBehaviour
{
    [SerializeField]
    public Animator animatorComponent = null;

    [System.NonSerialized]
    public System.Action callback = null;

    public void YesButton()
    {
        animatorComponent.SetTrigger("Hide");

        if (callback != null)
        {
            callback();
        }

        callback = null;
    }

    public void NoButton()
    {
        animatorComponent.SetTrigger("Hide");
        callback = null;
    }
}
