using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityStandardAssets.CrossPlatformInput;

public class UIButton : MonoBehaviour
{
    public string buttonName;
    public UnityEvent OnClickDown;
    public UnityEvent OnClickUp;
    public UnityEvent<bool> OnClickToggle;

    bool toggle;

    public void SetDownState()
    {
        CrossPlatformInputManager.SetButtonDown(buttonName);
        OnClickDown.Invoke();

        toggle = !toggle;
        OnClickToggle.Invoke(toggle);
    }

    public void SetUpState()
    {
        CrossPlatformInputManager.SetButtonUp(buttonName);
        OnClickUp.Invoke();
    }
}
